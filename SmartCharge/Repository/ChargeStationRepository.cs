using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;

namespace SmartCharge.Repository;

public interface IChargeStationRepository
{
    Task<bool> IsNameExist(string name, Guid? id = null);
    Task<bool> IsChargeStationExist(Guid id);
    Task AddChargeStation(ChargeStationEntity chargeStation);
    Task<ChargeStationEntity?> GetChargeStationById(Guid id);
    Task DeleteChargeStationById(Guid id);
    Task<IEnumerable<ChargeStationEntity>> GetChargeStations();
}

public class ChargeStationRepository : IChargeStationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IConnectorRepository _connectorRepository;

    public ChargeStationRepository(
        ApplicationDbContext context, 
        IConnectorRepository connectorRepository)
    {
        _context = context;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<bool> IsNameExist(string name, Guid? id)
    {
        return await _context.ChargeStations
            .AnyAsync(g => g.Name.ToLower() == name.ToLower() && (!id.HasValue || g.Id != id.Value));
    }
    
    public async Task<bool> IsChargeStationExist(Guid id)
    {
        return await _context.ChargeStations.AnyAsync(cs => cs.Id == id);
    }
    
    public async Task<IEnumerable<ChargeStationEntity>> GetChargeStations()
    {
        return await _context.ChargeStations
            .Include(cs => cs.Connectors)
            .ToListAsync();
    }
    
    public async Task AddChargeStation(ChargeStationEntity chargeStation)
    {
        _context.ChargeStations.Add(chargeStation);
    }

    public async Task<ChargeStationEntity?> GetChargeStationById(Guid id)
    {
        return await _context.ChargeStations
            .Include(cs => cs.Connectors)
            .FirstOrDefaultAsync(cs => cs.Id == id);
    }
    
    public async Task DeleteChargeStationById(Guid id)
    {
        var chargeStation = await _context.ChargeStations
            .Include(g => g.Connectors)
            .FirstOrDefaultAsync(g => g.Id == id);
        
        var connectors = chargeStation.Connectors.ToList();
        foreach (var connector in connectors)
        {
            await _connectorRepository.DeleteConnectorById(connector.Id);
        }
        
        _context.ChargeStations.Remove(chargeStation);
    }
}