using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Requests.ChargeStation;
using SmartCharge.Repository;

namespace SmartCharge.Controllers;

[ApiController]
[Route("[controller]")]
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
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllChargeStations()
    {
        var chargeStation = await _chargeStationRepository.GetAllChargeStations();
        return Ok(chargeStation);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetChargeStation(Guid id)
    {
        var command = new GetChargeStationByIdQuery(id);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
    
    [HttpPost("group/{groupId:guid}")]
    public async Task<IActionResult> AddChargeStation(Guid groupId, [FromBody]CreateChargeStationRequest request)
    {
        var command = new CreateChargeStationCommand(groupId, request.Name, request.Connectors);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
    [HttpPut("{id:guid}/group/{groupId:guid}")]
    public async Task<IActionResult> UpdateChargeStation(Guid groupId, Guid id, [FromBody]UpdateChargeStationRequest request)
    {
        var command = new UpdateChargeStationCommand(id, groupId, request.Name);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
    
    [HttpDelete("{id:guid}/group/{groupId:guid}")]
    public async Task<IActionResult> DeleteChargeStation(Guid id, Guid groupId)
    {
        var command = new DeleteChargeStationCommand(id, groupId);

        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
}