using TLRProcessor.Services;

namespace TLRProcessor.Jobs;

public class SmsTlrJob
{
    private readonly ILargeFileProcessor _processor;

    public SmsTlrJob(ILargeFileProcessor processor)
    {
        _processor = processor;
    }

    public async Task RunAsync(string filePath)
    {
        await _processor.ProcessAsync(filePath);
    }
}