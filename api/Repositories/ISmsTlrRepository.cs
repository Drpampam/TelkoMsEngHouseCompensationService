using TLRProcessor.DTOs;
using TLRProcessor.Models;

namespace TLRProcessor.Repositories;

public interface ISmsTlrRepository
{
    Task BulkInsertAsync(IEnumerable<SmsTlrRecord> records);
    Task BulkInsertAsyncV2(IEnumerable<SmsTlrRecord> records);
    Task BulkInsertAsyncV3(IEnumerable<SmsTlrRecord> records);
    Task<(List<SmsTlrRecord> Records, int TotalCount)> GetSmsTlrRecordsAsync(GetReporting filter);

}