using AutoMapper;
using Moq;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests.Connector;
using SmartCharge.Handlers.ChargeStation;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.ChargeStationTests;

public class CreateChargeStationHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IMapper> _mapper;
    private readonly GroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly CreateChargeStationHandler _handler;
    
    public CreateChargeStationHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new CreateChargeStationHandler(_unitOfWork, _mapper.Object, _chargeStationRepository, _groupRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNotExists()
    {
        var chargeStation = ChargeStationEntity.Create("Test ChargeStation 1");

        InMemoryDb.ChargeStations.Add(chargeStation);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new CreateChargeStationCommand(Guid.NewGuid(), "Test ChargeStation 2", null);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A Group with Id {notExist.GroupId} does not exists.", result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupExists()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");
        
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();

        var chargeStation = new ConnectorRequest
        {
            Name = "Test Connector",
            MaxCapacityInAmps = 1
        };
        
        // Act
        var notExist = new CreateChargeStationCommand(groupEntity.Id, "Test ChargeStation 2", [chargeStation]);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationNameAlreadyExists()
    {
        var expectedName = "Test ChargeStation";
        
        var groupEntity = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();

        // Act
        var exist = new CreateChargeStationCommand(groupEntity.Id, expectedName, null);
        var result = await _handler.Handle(exist, CancellationToken.None);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A ChargeStation with the name {expectedName} already exists.", result.Error);
    }
}