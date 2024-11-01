using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Requests.ChargeStation;

namespace SmartCharge.Controllers;

[ApiController]
[Route("[controller]")]
public class ChargeStationController(ISender sender, IMapper mapper) : Controller
{
    [HttpGet("all")]
    public async Task<IActionResult> GetChargeStations()
    {
        var command = new GetChargeStationsQuery();
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<IEnumerable<ChargeStationDto>>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetChargeStation(Guid id)
    {
        var command = new GetChargeStationByIdQuery(id);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<ChargeStationDto>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpPost("group/{groupId:guid}")]
    public async Task<IActionResult> AddChargeStation(Guid groupId, [FromBody]CreateChargeStationRequest request)
    {
        var command = new CreateChargeStationCommand(groupId, request.Name, request.Connectors);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<ChargeStationDto>(result.Data)) : BadRequest(result.Error);
    }
    [HttpPut("{id:guid}/group/{groupId:guid}")]
    public async Task<IActionResult> UpdateChargeStation(Guid groupId, Guid id, [FromBody]UpdateChargeStationRequest request)
    {
        var command = new UpdateChargeStationCommand(id, groupId, request.Name);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<ChargeStationDto>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpDelete("{id:guid}/group/{groupId:guid}")]
    public async Task<IActionResult> DeleteChargeStation(Guid id, Guid groupId)
    {
        var command = new DeleteChargeStationCommand(id, groupId);

        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
}