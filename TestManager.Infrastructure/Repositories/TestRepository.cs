using Microsoft.EntityFrameworkCore;
using TestManager.Application.Repositories;
using TestManager.Domain.Entities;
using TestManager.Infrastructure.Persistence;

namespace TestManager.Infrastructure.Repositories;

public class TestRepository(AppDbContext dbContext) : ITestRepository
{
    public async Task<IReadOnlyCollection<Test>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Tests
            .AsNoTracking()
            .Include(test => test.Questions)
            .OrderBy(test => test.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Test?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tests
            .Include(test => test.Questions)
            .ThenInclude(question => question.AnswerOptions)
            .FirstOrDefaultAsync(test => test.Id == id, cancellationToken);
    }

    public async Task AddAsync(Test test, CancellationToken cancellationToken = default)
    {
        await dbContext.Tests.AddAsync(test, cancellationToken);
    }

    public Task DeleteAsync(Test test, CancellationToken cancellationToken = default)
    {
        dbContext.Tests.Remove(test);

        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
