using TestManager.Domain.Entities;

namespace TestManager.Application.Repositories;

public interface ITestRepository
{
    Task<IReadOnlyCollection<Test>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Test?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(Test test, CancellationToken cancellationToken = default);

    Task DeleteAsync(Test test, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
