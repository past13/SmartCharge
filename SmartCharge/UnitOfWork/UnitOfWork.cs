using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SmartCharge.DataLayer;

namespace SmartCharge.UnitOfWork;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IDbContextTransaction _transaction;

    public async Task BeginTransactionAsync()
    {
        _transaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}