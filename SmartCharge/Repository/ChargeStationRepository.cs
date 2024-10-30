using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;
using SmartCharge.Domain.Entities;
using SmartCharge.DTOs;

namespace SmartCharge.Repository;

public interface IChargeStationRepository
{
    Task<ChargeStationEntity> AddChargeStation(ChargeStationEntity chargeStation);
    Task<ChargeStationEntity?> GetChargeStationById(Guid id);
    Task DeleteChargeStationById(Guid id);
    Task<IEnumerable<ChargeStationDTO>> GetAllChargeStations();
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
    
    public async Task<IEnumerable<ChargeStationDTO>> GetAllChargeStations()
    {
        var result = await _context.ChargeStations
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<ChargeStationDTO>>(result);
    }
    
    public async Task<ChargeStationEntity> AddChargeStation(ChargeStationEntity chargeStation)
    {
        _context.ChargeStations.Add(chargeStation);
        await _context.SaveChangesAsync();
        
        return chargeStation;
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