using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Commands.Group;
using SmartCharge.Repository;

namespace SmartCharge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChargeStationController : Controller
{
    private readonly ISender _sender;
    private readonly IChargeStationRepository _chargeStationRepository;
    
    public ChargeStationController(
        ISender sender, 
        IChargeStationRepository chargeStationRepository)
    {
        _sender = sender;
        _chargeStationRepository = chargeStationRepository;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddChargeStation(CreateChargeStationCommand command)
    {
        var chargeStation = await _sender.Send(command);
        return Ok(chargeStation);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateGroup(UpdateGroupCommand command)
    {
        var chargeStation = await _sender.Send(command);
        return Ok(chargeStation);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteGroupById(DeleteGroupCommand command)
    {
        await _sender.Send(command);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllGroups()
    {
        var chargeStation = await _chargeStationRepository.GetAllChargeStations();
        return Ok(chargeStation);
    }
}