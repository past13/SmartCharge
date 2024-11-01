using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;
using SmartCharge.Domain.Entities;

namespace SmartCharge.Repository;

public interface IChargeStationRepository
{
    Task<bool> IsNameExist(string name, Guid? id = null);
    Task<bool> IsChargeStationExist(Guid id);
    Task AddChargeStation(ChargeStationEntity chargeStation);
    Task<ChargeStationEntity> GetChargeStationById(Guid id);
    Task DeleteChargeStationById(Guid id);
    Task<IEnumerable<ChargeStationEntity>> GetChargeStations();
}

public class ChargeStationRepository(
    ApplicationDbContext context,
    IConnectorRepository connectorRepository)
    : IChargeStationRepository
{
    public async Task<bool> IsNameExist(string name, Guid? id)
    {
        return await context.ChargeStations
            .AnyAsync(g => g.Name.ToLower() == name.ToLower() && (!id.HasValue || g.Id != id.Value));
    }
    
    public async Task<bool> IsChargeStationExist(Guid id)
    {
        return await context.ChargeStations.AnyAsync(cs => cs.Id == id);
    }
    
    public async Task<IEnumerable<ChargeStationEntity>> GetChargeStations()
    {
        return await context.ChargeStations
            .Include(cs => cs.Connectors)
            .Where(cs => cs.RowState == RowState.Active)
            .ToListAsync();
    }
    
    public async Task AddChargeStation(ChargeStationEntity chargeStation)
    {
        context.ChargeStations.Add(chargeStation);
    }

    public async Task<ChargeStationEntity> GetChargeStationById(Guid id)
    {
        return await context.ChargeStations
            .Include(cs => cs.Connectors)
            .FirstOrDefaultAsync(cs => cs.Id == id);
    }
    
    public async Task DeleteChargeStationById(Guid id)
    {
        var chargeStation = await context.ChargeStations
            .Include(g => g.Connectors)
            .FirstOrDefaultAsync(g => g.Id == id);
        
        var connectors = chargeStation.Connectors.ToList();
        foreach (var connector in connectors)
        {
            await connectorRepository.DeleteConnectorById(connector.Id);
        }
        
        context.ChargeStations.Remove(chargeStation);
    }
}