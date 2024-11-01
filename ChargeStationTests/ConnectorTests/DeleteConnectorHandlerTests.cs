using AutoMapper;
using Moq;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Connector;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.ConnectorTests;

public class DeleteConnectorHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IMapper> _mapper;
    private readonly IGroupRepository _groupRepository;
    
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly DeleteConnectorHandler _handler;
    
    public DeleteConnectorHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new DeleteConnectorHandler(_unitOfWork, _groupRepository, _chargeStationRepository, _connectorRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeChargeStationNotExists()
    {
        var groupEntity = GroupEntity.Create("Test Group 1");
        
        var connectorEntity = ConnectorEntity.Create("Test Connector", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        
        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new DeleteConnectorCommand(chargeStationEntity.Id, Guid.NewGuid());
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"Connector with Id {notExist.Id} does not exists.", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenChargeChargeStationHaveOnlyOneConnector()
    {
        var groupEntity = GroupEntity.Create("Test Group 1");
        
        var connectorEntity = ConnectorEntity.Create("Test Connector", 1);
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        
        chargeStationEntity.AddConnector(connectorEntity);
        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new DeleteConnectorCommand(chargeStationEntity.Id, connectorEntity.Id);
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A ChargeStation Id {chargeStationEntity.Id} cannot have less than 1 connector.", result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenChargeChargeStationExists()
    {
        var groupEntity = GroupEntity.Create("Test Group 1");
        
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");

        var connectorEntity1 = ConnectorEntity.Create("Test Connector 1", 1);
        var connectorEntity2 = ConnectorEntity.Create("Test Connector 2", 1);
        
        chargeStationEntity.AddConnector(connectorEntity1);
        chargeStationEntity.AddConnector(connectorEntity2);

        groupEntity.AddChargeStation(chargeStationEntity);
        
        InMemoryDb.Groups.Add(groupEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new DeleteConnectorCommand(chargeStationEntity.Id, connectorEntity1.Id);
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorRemovedShouldReorderNumber()
    {
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");

        var connectorEntity1 = ConnectorEntity.Create("Test Connector 1", 1);
        var connectorEntity2 = ConnectorEntity.Create("Test Connector 2", 1);
        
        chargeStationEntity.AddConnector(connectorEntity1);
        chargeStationEntity.AddConnector(connectorEntity2);
        
        chargeStationEntity.RemoveConnector(connectorEntity1);

        Assert.Equal(2, connectorEntity2.ConnectorNumber);
    }
}