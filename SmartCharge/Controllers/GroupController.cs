using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands.Group;
using SmartCharge.Repository;

namespace SmartCharge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : Controller
{
    private readonly ISender _sender;
    private readonly IGroupRepository _groupRepository;
    
    public GroupController(ISender sender, IGroupRepository groupRepository)
    {
        _sender = sender;
        _groupRepository = groupRepository;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddGroup(CreateGroupCommand command)
    {
        var group = await _sender.Send(command);
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateGroup(UpdateGroupCommand command)
    {
        var group = await _sender.Send(command);
        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteGroupById(DeleteGroupCommand command)
    {
        await _sender.Send(command);
        return NoContent();
    }
    
    // [HttpGet("{id:Guid}")]
    // public async Task<IActionResult> GetGroupById(Guid id)
    // {
    //     var group = await _groupRepository.GetGroupById(id);
    //     if (group == null)
    //     {
    //         return NotFound();
    //     }
    //     return Ok(group);
    // }
    
    [HttpGet]
    public async Task<IActionResult> GetAllGroups()
    {
        var groups = await _groupRepository.GetAllGroups();
        return Ok(groups);
    }
}