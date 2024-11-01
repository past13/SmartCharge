using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Requests.Group;
using SmartCharge.Repository;

namespace SmartCharge.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : Controller
{
    private readonly ISender _sender;
    private readonly IGroupRepository _groupRepository;
    
    public GroupController(ISender sender, IGroupRepository groupRepository)
    {
        _sender = sender;
        _groupRepository = groupRepository;
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllGroups()
    {
        var groups = await _groupRepository.GetAllGroups();
        return Ok(groups);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGroupById(Guid id)
    {
        var command = new GetGroupByIdQuery(id);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddGroup([FromBody]CreateGroupRequest request)
    {
        var command = new CreateGroupCommand(request.Name, request.ChargeStation);

        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateGroup([FromBody]UpdateGroupRequest request)
    {
        var command = new UpdateGroupCommand(request.Id, request.Name);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGroupById(Guid id)
    {
        var command = new DeleteGroupCommand(id);

        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);

    }
}