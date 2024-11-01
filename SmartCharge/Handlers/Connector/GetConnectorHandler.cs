﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Connector;

public class GetConnectorHandler : IRequestHandler<GetConnectorByIdQuery, Result<ConnectorEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectorRepository _connectorRepository;
    private readonly IMapper _mapper;
    
    public GetConnectorHandler(
        IUnitOfWork unitOfWork,
        IConnectorRepository connectorRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _connectorRepository = connectorRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<ConnectorEntity>> Handle(GetConnectorByIdQuery query, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var connector = await _connectorRepository.GetConnectorById(query.Id);
            if (connector is null)
            {
                throw new ArgumentException($"A Connector with Id {query.Id} does not exist.");
            }
                
            connector.IsValidForChange();
            
            await _unitOfWork.CommitAsync();
            
            return Result<ConnectorEntity>.Success(connector);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
    }
}