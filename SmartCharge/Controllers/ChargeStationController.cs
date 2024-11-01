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
        
        var chargeStation = await _sender.Send(command);
        return Ok(chargeStation);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddChargeStation([FromBody]CreateChargeStationRequest request)
    {
        var command = new CreateChargeStationCommand(request.GroupId, request.Name, request.Connectors);
        
        var chargeStation = await _sender.Send(command);
        return Ok(chargeStation);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateChargeStation([FromBody]UpdateChargeStationRequest request)
    {
        var command = new UpdateChargeStationCommand(request.Id, request.GroupId, request.Name);
        
        var chargeStation = await _sender.Send(command);
        return Ok(chargeStation);
    }
    
    [HttpDelete("{id:guid}/{groupId:guid}")]
    public async Task<IActionResult> DeleteChargeStation(Guid id, Guid groupId)
    {
        var command = new DeleteChargeStationCommand(id, groupId);

        var result = await _sender.Send(command);
        return Ok(result);
    }
}