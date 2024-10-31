using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Repository;

public interface IConnectorRepository
{
    Task<bool> IsNameExist(string name);
    Task<ConnectorEntity> AddConnector(ConnectorEntity connectorEntity);
    Task<Result<ConnectorEntity>> UpdateConnector(ConnectorEntity connectorEntity);
    Task<ConnectorEntity?> GetConnectorById(Guid id);
    Task<Result<ConnectorEntity>> DeleteConnectorById(Guid id);
    Task<IEnumerable<ConnectorDto>> GetAllConnectors();
    Task<IEnumerable<ConnectorEntity>> GetAllConnectorsById(List<Guid> ids);
}

public class ConnectorRepository : IConnectorRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public ConnectorRepository(
        ApplicationDbContext context, 
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<bool> IsNameExist(string name)
    {
        var isNameValid = await _context.Connector
            .AnyAsync(g => g.Name.ToLower() == name.ToLower());
        
        return isNameValid;
    }
    
    public async Task<IEnumerable<ConnectorEntity>> GetAllConnectorsById(List<Guid> ids)
    {
        var result = await _context.Connector
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
        
        return result;
    }
    
    public async Task<IEnumerable<ConnectorDto>> GetAllConnectors()
    {
        var result = await _context.Connector
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<ConnectorDto>>(result);
    }
    
    public async Task<ConnectorEntity> AddConnector(ConnectorEntity connector)
    {
        _context.Connector.Add(connector);
        await _context.SaveChangesAsync();
        
        return connector;
    }

    public async Task<Result<ConnectorEntity>> UpdateConnector(ConnectorEntity connectorEntity)
    {
        try
        {
            await _context.SaveChangesAsync();
            
            return new Result<ConnectorEntity>
            {
                Data = connectorEntity,
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Result<ConnectorEntity>
            {
                Data = null,
                Error = "The Connector was modified by another user since you loaded it. Please reload the data and try again."
            };
        }
    }

    public async Task<ConnectorEntity?> GetConnectorById(Guid id)
    {
        return await _context.Connector.FindAsync(id);
    }
    
    public async Task<Result<ConnectorEntity>> DeleteConnectorById(Guid id)
    {
        var response = new Result<ConnectorEntity>();

        var connector = await _context.Connector.FindAsync(id);
        if (connector == null)
        {
            response.Error = $"A Connector with the Id '{id}' does not exists.";
            return response;
        }
        
        _context.Connector.Remove(connector);
        
        try
        {
            await _context.SaveChangesAsync();
            
            return new Result<ConnectorEntity>
            {
                Data = null,
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Result<ConnectorEntity>
            {
                Data = null,
                Error = "The Entity was modified by another user since you loaded it. Please reload the data and try again."
            };
        }
    }
}