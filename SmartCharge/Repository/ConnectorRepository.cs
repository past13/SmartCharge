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
    Task<bool> IsNameExist(string name, Guid? id = null);
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
    
    public async Task<bool> IsNameExist(string name, Guid? id)
    {
        return await _context.Connectors
            .AnyAsync(g => g.Name.ToLower() == name.ToLower() && (!id.HasValue || g.Id != id.Value));
    }
    
    public async Task<IEnumerable<ConnectorEntity>> GetAllConnectorsById(List<Guid> ids)
    {
        var result = await _context.Connectors
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
        
        return result;
    }
    
    public async Task<IEnumerable<ConnectorDto>> GetAllConnectors()
    {
        var result = await _context.Connectors
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<ConnectorDto>>(result);
    }
    
    public async Task AddConnector(ConnectorEntity connector)
    {
        _context.Connectors.Add(connector);
    }

    public async Task<ConnectorEntity?> GetConnectorById(Guid id)
    {
        return await _context.Connectors.FindAsync(id);
    }
    
    public async Task DeleteConnectorById(Guid id)
    {
        var connectors = await _context.ChargeStations
            .Include(cs => cs.Connectors)
            .SelectMany(cs => cs.Connectors)
            .ToListAsync();
       
        var connector = connectors.First(c => c.Id == id);
        _context.Connectors.Remove(connector);
    }
}