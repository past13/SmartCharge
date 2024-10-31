using AutoMapper;
using Moq;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;

namespace ChargeStationTests.GroupTests;

public class UpdateGroupHandlerTests : DatabaseDependentTestBase
{
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly UpdateGroupHandler _handler;
    
    public UpdateGroupHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);

        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new UpdateGroupHandler(_groupRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNotExists()
    {
        var command = new UpdateGroupCommand(Guid.NewGuid(), "Test Group 1", 1, []);
        var group = GroupEntity.Create(command.Name);

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new UpdateGroupCommand(Guid.NewGuid(), "Test Group 2", 1, []);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A Group does not exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupExists()
    {
        var group = GroupEntity.Create("Test Group 1");

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();

        var expectedName = "Test Group 2";
        var capacityInAmps = 2;

        var chargeStationRequest = new ChargeStationRequest
        {
            Name = "Test ChargeStation 2",
            Connectors =
            [
                new ConnectorRequest
                {
                    Name = "Test Connector 1",
                    MaxCapacityInAmps = 1
                }
            ]
        };
        
        // Act
        var notExist = new UpdateGroupCommand(group.Id, expectedName, capacityInAmps, [chargeStationRequest]);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, InMemoryDb.Groups.First().Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenNewChargeStationNotExists()
    {
        var chargeStation1 = new ChargeStationRequest
        {
            Name = "Test ChargeStation 1",
        };
        
        var group = GroupEntity.Create("Test Group");
    
        var chargeStationEntity = ChargeStationEntity.Create(chargeStation1.Name);
        group.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        var chargeStation2 = new ChargeStationRequest
        {
            Name = "Test ChargeStation 2",
        };
        
        // Act
        var notExist = new UpdateGroupCommand(group.Id, "Test Group 1", 1, [chargeStation2]);
        var result = await _handler.Handle(notExist, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
 
    // [Fact]
    // public async Task Handle_ShouldReturnError_WhenChargeStationAlreadyExists()
    // {
    //     var chargeStation = new ChargeStationRequest
    //     {
    //         Name = "Test ChargeStation",
    //     };
    //     
    //     var command = new CreateGroupCommand("Test Group", 1, chargeStation);
    //     var group = GroupEntity.Create(command.Name, command.CapacityInAmps);
    //
    //     var chargeStationEntity = ChargeStationEntity.Create(chargeStation.Name);
    //     group.AddChargeStation(chargeStationEntity);
    //     
    //     InMemoryDb.Groups.Add(group);
    //     await InMemoryDb.SaveChangesAsync();
    //     
    //     // Act
    //     var notExist = new CreateGroupCommand("Test Group 1", 1, chargeStation);
    //     var result = await _handler.Handle(notExist, CancellationToken.None);
    //
    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Contains(result.Error, "A ChargeStation with the name 'Test ChargeStation' already exists.");
    // }
}