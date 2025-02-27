using Microsoft.EntityFrameworkCore;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Connector;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.ConnectorTests;

public class CreateConnectorHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly CreateConnectorHandler _handler;
    
    public CreateConnectorHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _connectorRepository = new ConnectorRepository(InMemoryDb);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _chargeStationRepository);
        
        _handler = new CreateConnectorHandler(_unitOfWork, _groupRepository, _chargeStationRepository, _connectorRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenConnectorNameExists()
    {
        var nameExist = "Test Connector";

        var groupEntity = GroupEntity.Create("TestGroup 1");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        var connector = ConnectorEntity.Create(nameExist, 1);

        chargeStationEntity.AddConnector(connector);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new CreateConnectorCommand(nameExist, 1, chargeStationEntity.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, $"A Connector with the name {nameExist} already exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorNameNotExists()
    {
        var groupEntity = GroupEntity.Create("TestGroup 1");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        var connector = ConnectorEntity.Create("Test Connector 1", 1);
    
        chargeStationEntity.AddConnector(connector);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new CreateConnectorCommand("Test Connector 2", 1, chargeStationEntity.Id);
        var result = await _handler.Handle(command, CancellationToken.None);
    
        var connectors = InMemoryDb.Groups
            .Include(g => g.ChargeStations)
            .First(g => g.Id == groupEntity.Id)
            .ChargeStations.First(cs => cs.Id == chargeStationEntity.Id)
            .Connectors;
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, connectors.Count);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorRemoveInMiddleAndAddNewOneItReplaceItPlace()
    {
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");

        var connectorEntity1 = ConnectorEntity.Create("Test Connector 1", 1);
        var connectorEntity2 = ConnectorEntity.Create("Test Connector 2", 1);
        var connectorEntity3 = ConnectorEntity.Create("Test Connector 3", 1);

        chargeStationEntity.AddConnector(connectorEntity1);
        chargeStationEntity.AddConnector(connectorEntity2);
        chargeStationEntity.AddConnector(connectorEntity3);

        chargeStationEntity.RemoveConnector(connectorEntity1);
        
        var connectorEntity4 = ConnectorEntity.Create("Test Connector 4", 1);
        
        chargeStationEntity.AddConnector(connectorEntity4);
        Assert.Equal(1, connectorEntity4.ConnectorNumber);
    }
}