using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using SmartCharge.Commands.Group;
using SmartCharge.DataLayer;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;

namespace ChargeStationTests;

public class UpdateGroupHandlerTests : DatabaseDependentTestBase
{
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly Mock<IConnectorRepository> _connectorRepository;

    private readonly UpdateGroupHandler _handler;
    
    public UpdateGroupHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object);
        
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        _connectorRepository = new Mock<IConnectorRepository>();
        
        _handler = new UpdateGroupHandler(_groupRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNotExists()
    {
        var command = new UpdateGroupCommand(Guid.NewGuid(), "Test Group 1", 1, []);
        var group = GroupEntity.Create(command.Name, command.CapacityInAmps);

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new UpdateGroupCommand(Guid.NewGuid(), "Test Group 2", 1, []);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A Group does not exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupExists()
    {
        var group = GroupEntity.Create("Init Group 1", 1);
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using (var context = new ApplicationDbContext(options))
        {
            context.Groups.Add(group);
            await context.SaveChangesAsync();
        }
        
        var context1 = new ApplicationDbContext(options);
        var context2 = new ApplicationDbContext(options);
        
        var existingGroup1 = await context1.Groups.FirstAsync(g => g.Id == group.Id);
        var existingGroup2 = await context2.Groups.FirstAsync(g => g.Id == group.Id);

        existingGroup1.Update("Updated Group 1", 1);
        
        existingGroup2.Update("Updated Group 2", 1);
        await context2.SaveChangesAsync();

        try
        {
            await context1.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is GroupEntity)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = await context2.Groups
                        .AsNoTracking()
                        .FirstOrDefaultAsync(g => g.Id == (Guid)proposedValues["Id"]);

                    // Optionally, reload the database values and inform the user
                    if (databaseValues != null)
                    {
                        // Update the row version and inform the user

                        // Optionally throw an exception or return a specific result
                        throw new InvalidOperationException("The group has been modified by another user.");
                    }
                }
            }
        }

        // Verify the group has the updated value from context 1
        var updatedGroup = await context1.Groups.FirstAsync(g => g.Id == group.Id);
        Assert.Equal("Updated Group 1", updatedGroup.Name);
        
        // Act
    
        // Assert
    }
    
    // [Fact]
    // public async Task Handle_ShouldReturnSuccess_WhenNewChargeStationNotExists()
    // {
    //     var chargeStation1 = new ChargeStationRequest
    //     {
    //         Name = "Test ChargeStation 1",
    //     };
    //     
    //     var command = new CreateGroupCommand("Test Group", 1, chargeStation1);
    //     var group = GroupEntity.Create(command.Name, command.CapacityInAmps);
    //
    //     var chargeStationEntity = ChargeStationEntity.Create(chargeStation1.Name);
    //     group.AddChargeStation(chargeStationEntity);
    //     
    //     InMemoryDb.Groups.Add(group);
    //     await InMemoryDb.SaveChangesAsync();
    //     
    //     var chargeStation2 = new ChargeStationRequest
    //     {
    //         Name = "Test ChargeStation 2",
    //     };
    //     
    //     // Act
    //     var notExist = new CreateGroupCommand("Test Group 1", 1, chargeStation2);
    //     var result = await _handler.Handle(notExist, CancellationToken.None);
    //
    //     // Assert
    //     Assert.True(result.IsSuccess);
    // }
    //
    // [Fact]
    // public async Task Handle_ShouldReturnError_WhenChargeStationAlreadyExists()
    // {
    //     var chargeStation = new ChargeStationRequest
    //     {
    //         Name = "Test ChargeStation",
    //     };
    //     
    //     var command = new CreateGroupCommand("Test Group", 1, chargeStation);
    //     var group = GroupEntity.Create(command.Name, command.CapacityInAmps);
    //
    //     var chargeStationEntity = ChargeStationEntity.Create(chargeStation.Name);
    //     group.AddChargeStation(chargeStationEntity);
    //     
    //     InMemoryDb.Groups.Add(group);
    //     await InMemoryDb.SaveChangesAsync();
    //     
    //     // Act
    //     var notExist = new CreateGroupCommand("Test Group 1", 1, chargeStation);
    //     var result = await _handler.Handle(notExist, CancellationToken.None);
    //
    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Contains(result.Error, "A ChargeStation with the name 'Test ChargeStation' already exists.");
    // }
}