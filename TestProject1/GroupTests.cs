using AutoMapper;
using Moq;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;

namespace TestProject1;

public class CreateGroupHandlerTests : DatabaseDependentTestBase
{
    private readonly GroupRepository _groupRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IChargeStationRepository> _chargeStationRepository;
    private readonly Mock<IConnectorRepository> _connectorRepository;

    private readonly CreateGroupHandler _handler;
    
    public CreateGroupHandlerTests()
    {
        _mapper = new Mock<IMapper>();
        _chargeStationRepository = new Mock<IChargeStationRepository>();
        
        _groupRepository = new GroupRepository(InMemoryDb, _mapper.Object, _chargeStationRepository.Object);
        _connectorRepository = new Mock<IConnectorRepository>();
        
        _handler = new CreateGroupHandler(_groupRepository, _chargeStationRepository.Object, _connectorRepository.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenGroupNameExists()
    {
        var chargeStation = new ChargeStationRequest();
        var command = new CreateGroupCommand("Test Group", 1, chargeStation);
        var group = GroupEntity.Create(command.Name, command.CapacityInAmps);

        InMemoryDb.Groups.Add(group);
        await InMemoryDb.SaveChangesAsync();
        
        // Act
        var notExist = new CreateGroupCommand("Test Group 1", 1, chargeStation);
        var result = await _handler.Handle(notExist, CancellationToken.None);

        // Assert
        
    }
}