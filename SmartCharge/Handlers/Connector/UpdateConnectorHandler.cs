using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands;
using SmartCharge.Domain.Entities;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Connector;

public class UpdateConnectorHandler : IRequestHandler<UpdateConnectorCommand, ConnectorEntity>
{
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    public UpdateConnectorHandler(IConnectorRepository connectorRepository)
    {
        _connectorRepository = connectorRepository;
    }
    
    public Task<ConnectorEntity> Handle(UpdateConnectorCommand request, CancellationToken cancellationToken)
    {
        
        
        throw new System.NotImplementedException();
    }
}