using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Requests.Group;

namespace SmartCharge.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController(
    ISender sender,
    IMapper mapper) : Controller
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAllGroups()
    {
        var command = new GetGroupsQuery();
        
        var result = await sender.Send(command);
        
        return result.IsSuccess ? Ok(mapper.Map<IEnumerable<GroupDto>>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGroupById(Guid id)
    {
        var command = new GetGroupByIdQuery(id);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<GroupDto>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddGroup([FromBody]CreateGroupRequest request)
    {
        var command = new CreateGroupCommand(request.Name, request.ChargeStation);

        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<GroupDto>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateGroup([FromBody]UpdateGroupRequest request)
    {
        var command = new UpdateGroupCommand(request.Id, request.Name);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<GroupDto>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGroupById(Guid id)
    {
        var command = new DeleteGroupCommand(id);

        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);

    }
}