using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;

namespace TestProject1;

public abstract class DatabaseDependentTestBase : TestBase, IDisposable 
{
    protected ApplicationDbContext InMemoryDb { get; set; }
    protected DatabaseDependentTestBase() 
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        InMemoryDb = new ApplicationDbContext(options);
    }
    
    public void Dispose() 
    {
        InMemoryDb.Dispose();
    }
}