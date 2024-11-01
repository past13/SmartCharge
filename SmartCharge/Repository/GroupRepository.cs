using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartCharge.DataLayer;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using GroupEntity = SmartCharge.Domain.Entities.GroupEntity;

namespace SmartCharge.Repository;

public interface IGroupRepository
{
    Task<bool> IsNameExist(string name, Guid? id = null);
    Task<GroupEntity> AddGroup(GroupEntity group);
    Task<GroupEntity?> GetGroupById(Guid id);
    Task<GroupEntity?> GetGroupByChargeStationId(Guid chargeStationId);
    Task DeleteGroupById(Guid id);
    Task<IEnumerable<GroupEntity>> GetGroups();
}

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IChargeStationRepository _chargeStationRepository;
    
    public GroupRepository(
        ApplicationDbContext context, 
        IChargeStationRepository chargeStationRepository)
    {
        _context = context;
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<bool> IsNameExist(string name, Guid? id)
    {
        return await _context.Groups
            .AnyAsync(g => g.Name.ToLower() == name.ToLower() && (!id.HasValue || g.Id != id.Value));
    }
    
    public async Task<IEnumerable<GroupEntity>> GetGroups()
    {
        return await _context.Groups
            .Include(g => g.ChargeStations)
            .ThenInclude(cs => cs.Connectors)
            .Where(g => g.RowState == RowState.Active)
            .ToListAsync();
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