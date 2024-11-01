using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Connector;

public class GetConnectorHandler(
    IUnitOfWork unitOfWork,
    IConnectorRepository connectorRepository)
    : IRequestHandler<GetConnectorByIdQuery, Result<ConnectorEntity>>
{
    public async Task<Result<ConnectorEntity>> Handle(GetConnectorByIdQuery query, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var connector = await connectorRepository.GetConnectorById(query.Id);
            if (connector is null)
            {
                throw new ArgumentException($"A Connector with Id {query.Id} does not exist.");
            }
                
            connector.IsValidForChange();
            
            await unitOfWork.CommitAsync();
            
            return Result<ConnectorEntity>.Success(connector);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
    }
}