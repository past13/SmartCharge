using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Connector;

public class DeleteConnectorHandler : IRequestHandler<DeleteConnectorCommand, ApiResponse<ConnectorEntity>>
{
    private readonly IConnectorRepository _connectorRepository;
    public DeleteConnectorHandler(IConnectorRepository connectorRepository)
    {
        _connectorRepository = connectorRepository;
    }
    
    public async Task<ApiResponse<ConnectorEntity>> Handle(DeleteConnectorCommand request, CancellationToken cancellationToken)
    {
        var response = new ApiResponse<ConnectorEntity>();
        
        var connector = await _connectorRepository.GetConnectorById(request.Id);
        if (connector == null)
        {
            response.Error = $"Connector with Id {request.Id} does not exists.";
            return response;
        }
        
        var result = await _connectorRepository.DeleteConnectorById(request.Id);
        return result;
    }
}