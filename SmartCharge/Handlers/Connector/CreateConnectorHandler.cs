using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Connector;

public class CreateConnectorHandler : IRequestHandler<CreateConnectorCommand, ApiResponse<ConnectorDto>>
{
    private readonly IMapper _mapper;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    public CreateConnectorHandler(
        IMapper mapper,
        IChargeStationRepository chargeStationRepository,
        IGroupRepository groupRepository,
        IConnectorRepository connectorRepository)
    {
        _mapper = mapper;
        _chargeStationRepository = chargeStationRepository;
        _groupRepository = groupRepository;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<ApiResponse<ConnectorDto>> Handle(CreateConnectorCommand command, CancellationToken cancellationToken)
    {
        var response = new ApiResponse<ConnectorDto>();

        var chargeStation = await _chargeStationRepository.GetChargeStationById(command.ChargeStationId);
        if (chargeStation == null)
        {
            response.Error = $"A ChargeStation does not exists.";
            return response;
        }

        var connectorName = command.Name.Trim();
        var connectorNameExist = await _connectorRepository.IsNameExist(connectorName);
        if (connectorNameExist)
        {
            response.Error = $"A Connector with the name {connectorName} already exists.";
            return response; 
        }
        
        var connector = ConnectorEntity.Create(connectorName, command.CapacityInAmps);
        chargeStation.AddConnector(connector);
        
        await _chargeStationRepository.UpdateChargeStation(chargeStation);
        
        response.Data = _mapper.Map<ConnectorDto>(connector);
        
        return response;
    }
}