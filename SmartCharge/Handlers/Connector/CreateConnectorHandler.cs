using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands;
using SmartCharge.Domain.Entities;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Connector;

public class CreateConnectorHandler : IRequestHandler<CreateConnectorCommand, ConnectorEntity>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    public CreateConnectorHandler(
        IChargeStationRepository chargeStationRepository,
        IGroupRepository groupRepository,
        IConnectorRepository connectorRepository)
    {
        _chargeStationRepository = chargeStationRepository;
        _groupRepository = groupRepository;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<ConnectorEntity> Handle(CreateConnectorCommand command, CancellationToken cancellationToken)
    {
        var chargeStation = await _chargeStationRepository.GetChargeStationById(command.ChargeStationId);
        if (chargeStation == null)
        {
            //Todo: if group dont exist cant create station
            return null;
        }
        
        //Todo: validate to add connectors
        //Todo: validate if chargeStation not exist same name
        
        var connector = ConnectorEntity.Create(command.Name, command.CapacityInAmps);
        chargeStation.AddConnector(connector);

        await _connectorRepository.AddConnector(connector);
        
        return connector;
    }
}