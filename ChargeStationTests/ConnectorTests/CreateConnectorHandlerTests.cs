using AutoMapper;
using Moq;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Connector;
using SmartCharge.Repository;

namespace ChargeStationTests.ConnectorTests;

public class CreateConnectorHandlerTests : DatabaseDependentTestBase
{
    private readonly IGroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    private readonly CreateConnectorHandler _handler;
    
    public CreateConnectorHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new CreateConnectorHandler(_mapper.Object, _chargeStationRepository,  _groupRepository, _connectorRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenConnectorNameExists()
    {
        var nameExist = "Test Connector";
        
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        var connector = ConnectorEntity.Create(nameExist, 1);

        chargeStationEntity.AddConnector(connector);
        
        InMemoryDb.ChargeStations.Add(chargeStationEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var exist = new CreateConnectorCommand(nameExist, 1, chargeStationEntity.Id);
        var result = await _handler.Handle(exist, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Error, $"A Connector with the name {nameExist} already exists.");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenConnectorNameNotExists()
    {
        var chargeStationEntity = ChargeStationEntity.Create("Test ChargeStation");
        var connector = ConnectorEntity.Create("Test Connector", 1);
    
        chargeStationEntity.AddConnector(connector);
        
        InMemoryDb.ChargeStations.Add(chargeStationEntity);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new CreateConnectorCommand("Test ChargeStation 1", 1, chargeStationEntity.Id);
        var result = await _handler.Handle(notExist, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
}