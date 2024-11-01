using AutoMapper;
using Moq;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.GroupTests;

public class DeleteGroupHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IMapper> _mapper;
    private readonly GroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly DeleteGroupHandler _handler;
    
    public DeleteGroupHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _mapper = new Mock<IMapper>();
        
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new DeleteGroupHandler(_unitOfWork, _groupRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNotExists()
    {
        var groupEntity = GroupEntity.Create("Test Group");

        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new DeleteGroupCommand(Guid.NewGuid());
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A Group with the Id {command.Id} does not exists.", result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupExists_WithChargeStation_WithConnector()
    {
        var groupEntity = GroupEntity.Create("Test Group");
        var connectorEntity1 = ConnectorEntity.Create("Test Connector 1", 1);
        var connectorEntity2 = ConnectorEntity.Create("Test Connector 2", 1);

        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        
        chargeStationEntity.AddConnector(connectorEntity1);
        chargeStationEntity.AddConnector(connectorEntity2);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new DeleteGroupCommand(groupEntity.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupExists_WithMultiple_ChargeStation_Connector()
    {
        var groupEntity1 = GroupEntity.Create("Test Group 1");
        for (var i = 0; i < 5; i++)
        {
            var connectorEntity = ConnectorEntity.Create("Test Connector" + i, 1);
            var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation" + i);
            
            chargeStationEntity.AddConnector(connectorEntity);
            groupEntity1.AddChargeStation(chargeStationEntity);
        }
        
        var groupEntity2 = GroupEntity.Create("Test Group 2");
        for (var i = 0; i < 5; i++)
        {
            var connectorEntity = ConnectorEntity.Create("Test Connector" + i, 1);
            var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation" + i);
            
            chargeStationEntity.AddConnector(connectorEntity);
            groupEntity2.AddChargeStation(chargeStationEntity);
        }
        
        InMemoryDb.Groups.AddRange(groupEntity1, groupEntity2);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new DeleteGroupCommand(groupEntity1.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(1, InMemoryDb.Groups.Count());
        Assert.Equal(groupEntity2.Id, InMemoryDb.Groups.First(x => x.Id == groupEntity2.Id).Id);
        
        Assert.Equal(5, groupEntity2.ChargeStations.Count);
        Assert.Equal(5, groupEntity2.ChargeStations.SelectMany(cs => cs.Connectors).ToList().Count);
        
        foreach (var chargeStation in groupEntity2.ChargeStations)
        {
            Assert.Single(chargeStation.Connectors);
        }
    }
}