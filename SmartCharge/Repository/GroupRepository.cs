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
    Task<GroupEntity> GetGroupById(Guid id);
    Task<GroupEntity> GetGroupByChargeStationId(Guid chargeStationId);
    Task DeleteGroupById(Guid id);
    Task<IEnumerable<GroupEntity>> GetGroups();
}

public class GroupRepository(
    ApplicationDbContext context,
    IChargeStationRepository chargeStationRepository)
    : IGroupRepository
{
    public async Task<bool> IsNameExist(string name, Guid? id)
    {
        return await context.Groups
            .AnyAsync(g => g.Name.ToLower() == name.ToLower() && (!id.HasValue || g.Id != id.Value));
    }
    
    public async Task<IEnumerable<GroupEntity>> GetGroups()
    {
        return await context.Groups
            .Include(g => g.ChargeStations)
            .ThenInclude(cs => cs.Connectors)
            .Where(g => g.RowState == RowState.Active)
            .ToListAsync();
    }
    
    public async Task<GroupEntity> AddGroup(GroupEntity group)
    {
        context.Groups.Add(group);
        await context.SaveChangesAsync();
        
        return group;
    }

    public async Task<GroupEntity> GetGroupById(Guid id)
    {
        return await context.Groups
            .Include(g => g.ChargeStations)
            .ThenInclude(c => c.Connectors)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
    
    public async Task<GroupEntity> GetGroupByChargeStationId(Guid chargeStationId)
    {
        return await context.Groups
            .Include(g => g.ChargeStations)
            .ThenInclude(c => c.Connectors)
            .FirstOrDefaultAsync(g => g.ChargeStations.Any(cs => cs.Id == chargeStationId));
    }
    
    public async Task DeleteGroupById(Guid id)
    {
        var group = await context.Groups
            .Include(g => g.ChargeStations)
            .FirstOrDefaultAsync(g => g.Id == id);
        
        foreach (var chargeStation in group.ChargeStations.ToList())
        {
            await chargeStationRepository.DeleteChargeStationById(chargeStation.Id);
        }
        
        context.Groups.Remove(group);
    }
}