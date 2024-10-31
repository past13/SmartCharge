using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Connector;

public class UpdateConnectorHandler : IRequestHandler<UpdateConnectorCommand, Result<ConnectorEntity>>
{
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    public UpdateConnectorHandler(IConnectorRepository connectorRepository)
    {
        _connectorRepository = connectorRepository;
    }
    
    public async Task<Result<ConnectorEntity>> Handle(UpdateConnectorCommand command, CancellationToken cancellationToken)
    {
        var response = new Result<ConnectorEntity>();

        var connectorName = command.Name.Trim();
        var connectorNameExist = await _connectorRepository.IsNameExist(connectorName);
        if (connectorNameExist)
        {
            response.Error = $"A Connector with the name '{connectorName}' already exists.";
            return response; 
        }
        
        var connector = await _connectorRepository.GetConnectorById(command.Id);
        if (connector == null)
        {
            response.Error = $"A Connector does not exists.";
            return response; 
        }
        
        connector.Update(connectorName, command.MaxCurrentInAmps);

        //Todo: update many
        var result = await _connectorRepository.UpdateConnector(connector);
        return result;
    }
}