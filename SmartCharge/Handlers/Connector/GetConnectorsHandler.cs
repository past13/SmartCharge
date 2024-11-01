using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Connector;

public class GetConnectorsHandler(
    IUnitOfWork unitOfWork,
    IConnectorRepository connectorRepository)
    : IRequestHandler<GetConnectorsQuery, Result<IEnumerable<ConnectorEntity>>>
{
    public async Task<Result<IEnumerable<ConnectorEntity>>> Handle(GetConnectorsQuery query, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var connectors = await connectorRepository.GetConnectors();
            
            await unitOfWork.CommitAsync();
            
            return Result<IEnumerable<ConnectorEntity>>.Success(connectors);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<IEnumerable<ConnectorEntity>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<IEnumerable<ConnectorEntity>>.Failure(ex.Message);
        }
    }
}