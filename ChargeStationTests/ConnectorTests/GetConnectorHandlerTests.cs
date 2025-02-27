﻿using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Connector;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.ConnectorTests;

public class GetConnectorHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectorRepository _connectorRepository;

    private readonly GetConnectorHandler _handler;
    
    public GetConnectorHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _connectorRepository = new ConnectorRepository(InMemoryDb);
        
        _handler = new GetConnectorHandler(_unitOfWork, _connectorRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorExists()
    {
        var group = GroupEntity.Create("Test Group 1");
        var chargeStation = ChargeStationEntity.Create("Test ChargeStation 1");
        var connector = ConnectorEntity.Create("Test Connector 1", 1);
        
        chargeStation.AddConnector(connector);
        group.AddChargeStation(chargeStation);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new GetConnectorByIdQuery(connector.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenConnectorNotExists()
    {
        var group = GroupEntity.Create("Test Group 1");
        var chargeStation = ChargeStationEntity.Create("Test ChargeStation 1");
        var connector = ConnectorEntity.Create("Test Connector 1", 1);
        
        chargeStation.AddConnector(connector);
        group.AddChargeStation(chargeStation);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();

        var notExistId = Guid.NewGuid();
        
        // Act
        var command = new GetConnectorByIdQuery(notExistId);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A Connector with Id {notExistId} does not exist.", result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenConnectorRowSateDeleting()
    {
        var group = GroupEntity.Create("Test Group 1");
        var chargeStation = ChargeStationEntity.Create("Test ChargeStation 1");
        var connector = ConnectorEntity.Create("Test Connector 1", 1);
        
        chargeStation.AddConnector(connector);
        group.AddChargeStation(chargeStation);
        
        connector.UpdateRowState(RowState.PendingDelete);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new GetConnectorByIdQuery(connector.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A Connector with Id {connector.Id} already deleting.", result.Error);
    }
}