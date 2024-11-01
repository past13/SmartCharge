using Microsoft.EntityFrameworkCore;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Connector;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.ConnectorTests;

public class UpdateConnectorHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly UpdateConnectorHandler _handler;
    
    public UpdateConnectorHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _connectorRepository = new ConnectorRepository(InMemoryDb);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _chargeStationRepository);
        
        _handler = new UpdateConnectorHandler(_unitOfWork, _groupRepository, _chargeStationRepository, _connectorRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenConnectorNotExists()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");
        var connectorEntity = ConnectorEntity.Create("Test Connector 1", 1);

        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExistId = Guid.NewGuid();
        var command = new UpdateConnectorCommand(notExistId, connectorEntity.Id, "Test Connector 2", 1);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, $"A Connector with Id {notExistId} does not exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorExists()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");
        var connectorEntity = ConnectorEntity.Create("Test Connector 1", 1);

        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new UpdateConnectorCommand(connectorEntity.Id, chargeStationEntity.Id, "Updated Test Group", 1);
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_UpdateAnotherChargeStation()
    {
        var groupEntity1 = GroupEntity.Create("Test Group 1");
        var chargeStationEntity1 = ChargeStationEntity.Create("Test ChargeStation 1");
        var connectorEntity1 = ConnectorEntity.Create("Test Connector 1", 1);
        var connectorEntity2 = ConnectorEntity.Create("Test Connector 2", 1);
        
        chargeStationEntity1.AddConnector(connectorEntity1);
        chargeStationEntity1.AddConnector(connectorEntity2);

        groupEntity1.AddChargeStation(chargeStationEntity1);
        
        var groupEntity2 = GroupEntity.Create("Test Group 2");
        var chargeStationEntity2 = ChargeStationEntity.Create("Test ChargeStation 2");
        var connectorEntity3 = ConnectorEntity.Create("Test Connector 3", 1);

        chargeStationEntity2.AddConnector(connectorEntity3);
        groupEntity2.AddChargeStation(chargeStationEntity2);
        
        InMemoryDb.Groups.AddRange(groupEntity1, groupEntity2);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new UpdateConnectorCommand(connectorEntity2.Id, chargeStationEntity2.Id, "Updated Test Group", 1);
        var result = await _handler.Handle(command, CancellationToken.None);

        var connectors2 = InMemoryDb.Groups
            .Include(g => g.ChargeStations)
            .First(g => g.Id == groupEntity2.Id)
            .ChargeStations.First(cs => cs.Id == chargeStationEntity2.Id)
            .Connectors;
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, connectors2.Count);
        Assert.Contains(connectors2, c => c.Id == connectorEntity2.Id);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_SwapChargeStationRequiredAtLeastOneConnector()
    {
        var groupEntity1 = GroupEntity.Create("Test Group 1");
        var chargeStationEntity1 = ChargeStationEntity.Create("Test ChargeStation 1");
        var connectorEntity1 = ConnectorEntity.Create("Test Connector 1", 1);
        
        chargeStationEntity1.AddConnector(connectorEntity1);
        groupEntity1.AddChargeStation(chargeStationEntity1);
        
        var groupEntity2 = GroupEntity.Create("Test Group 2");
        var chargeStationEntity2 = ChargeStationEntity.Create("Test ChargeStation 2");
        var connectorEntity2 = ConnectorEntity.Create("Test Connector 2", 1);

        chargeStationEntity2.AddConnector(connectorEntity2);
        groupEntity2.AddChargeStation(chargeStationEntity2);
        
        InMemoryDb.Groups.AddRange(groupEntity1, groupEntity2);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new UpdateConnectorCommand(connectorEntity1.Id, chargeStationEntity2.Id, "Updated Test Group", 1);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A ChargeStation Id {chargeStationEntity1.Id} cannot have less than 1 connector.",
            result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenConnectorRowSateDeleting()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");
        var connectorEntity = ConnectorEntity.Create("Test Connector 1", 1);

        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        connectorEntity.UpdateRowState(RowState.PendingDelete);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new UpdateConnectorCommand(connectorEntity.Id, chargeStationEntity.Id, "Updated Test Group", 1);
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A Connector with Id {connectorEntity.Id} already deleting.", result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorUpdateNewChargeStationGetNewNumber()
    {
        var chargeStationEntity1 = ChargeStationEntity.Create("Test ChargeStation");

        var connectorEntity1 = ConnectorEntity.Create("Test Connector 1", 1);
        var connectorEntity2 = ConnectorEntity.Create("Test Connector 2", 1);
        var connectorEntity3 = ConnectorEntity.Create("Test Connector 3", 1);

        chargeStationEntity1.AddConnector(connectorEntity1);
        chargeStationEntity1.AddConnector(connectorEntity2);
        chargeStationEntity1.AddConnector(connectorEntity3);

        var chargeStationEntity2 = ChargeStationEntity.Create("Test ChargeStation");
        var connectorEntity4 = ConnectorEntity.Create("Test Connector 4", 1);
        chargeStationEntity2.AddConnector(connectorEntity4);
        
        chargeStationEntity1.RemoveConnector(connectorEntity3);
        chargeStationEntity2.AddConnector(connectorEntity3);

        Assert.Equal(2, connectorEntity3.ConnectorNumber);
    }
}