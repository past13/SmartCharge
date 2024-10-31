using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Handlers.ChargeStation;
using SmartCharge.Repository;

namespace ChargeStationTests.ChargeStationTests;

public class UpdateChargeStationHandlerTests : DatabaseDependentTestBase
{
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly UpdateChargeStationHandler _handler;
    
    public UpdateChargeStationHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);

        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new UpdateChargeStationHandler(_groupRepository, _chargeStationRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationNotExists()
    {
        var groupEntity = GroupEntity.Create("Test Group", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");
        
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new UpdateChargeStationCommand(Guid.NewGuid(), groupEntity.Id,"Test Group 1", []);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A ChargeStation does not exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSucces_WhenChargeStationExists()
    {
        var groupEntity = GroupEntity.Create("Test Group", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");
        
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        var expectedName = "Updated Test ChargeStation";
        
        // Act
        var exist = new UpdateChargeStationCommand(chargeStationEntity.Id, groupEntity.Id, expectedName, []);
        var result = await _handler.Handle(exist, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, InMemoryDb.Groups.Include(x => x.ChargeStations).First().ChargeStations.First().Name);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenNewConnectorNotExistsEmptyConnectors()
    {
        var groupEntity = GroupEntity.Create("Test Group", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        var connectorEntity = ConnectorEntity.Create("Test Connector", 1);

        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();

        var expectedName = "Updated Test ChargeStation";
        
        // Act
        var notExist = new UpdateChargeStationCommand(chargeStationEntity.Id, groupEntity.Id, expectedName, []);
        var result = await _handler.Handle(notExist, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
}