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
    Task AddConnector(ConnectorEntity connectorEntity);
    Task<ConnectorEntity?> GetConnectorById(Guid id);
    Task DeleteConnectorById(Guid id);
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
    
    public async Task AddConnector(ConnectorEntity connector)
    {
        _context.Connector.Add(connector);
    }

    public async Task<ConnectorEntity?> GetConnectorById(Guid id)
    {
        return await _context.Connector.FindAsync(id);
    }
    
    public async Task DeleteConnectorById(Guid id)
    {
        var connectors = await _context.ChargeStations
            .Include(cs => cs.Connectors)
            .SelectMany(cs => cs.Connectors)
            .ToListAsync();

        if (connectors.Count <= 1)
        {
            throw new ArgumentException($"Connector can not be deleted ChargeStation required at least one connector.");
        }

        var connector = connectors.First(c => c.Id == id);
        _context.Connector.Remove(connector);
    }
}