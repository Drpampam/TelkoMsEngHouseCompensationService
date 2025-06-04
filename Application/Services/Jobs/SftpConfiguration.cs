using Application.Features.Configurations;
using Application.Features.Constants;
using Application.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WinSCP;

namespace Application.Services.Jobs
{
    public class SFTPConfiguration : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly SftpConnectionDetails _archCredentials;
        private readonly ILogger<SFTPConfiguration> _logger;
        private readonly FilePathConfiguration _filePathConfiguration;



        public SFTPConfiguration(
                                        IServiceProvider serviceProvider,
                                        ILogger<SFTPConfiguration> logger,
                                        IOptions<SftpConnectionDetails> archCredentials,
                                        IOptions<FilePathConfiguration> filepath
        )
        {
            _logger = logger;
            this.serviceProvider = serviceProvider;
            _filePathConfiguration = filepath.Value;
            _archCredentials = archCredentials.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                _logger.LogInformation("Cancellation requested. Stopping file watcher...");
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string tempDirectory = Directory.CreateDirectory(_filePathConfiguration.TempPath!).FullName;

                    using (Session session = new Session())
                    {
                        SessionOptions sessionOptions = new SessionOptions
                        {
                            Protocol = Protocol.Sftp,
                            HostName = _archCredentials.Host,
                            UserName = _archCredentials.UserId,
                            Password = _archCredentials.Password,
                            PortNumber = int.Parse(_archCredentials.PortNumber),
                            SshHostKeyFingerprint = _archCredentials.HostKeyFingerPrint,

                        };
                        sessionOptions.AddRawSettings("SendBufKeepAlive", "1");
                        sessionOptions.AddRawSettings("KeepAlive", "1");
                        sessionOptions.Timeout = TimeSpan.FromMinutes(20);
                        session.Open(sessionOptions);

                        string remoteDirectory = _filePathConfiguration.InboxPath!;
                        string WipDirectory = _filePathConfiguration.WIPPath!;
                        string errorDirectory = _filePathConfiguration.ErrorPath!;
                        string originFileDirectory = _filePathConfiguration.OriginBatchRecords!;


                        RemoteDirectoryInfo directoryInfo = session.ListDirectory(remoteDirectory);
                        RemoteDirectoryInfo WipDirectoryInfo = session.ListDirectory(WipDirectory);

                        if (directoryInfo.Files.FirstOrDefault()!.Length > 0)
                        {
                            foreach (RemoteFileInfo remoteFileInfo in directoryInfo.Files)
                            {
                                if (!remoteFileInfo.IsDirectory)
                                {
                                    string sourceFilePath = Path.Combine(remoteDirectory, remoteFileInfo.Name).Replace("\\", "/");
                                    string destinationFilePath = Path.Combine(_filePathConfiguration.WIPPath!, remoteFileInfo.Name).Replace("\\", "/");
                                    string tempFilePath = Path.Combine(tempDirectory, remoteFileInfo.Name);
                                    string errorFilePath = Path.Combine(errorDirectory, remoteFileInfo.Name).Replace("\\", "/");
                                    string originFilePath = Path.Combine(originFileDirectory, remoteFileInfo.Name);

                                    _logger.LogInformation($"Source File Path: {sourceFilePath}");
                                    _logger.LogInformation($"Destination File Path: {destinationFilePath}");
                                    _logger.LogInformation($"Temporary File Path: {tempFilePath}");

                                    try
                                    {
                                        // Move the file on the SFTP server from inbox to wip
                                        session.MoveFile(sourceFilePath, destinationFilePath);

                                        // Download the file to the temporary local path or BackUp(OriginBatchRecords) path
                                        if (!File.Exists(originFilePath))
                                        {
                                            session.GetFiles(destinationFilePath, originFilePath).Check();
                                        }
                                        if (!File.Exists(tempFilePath))
                                        {
                                            session.GetFiles(destinationFilePath, tempFilePath).Check();
                                        }

                                        _logger.LogInformation($"Moved {remoteFileInfo.Name} to {_filePathConfiguration.WIPPath} on SFTP server and downloaded to temporary path.");
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"Error processing file {remoteFileInfo.Name}: {ex.Message}");
                                        if (session.FileExists(destinationFilePath))
                                        {
                                            if (session.FileExists(errorFilePath))
                                            {
                                                string newErrorFilePath = GetUniqueFileNameSftp(errorDirectory, remoteFileInfo.Name, session);
                                                session.MoveFile(destinationFilePath, newErrorFilePath);
                                                _logger.LogInformation($"Moved and renamd file to {newErrorFilePath} in the error directory");
                                            }
                                            else
                                            {
                                                string newErrorFilePath = GetUniqueFileNameSftp(errorDirectory, remoteFileInfo.Name, session);
                                                session.MoveFile(destinationFilePath, newErrorFilePath);
                                                _logger.LogInformation($"Moved file to {errorFilePath} in the error directory");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (WipDirectoryInfo.Files.FirstOrDefault()!.Length > 0)
                        {
                            foreach (RemoteFileInfo WipremoteFileInfo in WipDirectoryInfo.Files)
                            {
                                if (!WipremoteFileInfo.IsDirectory)
                                {
                                    string remoteFilePath = Path.Combine(WipDirectory, WipremoteFileInfo.Name).Replace("\\", "/");
                                    string tempFilePath = Path.Combine(tempDirectory, WipremoteFileInfo.Name);
                                    string errorFilePath = Path.Combine(errorDirectory, WipremoteFileInfo.Name).Replace("\\", "/");
                                    string originFilePath = Path.Combine(originFileDirectory, WipremoteFileInfo.Name);


                                    _logger.LogInformation($"Remote File Path: {remoteFilePath}");
                                    _logger.LogInformation($"Temporary File Path: {tempFilePath}");

                                    try
                                    {
                                        // Download the file on the SFTP server from wip to local tempPath
                                        if (!File.Exists(originFilePath))
                                        {
                                            session.GetFiles(remoteFilePath, originFilePath).Check();
                                        }
                                        if (!File.Exists(tempFilePath))
                                        {
                                            session.GetFiles(remoteFilePath, tempFilePath).Check();
                                        }
                                        _logger.LogInformation($"Downloaded {WipremoteFileInfo.Name} to {originFilePath} on local drive");

                                        var file = await HandleFileAsync(tempFilePath, stoppingToken, session);
                                        if (file.ResponseCode == ResponseCodes.SUCCESS)
                                        {
                                            session.RemoveFile(remoteFilePath);
                                        }
                                        else
                                        {
                                            if (session.FileExists(remoteFilePath))
                                            {
                                                if (session.FileExists(errorFilePath))
                                                {
                                                    string newErrorFilePath = GetUniqueFileNameSftp(errorDirectory, WipremoteFileInfo.Name, session);
                                                    session.MoveFile(remoteFilePath, newErrorFilePath);
                                                    _logger.LogInformation($"Moved and renamd file to {newErrorFilePath} in the error directory");
                                                }
                                                else
                                                {
                                                    string newErrorFilePath = GetUniqueFileNameSftp(errorDirectory, WipremoteFileInfo.Name, session);
                                                    session.MoveFile(remoteFilePath, newErrorFilePath);
                                                    _logger.LogInformation($"Moved file to {errorFilePath} in the error directory");
                                                }
                                            }
                                        }

                                        // Optionally, delete the temporary file after processing
                                        if (File.Exists(tempFilePath))
                                        {
                                            File.Delete(tempFilePath);
                                            _logger.LogInformation($"Deleted temporary file: {tempFilePath}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"Error downloading or processing file {WipremoteFileInfo.Name}: {ex.Message}");
                                        if (session.FileExists(remoteFilePath))
                                        {
                                            if (session.FileExists(errorFilePath))
                                            {
                                                string newErrorFilePath = GetUniqueFileNameSftp(errorDirectory, WipremoteFileInfo.Name, session);
                                                session.MoveFile(remoteFilePath, newErrorFilePath);
                                                _logger.LogInformation($"Moved and renamd file to {newErrorFilePath} in the error directory");
                                            }
                                            else
                                            {
                                                string newErrorFilePath = GetUniqueFileNameSftp(errorDirectory, WipremoteFileInfo.Name, session);
                                                session.MoveFile(remoteFilePath, newErrorFilePath);
                                                _logger.LogInformation($"Moved file to {errorFilePath} in the error directory");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex, "Task was canceled.");
                }
            }
        }

        private async Task<BaseResponse<string>> HandleFileAsync(string filePath, CancellationToken cancellationToken, Session session)
        {
            var data = new BaseResponse<string>();

            try
            {
                _logger.LogInformation($"Processing WIP file: {filePath}");
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileWithExt = Path.GetFileName(filePath);

                using (var scope = serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<INeftBulkProcessing>();

                    var tran = await repository.TranCheck(filePath, session);
                    if (tran.ResponseCode == ResponseCodes.SUCCESS)
                    {
                        _logger.LogInformation("Completed processing of WIP file: {filePath}", filePath);
                        data.Message = tran.Message;
                        data.ResponseCode = ResponseCodes.SUCCESS;
                        return data;
                    }
                    File.Delete(filePath);
                    _logger.LogError("Failed to process WIP file: {filePath}", filePath);
                    data.Message = tran.Message;
                    data.ResponseCode = ResponseCodes.FAILURE;
                    return data;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, "ProcessWipFileAsync was canceled.");
                data.Message = ResponseCodes.FAILURE + ex.Message;
                data.ResponseCode = ResponseCodes.FAILURE;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the WIP file");
                data.Message = ResponseCodes.FAILURE + ex.Message;
                data.ResponseCode = ResponseCodes.FAILURE;
            }
            return data;
        }

        private string GetUniqueFileName(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath)!;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            int counter = 1;
            string newFilePath;
            do
            {
                string newFileName = $"{fileNameWithoutExtension}_{counter}{extension}";
                newFilePath = Path.Combine(directory, newFileName).Replace("\\", "/");
                counter++;
            } while (File.Exists(newFilePath));

            return newFilePath;
        }

        private string GetUniqueFileNameSftp(string directory, string fileName, Session session)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            long counter = GenerateNumericValue();
            string newFilePath;
            do
            {
                string newFileName = $"{fileNameWithoutExtension}_{counter}{extension}";
                newFilePath = Path.Combine(directory, newFileName).Replace("\\", "/");
            } while (session.FileExists(newFilePath));

            return newFilePath;
        }

        private long GenerateNumericValue()
        {
            var dateTime = DateTime.Now;
            Random random = new Random();
            var timestamp = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
            var randomPart = random.Next(100, 99999999);
            var uniqueReference = timestamp + randomPart;
            return uniqueReference;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping file watcher...");
            await base.StopAsync(stoppingToken);
        }
    }
}