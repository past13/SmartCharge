using AutoMapper;
using Moq;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Domain.Requests.ChargeStation;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.GroupTests;

public class CreateGroupHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;
    private readonly CreateGroupHandler _handler;
    
    public CreateGroupHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _unitOfWork = new UnitOfWork(InMemoryDb);
        
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new CreateGroupHandler(_unitOfWork, _mapper.Object, _groupRepository, _chargeStationRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNameExists()
    {
        var group = GroupEntity.Create("Test Group");

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        var chargeStationRequest = new GetChargeStationRequest { Name = "Test ChargeStation" };
        
        // Act
        var notExist = new CreateGroupCommand("Test Group", chargeStationRequest);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A Group with the name 'Test Group' already exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupNameNotExists()
    {
        var group = GroupEntity.Create("Test Group");

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        var chargeStationRequest = new GetChargeStationRequest
        {
            Name = "Test ChargeStation 1",
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
        var notExist = new CreateGroupCommand("Test Group 1", chargeStationRequest);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenNewChargeStationNotExists()
    {
        var group = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation 1");

        var connector = ConnectorEntity.Create("Test Connector 1", 1);
        
        chargeStationEntity.AddConnector(connector);
        group.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();

        var chargeStationRequest = new GetChargeStationRequest
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
        var notExist = new CreateGroupCommand("Test Group 1", chargeStationRequest);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, InMemoryDb.Groups.Count());
        Assert.Equal(2, InMemoryDb.ChargeStations.Count());
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationAlreadyExists()
    {
        var existName = "Test ChargeStation";
        
        var group = GroupEntity.Create("Test Group");
        var chargeStationEntity = ChargeStationEntity.Create(existName);
        
        group.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        var chargeStation = new GetChargeStationRequest { Name = existName };

        // Act
        var notExist = new CreateGroupCommand("Test Group 1", chargeStation);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, $"A ChargeStation with the name {chargeStation.Name} already exists.");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationHasMoreThenFive()
    {
        var connectors = new List<ConnectorRequest>
        {
            new()
            {
                Name = "Test Connector 1",
                MaxCapacityInAmps = 10
            },
            new()
            {
                Name = "Test Connector 1",
                MaxCapacityInAmps = 20
            },
            new()
            {
                Name = "Test Connector 3",
                MaxCapacityInAmps = 30
            },
            new()
            {
                Name = "Test Connector 4",
                MaxCapacityInAmps = 40
            },
            new()
            {
                Name = "Test Connector 5",
                MaxCapacityInAmps = 50
            },
            new()
            {
                Name = "Test Connector 6",
                MaxCapacityInAmps = 60
            }
        };
        
        var chargeStation = new GetChargeStationRequest
        {
            Name = "Test ChargeStation",
            Connectors = connectors
        };
        
        // Act
        var notExist = new CreateGroupCommand("Test Group 1", chargeStation);
        var result = await _handler.Handle(notExist, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, "A charge station cannot have more than 5 connectors.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenChargeStationHasFive()
    {
        var connectors = new List<ConnectorRequest>
        {
            new()
            {
                Name = "Test Connector 1",
                MaxCapacityInAmps = 10
            },
            new()
            {
                Name = "Test Connector 1",
                MaxCapacityInAmps = 20
            },
            new()
            {
                Name = "Test Connector 3",
                MaxCapacityInAmps = 30
            },
            new()
            {
                Name = "Test Connector 4",
                MaxCapacityInAmps = 40
            },
            new()
            {
                Name = "Test Connector 5",
                MaxCapacityInAmps = 50
            }
        };
        
        var chargeStation = new GetChargeStationRequest
        {
            Name = "Test ChargeStation",
            Connectors = connectors
        };
        
        // Act
        var notExist = new CreateGroupCommand("Test Group 1", chargeStation);
        var result = await _handler.Handle(notExist, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Data.CapacityInAmps, connectors.Sum(c => c.MaxCapacityInAmps));
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeStationWithoutConnectors()
    {
        var connectors = new List<ConnectorRequest>();
        var chargeStation = new GetChargeStationRequest
        {
            Name = "Test ChargeStation",
            Connectors = connectors
        };
        
        // Act
        var notExist = new CreateGroupCommand("Test Group 1", chargeStation);
        var result = await _handler.Handle(notExist, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, $"A ChargeStation name {chargeStation.Name} do not have connector.");
    }
}