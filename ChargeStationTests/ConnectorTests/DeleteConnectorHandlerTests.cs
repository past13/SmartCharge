using AutoMapper;
using Moq;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Connector;
using SmartCharge.Repository;

namespace ChargeStationTests.ConnectorTests;

public class DeleteConnectorHandlerTests : DatabaseDependentTestBase
{
    private readonly Mock<IMapper> _mapper;
    private readonly IConnectorRepository _connectorRepository;

    private readonly DeleteConnectorHandler _handler;
    
    public DeleteConnectorHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);
        
        _handler = new DeleteConnectorHandler(_connectorRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeChargeStationNotExists()
    {
        var groupEntity = GroupEntity.Create("Test Group 1", 1);
        
        var connectorEntity = ConnectorEntity.Create("Test Connector", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        
        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new DeleteConnectorCommand(chargeStationEntity.Id, Guid.NewGuid());
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"Connector with Id {notExist.Id} does not exists.", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenChargeChargeStationExists()
    {
        var groupEntity = GroupEntity.Create("Test Group 1", 1);
        
        var connectorEntity = ConnectorEntity.Create("Test Connector", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        
        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var exist = new DeleteConnectorCommand(chargeStationEntity.Id, connectorEntity.Id);
        var result = await _handler.Handle(exist, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
}