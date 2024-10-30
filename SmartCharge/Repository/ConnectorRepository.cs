using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;

namespace SmartCharge.Repository;

public interface IConnectorRepository
{
    Task<bool> IsNameExist(string name);
    Task<IEnumerable<ConnectorEntity>> GetAllConnectorsById(List<Guid> ids);
    
    Task<ConnectorEntity> AddConnector(ConnectorEntity connectorEntity);
    Task<ConnectorEntity?> GetConnectorById(Guid id);
    Task DeleteConnectorById(Guid id);
    Task<IEnumerable<ConnectorDto>> GetAllConnectors();
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

    public async Task<ConnectorEntity?> GetConnectorById(Guid id)
    {
        return await _context.Connector.FindAsync(id);
    }
    
    public async Task DeleteConnectorById(Guid id)
    {
        var connector = await _context.Connector.FindAsync(id);
        if (connector == null)
        {
            return;
        }
        
        _context.Connector.Remove(connector);
        await _context.SaveChangesAsync();
    }
}