using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.GroupTests;

public class UpdateGroupHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly UpdateGroupHandler _handler;
    
    public UpdateGroupHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _connectorRepository = new ConnectorRepository(InMemoryDb);

        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _chargeStationRepository);
        
        _handler = new UpdateGroupHandler(_unitOfWork, _groupRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNotExists()
    {
        var group = GroupEntity.Create("Test Group 1");

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new UpdateGroupCommand(Guid.NewGuid(), "Test Group 2");
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A Group does not exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupExists()
    {
        var group = GroupEntity.Create("Test Group 1");

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();

        var expectedName = "Test Group 2";

        // Act
        var command = new UpdateGroupCommand(group.Id, expectedName);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, InMemoryDb.Groups.First().Name);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupRowSateDeleting()
    {
        var group = GroupEntity.Create("Test Group 1");

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();

        var expectedName = "Test Group 2";

        group.UpdateRowState(RowState.PendingDelete);
        
        // Act
        var command = new UpdateGroupCommand(group.Id, expectedName);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A Group with Id {group.Id} already deleting.", result.Error);
    }
}