using TLRProcessor.Services;

namespace TLRProcessor.Jobs;

public class SmsTlrJob
{
    private readonly LargeFileProcessor _processor;

    public SmsTlrJob(LargeFileProcessor processor)
    {
        _processor = processor;
    }

    public async Task RunAsync(string filePath)
    {
        await _processor.ProcessAsync(filePath);
    }
}