using TestManager.Application.DTOs;

namespace TestManager.Application.Services;

public interface ITestService
{
    Task<IReadOnlyCollection<TestListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TestDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TestDetailsDto> CreateAsync(CreateTestRequest request, CancellationToken cancellationToken = default);

    Task<TestDetailsDto?> UpdateAsync(int id, UpdateTestRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
