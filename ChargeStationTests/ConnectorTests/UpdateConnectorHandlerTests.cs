using AutoMapper;
using Moq;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Handlers.Connector;
using SmartCharge.Repository;

namespace ChargeStationTests.ConnectorTests;

public class UpdateConnectorHandlerTests : DatabaseDependentTestBase
{
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly UpdateConnectorHandler _handler;
    
    public UpdateConnectorHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);

        _handler = new UpdateConnectorHandler(_connectorRepository);
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
        var notExist = new UpdateConnectorCommand(Guid.NewGuid(), groupEntity.Id,"Test Connector 2", 1);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A Connector does not exists.");
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
        var exist = new UpdateConnectorCommand(connectorEntity.Id, chargeStationEntity.Id,"Updated Test Group", 1);
        var result = await _handler.Handle(exist, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
}