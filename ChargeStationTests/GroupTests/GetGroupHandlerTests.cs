using AutoMapper;
using Moq;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace ChargeStationTests.GroupTests;

public class GetGroupHandlerTests : DatabaseDependentTestBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IMapper> _mapper;
    private readonly GroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;
    private readonly GetGroupHandler _handler;
    
    public GetGroupHandlerTests()
    {
        _unitOfWork = new UnitOfWork(InMemoryDb);
        _mapper = new Mock<IMapper>();
        _connectorRepository = new ConnectorRepository(InMemoryDb, _mapper.Object);
        _chargeStationRepository = new ChargeStationRepository(InMemoryDb, _mapper.Object, _connectorRepository);
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository);
        
        _handler = new GetGroupHandler(_unitOfWork, _groupRepository, _mapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenGroupExists()
    {
        var group = GroupEntity.Create("Test Group 1");

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new GetGroupByIdQuery(group.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNotExists()
    {
        var group = GroupEntity.Create("Test Group 1");

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExistId = Guid.NewGuid();
        
        var command = new GetGroupByIdQuery(notExistId);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A Group with Id {notExistId} does not exist.", result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupRowSateDeleting()
    {
        var group = GroupEntity.Create("Test Group 1");

        group.UpdateRowState(RowState.PendingDelete);
        
        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var command = new GetGroupByIdQuery(group.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains($"A Group with Id {group.Id} already deleting.", result.Error);
    }
}