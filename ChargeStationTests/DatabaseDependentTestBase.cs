using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartCharge.DataLayer;

namespace ChargeStationTests;

public abstract class DatabaseDependentTestBase : TestBase, IDisposable 
{
    protected ApplicationDbContext InMemoryDb { get; set; }
    protected DatabaseDependentTestBase() 
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        InMemoryDb = new ApplicationDbContext(options);
    }
    
    public void Dispose() 
    {
        InMemoryDb.Dispose();
    }
}