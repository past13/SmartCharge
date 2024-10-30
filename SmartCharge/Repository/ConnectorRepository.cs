using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;
using SmartCharge.Domain.Entities;
using SmartCharge.DTOs;

namespace SmartCharge.Repository;

public interface IConnectorRepository
{
    Task<ConnectorEntity> AddConnector(ConnectorEntity connectorEntity);
    Task<ConnectorEntity?> GetConnectorById(Guid id);
    Task DeleteConnectorById(Guid id);
    Task<IEnumerable<ConnectorDTO>> GetAllConnectors();
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
    
    public async Task<IEnumerable<ConnectorDTO>> GetAllConnectors()
    {
        var result = await _context.Connector
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<ConnectorDTO>>(result);
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