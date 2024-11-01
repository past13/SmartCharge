using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.ChargeStation;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.ChargeStationTests;

public class GetChargeStationsHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;
    private readonly GetChargeStationsHandler _handler;
    
    public GetChargeStationsHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _connectorRepository = new ConnectorRepository(InMemoryDb);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _connectorRepository);
        
        _handler = new GetChargeStationsHandler(_unitOfWork, _chargeStationRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTwoGroupExists()
    {
        var group1 = GroupEntity.Create("Test Group 2");
        var chargeStation1 = ChargeStationEntity.Create("Test ChargeStation 2");
        var connector1 = ConnectorEntity.Create("Test Connector 2", 1);
        
        chargeStation1.AddConnector(connector1);
        group1.AddChargeStation(chargeStation1);
        
        var group2 = GroupEntity.Create("Test Group 2");
        var chargeStation2 = ChargeStationEntity.Create("Test ChargeStation 2");
        var connector2 = ConnectorEntity.Create("Test Connector 2", 1);
        
        chargeStation2.AddConnector(connector2);
        group2.AddChargeStation(chargeStation2);
        
        InMemoryDb.Groups.AddRange(group1, group2);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new GetChargeStationsQuery();
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count());
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNotExists()
    {
        // Act
        var command = new GetChargeStationsQuery();
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }
}