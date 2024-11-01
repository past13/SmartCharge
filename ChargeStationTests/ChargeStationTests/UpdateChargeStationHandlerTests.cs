using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.ChargeStation;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.ChargeStationTests;

public class UpdateChargeStationHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly UpdateChargeStationHandler _handler;
    
    public UpdateChargeStationHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);

        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new UpdateChargeStationHandler(_unitOfWork, _mapper.Object, _groupRepository, _chargeStationRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationNotExists()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");
        
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        var chargeStationId = Guid.NewGuid();
        
        // Act
        var command = new UpdateChargeStationCommand(chargeStationId, groupEntity.Id,"Test Group 1");
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, $"A ChargeStation with Id {chargeStationId} does not exists..");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenChargeStationExists()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");
        var connectorEntity = ConnectorEntity.Create("Test Connector 1", 1);

        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        var expectedName = "Updated Test ChargeStation";
        
        // Act
        var command = new UpdateChargeStationCommand(chargeStationEntity.Id, groupEntity.Id, expectedName);
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, InMemoryDb.Groups.Include(x => x.ChargeStations).First().ChargeStations.First().Name);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_UpdateExistChargeStationName()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        var connectorEntity = ConnectorEntity.Create("Test Connector", 1);

        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();

        var expectedName = "Updated Test ChargeStation";
        
        // Act
        var command = new UpdateChargeStationCommand(chargeStationEntity.Id, groupEntity.Id, expectedName);
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationRowSateDeleting()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        var connectorEntity = ConnectorEntity.Create("Test Connector", 1);

        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);

        chargeStationEntity.UpdateRowState(RowState.PendingDelete);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();

        var expectedName = "Updated Test ChargeStation";
        
        // Act
        var command = new UpdateChargeStationCommand(chargeStationEntity.Id, groupEntity.Id, expectedName);
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A ChargeStation with Id {chargeStationEntity.Id} already deleting.", result.Error);
    }
}