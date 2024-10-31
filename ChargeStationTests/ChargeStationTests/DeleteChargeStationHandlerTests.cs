using AutoMapper;
using Moq;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.ChargeStation;
using SmartCharge.Repository;

namespace ChargeStationTests.ChargeStationTests;

public class DeleteChargeStationHandlerTests : DatabaseDependentTestBase
{
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly DeleteChargeStationHandler _handler;
    
    public DeleteChargeStationHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        
        _handler = new DeleteChargeStationHandler(_chargeStationRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeChargeStationNotExists()
    {
        var groupEntity = GroupEntity.Create("Test Group", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");

        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new DeleteChargeStationCommand(Guid.NewGuid());
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A ChargeStation with the Id {notExist.Id} does not exists.", result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSucces_WhenChargeChargeStationExists()
    {
        var groupEntity = GroupEntity.Create("Test Group", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");

        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var exist = new DeleteChargeStationCommand(chargeStationEntity.Id);
        var result = await _handler.Handle(exist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSucces_WhenChargeChargeStationExists_WithConnectors()
    {
        var groupEntity = GroupEntity.Create("Test Group", 1);
        for (var i = 0; i < 2; i++)
        {
            var connectorEntity = ConnectorEntity.Create("Test Connector" + i, 1);
            var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation" + i);
            
            chargeStationEntity.AddConnector(connectorEntity);
            groupEntity.AddChargeStation(chargeStationEntity);
        }
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();

        var existFirstChargeStationId = groupEntity.ChargeStations.First().Id; 
        
        // Act
        var exist = new DeleteChargeStationCommand(existFirstChargeStationId);
        var result = await _handler.Handle(exist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, InMemoryDb.ChargeStations.Count());
        Assert.Equal(1, InMemoryDb.Connector.Count());
    }
}