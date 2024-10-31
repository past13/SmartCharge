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

public interface IChargeStationRepository
{
    Task<bool> IsNameExist(string name);
    Task<bool> IsChargeStationExist(Guid id);
    Task AddChargeStation(ChargeStationEntity chargeStation);
    Task<ChargeStationEntity?> GetChargeStationById(Guid id);
    Task DeleteChargeStationById(Guid id);
    Task<IEnumerable<ChargeStationDto>> GetAllChargeStations();
}

public class ChargeStationRepository : IChargeStationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConnectorRepository _connectorRepository;

    public ChargeStationRepository(
        ApplicationDbContext context, 
        IMapper mapper,
        IConnectorRepository connectorRepository)
    {
        _context = context;
        _mapper = mapper;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<bool> IsNameExist(string name)
    {
        var isNameValid = await _context.ChargeStations
            .AnyAsync(g => g.Name.ToLower() == name.ToLower());
        
        return isNameValid;
    }
    
    public async Task<bool> IsChargeStationExist(Guid id)
    {
        return await _context.ChargeStations.AnyAsync(cs => cs.Id == id);
    }
    
    public async Task<IEnumerable<ChargeStationDto>> GetAllChargeStations()
    {
        var result = await _context.ChargeStations
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<ChargeStationDto>>(result);
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