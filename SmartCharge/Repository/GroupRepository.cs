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

public interface IGroupRepository
{
    Task<bool> IsNameExist(string name);
    Task<GroupEntity> AddGroup(GroupEntity group);
    Task<GroupEntity?> GetGroupById(Guid id);
    Task<GroupEntity?> GetGroupByChargeStationId(Guid chargeStationId);
    Task DeleteGroupById(Guid id);
    Task<IEnumerable<GroupDto>> GetAllGroups();
}

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IChargeStationRepository _chargeStationRepository;
    
    public GroupRepository(
        ApplicationDbContext context, 
        IMapper mapper,
        IChargeStationRepository chargeStationRepository)
    {
        _context = context;
        _mapper = mapper;
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<bool> IsNameExist(string name)
    {
        var isNameValid = await _context.Groups
            .AnyAsync(g => g.Name.ToLower() == name.ToLower());
        
        return isNameValid;
    }
    
    public async Task<IEnumerable<GroupDto>> GetAllGroups()
    {
        var groups = await _context.Groups
            .Include(x => x.ChargeStations)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<GroupDto>>(groups);
    }
    
    public async Task<GroupEntity> AddGroup(GroupEntity group)
    {
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        
        return group;
    }

    public async Task<GroupEntity> GetGroupById(Guid id)
    {
        return await _context.Groups
            .Include(g => g.ChargeStations)
            .ThenInclude(c => c.Connectors)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
    
    public async Task<GroupEntity> GetGroupByChargeStationId(Guid chargeStationId)
    {
        return await _context.Groups
            .Include(g => g.ChargeStations)
            .ThenInclude(c => c.Connectors)
            .FirstOrDefaultAsync(g => g.ChargeStations.Any(cs => cs.Id == chargeStationId));
    }
    
    public async Task DeleteGroupById(Guid id)
    {
        var group = await _context.Groups
            .Include(g => g.ChargeStations)
            .FirstOrDefaultAsync(g => g.Id == id);
        
        foreach (var chargeStation in group.ChargeStations.ToList())
        {
            await _chargeStationRepository.DeleteChargeStationById(chargeStation.Id);
        }
        
        _context.Groups.Remove(group);
    }
}