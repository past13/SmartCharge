using AutoMapper;
using Moq;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Handlers.ChargeStation;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;

namespace ChargeStationTests;

public class CreateChargeStationHandlerTests : DatabaseDependentTestBase
{
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly Mock<IConnectorRepository> _connectorRepository;

    private readonly CreateChargeStationHandler _handler;
    
    public CreateChargeStationHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object);
        
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        _connectorRepository = new Mock<IConnectorRepository>();
        
        _handler = new CreateChargeStationHandler(_chargeStationRepository, _groupRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNotExists()
    {
        var command = new CreateChargeStationCommand(Guid.NewGuid(), "Test ChargeStation 1", null);
        var chargeStation = ChargeStationEntity.Create(command.Name);

        InMemoryDb.ChargeStations.Add(chargeStation);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new CreateChargeStationCommand(Guid.NewGuid(), "Test ChargeStation 2", null);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A Group does not exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupExists()
    {
        var groupEntity = GroupEntity.Create("Test Group", 1);
        var command = new CreateChargeStationCommand(groupEntity.Id, "Test ChargeStation 2", []);
        
        var chargeStationEntity = ChargeStationEntity.Create(command.Name);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new CreateChargeStationCommand(groupEntity.Id, "Test ChargeStation 2", []);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Data.GroupId, groupEntity.Id);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationNameAlreadyExists()
    {
        var chargeStationRequest = new ChargeStationRequest { Name = "Test ChargeStation" };
        
        var groupCommand = new CreateGroupCommand("Test Group", 1, chargeStationRequest);
        var groupEntity = GroupEntity.Create(groupCommand.Name, groupCommand.CapacityInAmps);
        var chargeStationEntity = ChargeStationEntity.Create(chargeStationRequest.Name);
        
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();

        // Act
        var exist = new CreateChargeStationCommand(groupEntity.Id, "Test ChargeStation", null);
        var result = await _handler.Handle(exist, CancellationToken.None);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A ChargeStation with the name 'Test ChargeStation' already exists.");
    }
}