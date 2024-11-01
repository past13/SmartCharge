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

public class GetConnectorsHandler : IRequestHandler<GetConnectorsQuery, Result<IEnumerable<ConnectorEntity>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectorRepository _connectorRepository;
    
    public GetConnectorsHandler(
        IUnitOfWork unitOfWork,
        IConnectorRepository connectorRepository)
    {
        _unitOfWork = unitOfWork;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<Result<IEnumerable<ConnectorEntity>>> Handle(GetConnectorsQuery query, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var connectors = await _connectorRepository.GetConnectors();
            
            await _unitOfWork.CommitAsync();
            
            return Result<IEnumerable<ConnectorEntity>>.Success(connectors);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<IEnumerable<ConnectorEntity>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<IEnumerable<ConnectorEntity>>.Failure(ex.Message);
        }
    }
}