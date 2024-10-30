using AutoMapper;
using Moq;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;

namespace ChargeStationTests;

public class CreateGroupHandlerTests : DatabaseDependentTestBase
{
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly Mock<IConnectorRepository> _connectorRepository;

    private readonly CreateGroupHandler _handler;
    
    public CreateGroupHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object);
        
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        _connectorRepository = new Mock<IConnectorRepository>();
        
        _handler = new CreateGroupHandler(_groupRepository, _chargeStationRepository, _connectorRepository.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNameExists()
    {
        var command = new CreateGroupCommand("Test Group", 1, null);
        var group = GroupEntity.Create(command.Name, command.CapacityInAmps);

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new CreateGroupCommand("Test Group", 1, null);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A Group with the name 'Test Group' already exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupNameNotExists()
    {
        var command = new CreateGroupCommand("Test Group", 1, null);
        var group = GroupEntity.Create(command.Name, command.CapacityInAmps);

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new CreateGroupCommand("Test Group 1", 1, null);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenNewChargeStationNotExists()
    {
        var chargeStation1 = new ChargeStationRequest
        {
            Name = "Test ChargeStation 1",
        };
        
        var command = new CreateGroupCommand("Test Group", 1, chargeStation1);
        var group = GroupEntity.Create(command.Name, command.CapacityInAmps);

        var chargeStationEntity = ChargeStationEntity.Create(chargeStation1.Name);
        group.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        var chargeStation2 = new ChargeStationRequest
        {
            Name = "Test ChargeStation 2",
        };
        
        // Act
        var notExist = new CreateGroupCommand("Test Group 1", 1, chargeStation2);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Data.ChargeStations.Count);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationAlreadyExists()
    {
        var chargeStation = new ChargeStationRequest
        {
            Name = "Test ChargeStation",
        };
        
        var command = new CreateGroupCommand("Test Group", 1, chargeStation);
        var group = GroupEntity.Create(command.Name, command.CapacityInAmps);

        var chargeStationEntity = ChargeStationEntity.Create(chargeStation.Name);
        group.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new CreateGroupCommand("Test Group 1", 1, chargeStation);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A ChargeStation with the name 'Test ChargeStation' already exists.");
    }
}