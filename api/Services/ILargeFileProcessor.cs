using TLRProcessor.DTOs;
using TLRProcessor.Models;
using TLRProcessor.Pagination;
using TLRProcessor.Responses;

namespace TLRProcessor.Services
{
    public interface ILargeFileProcessor
    {
        Task<BaseResponse<string>> UploadFileAsync(IFormFile file);
        Task ProcessAsync(string filePath);
        Task<BaseResponse<PaginationResult<SmsTlrRecord>>> GetAllContacts(GetReporting? filter = null);
    }
}
