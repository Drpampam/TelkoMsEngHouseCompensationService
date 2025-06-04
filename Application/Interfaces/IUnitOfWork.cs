using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {

        Task ReloadEntityAsync(IEnumerable<object> entities);
        Task<int> SaveChangesAsync();
        Task DetachEntityTracker();
        void Dispose();
    }
}
