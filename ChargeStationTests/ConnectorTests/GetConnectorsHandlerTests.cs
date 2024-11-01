using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Connector;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.ConnectorTests;

public class GetConnectorsHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectorRepository _connectorRepository;

    private readonly GetConnectorsHandler _handler;
    
    public GetConnectorsHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _connectorRepository = new ConnectorRepository(InMemoryDb);
        
        _handler = new GetConnectorsHandler(_unitOfWork, _connectorRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorsExists()
    {
        var group1 = GroupEntity.Create("Test Group 1");
        var chargeStation1 = ChargeStationEntity.Create("Test ChargeStation 1");
        var connector1 = ConnectorEntity.Create("Test Connector 1", 1);
        
        chargeStation1.AddConnector(connector1);
        group1.AddChargeStation(chargeStation1);
        
        var group2 = GroupEntity.Create("Test Group 2");
        var chargeStation2 = ChargeStationEntity.Create("Test ChargeStation 2");
        var connector2 = ConnectorEntity.Create("Test Connector 2", 2);
        
        chargeStation2.AddConnector(connector2);
        group2.AddChargeStation(chargeStation2);
        
        InMemoryDb.Groups.AddRange(group1, group2);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new GetConnectorsQuery();
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count());    
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorsNotExists()
    {
        // Act
        var command = new GetConnectorsQuery();
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }
}