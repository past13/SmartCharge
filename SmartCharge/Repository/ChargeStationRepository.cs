using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;

namespace SmartCharge.Repository;

public interface IChargeStationRepository
{
    Task<bool> IsNameExist(string name);
    Task<ChargeStationEntity> AddChargeStation(Guid groupId, ChargeStationEntity chargeStation);
    Task UpdateChargeStation(ChargeStationEntity chargeStation);
    Task<ChargeStationEntity?> GetChargeStationById(Guid id);
    Task DeleteChargeStationById(Guid id);
    Task<IEnumerable<ChargeStationDto>> GetAllChargeStations();
}

public class ChargeStationRepository : IChargeStationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public ChargeStationRepository(
        ApplicationDbContext context, 
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<bool> IsNameExist(string name)
    {
        var isNameValid = await _context.ChargeStations
            .AnyAsync(g => g.Name.ToLower() == name.ToLower());
        
        return isNameValid;
    }
    
    public async Task<IEnumerable<ChargeStationDto>> GetAllChargeStations()
    {
        var result = await _context.ChargeStations
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<ChargeStationDto>>(result);
    }
    
    public async Task<ChargeStationEntity> AddChargeStation(Guid groupId, ChargeStationEntity chargeStation)
    {
        chargeStation.GroupId = groupId;
        
        _context.ChargeStations.Add(chargeStation);
        await _context.SaveChangesAsync();
        
        return chargeStation;
    }

    public async Task UpdateChargeStation(ChargeStationEntity chargeStation)
    {
        _context.ChargeStations.Update(chargeStation);
        await _context.SaveChangesAsync();
    }

    public async Task<ChargeStationEntity?> GetChargeStationById(Guid id)
    {
        return await _context.ChargeStations.FindAsync(id);
    }
    
    public async Task DeleteChargeStationById(Guid id)
    {
        var chargeStation = await _context.ChargeStations.FindAsync(id);
        if (chargeStation == null)
        {
            return;
        }
        
        _context.ChargeStations.Remove(chargeStation);
        await _context.SaveChangesAsync();
    }
}