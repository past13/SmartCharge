using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.GroupTests;

public class GetGroupsHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;
    private readonly GetGroupsHandler _handler;
    
    public GetGroupsHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _connectorRepository = new ConnectorRepository(InMemoryDb);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _chargeStationRepository);
        
        _handler = new GetGroupsHandler(_unitOfWork, _groupRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupsExists()
    {
        var group1 = GroupEntity.Create("Test Group 1");
        var chargeStation1 = ChargeStationEntity.Create("Test ChargeStation 1");
        var connector1 = ConnectorEntity.Create("Test Connector 1", 1);

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
        var command = new GetGroupsQuery();
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}