using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands;
using SmartCharge.Commands.Connector;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Connector;

public class DeleteConnectorHandler : IRequestHandler<DeleteConnectorCommand>, IRequest<Unit>
{
    private readonly IConnectorRepository _connectorRepository;
    public DeleteConnectorHandler(IConnectorRepository connectorRepository)
    {
        _connectorRepository = connectorRepository;
    }
    
    public async Task<Unit> Handle(DeleteConnectorCommand request, CancellationToken cancellationToken)
    {
        var connector = await _connectorRepository.GetConnectorById(request.Id);
        if (connector == null)
        {
            throw new InvalidOperationException($"Connector with ID {request.Id} not found.");
        }
        
        await _connectorRepository.DeleteConnectorById(request.Id);
        
        return Unit.Value;
    }
}