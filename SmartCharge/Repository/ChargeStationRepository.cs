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
    Task<ChargeStationEntity> AddChargeStation(Guid groupId, ChargeStationEntity chargeStation);
    Task<ApiResponse<ChargeStationEntity>> UpdateChargeStation(ChargeStationEntity chargeStation);
    Task<ChargeStationEntity?> GetChargeStationById(Guid id);
    Task<ApiResponse<ChargeStationEntity>> DeleteChargeStationById(Guid id);
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

    public async Task<ApiResponse<ChargeStationEntity>> UpdateChargeStation(ChargeStationEntity chargeStation)
    {
        try
        {
            await _context.SaveChangesAsync();
            
            return new ApiResponse<ChargeStationEntity>
            {
                Data = null,
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            return new ApiResponse<ChargeStationEntity>
            {
                Data = null,
                Error = "The ChargeStation was modified by another user since you loaded it. Please reload the data and try again."
            };
        }
    }

    public async Task<ChargeStationEntity?> GetChargeStationById(Guid id)
    {
        return await _context.ChargeStations
            .FirstOrDefaultAsync(cs => cs.Id == id);
    }
    
    public async Task<ApiResponse<ChargeStationEntity>> DeleteChargeStationById(Guid id)
    {
        var response = new ApiResponse<ChargeStationEntity>();

        var chargeStation = await GetChargeStationById(id);
        if (chargeStation == null)
        {
            response.Error = $"A ChargeStation with the Id {id} does not exists.";
            return response;
        }

        var connectors = chargeStation.Connectors.ToList();
        foreach (var connector in connectors)
        {
            await _connectorRepository.DeleteConnectorById(connector.Id);
        }
        
        _context.ChargeStations.Remove(chargeStation);
        
        try
        {
            await _context.SaveChangesAsync();
            
            return new ApiResponse<ChargeStationEntity>
            {
                Data = chargeStation,
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            return new ApiResponse<ChargeStationEntity>
            {
                Data = null,
                Error = "The ChargeStation was modified by another user since you loaded it. Please reload the data and try again."
            };
        }
    }
}