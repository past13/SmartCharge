using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Requests.Connector;

namespace SmartCharge.Controllers;

[ApiController]
[Route("[controller]")]
public class ConnectorController(ISender sender, IMapper mapper) : Controller
{
    [HttpGet("all")]
    public async Task<IActionResult> GetConnectors()
    {
        var command = new GetConnectorsQuery();
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<IEnumerable<ConnectorDto>>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetConnector(Guid id)
    {
        var command = new GetConnectorByIdQuery(id);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<ConnectorDto>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpPut("chargestation/{chargeStationId:guid}")]
    public async Task<IActionResult> AddConnector(Guid chargeStationId, [FromBody]CreateConnectorRequest request)
    {
        var command = new CreateConnectorCommand(request.Name, request.CapacityInAmps, chargeStationId);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<ConnectorDto>(result.Data)) : BadRequest(result.Error);
    }
    
    [HttpPut("{id:guid}/chargestation/{chargeStationId:guid}")]
    public async Task<IActionResult> UpdateConnector(Guid id, Guid chargeStationId, [FromBody]UpdateConnectorRequest request)
    {
        var command = new UpdateConnectorCommand(id, chargeStationId, request.Name, request.MaxCurrentInAmps);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(mapper.Map<ConnectorDto>(result.Data)) : BadRequest(result.Error);

    }
    
    [HttpDelete("{id:guid}/chargestation/{chargeStationId:guid}")]
    public async Task<IActionResult> DeleteConnectorById(Guid chargeStationId, Guid id)
    {
        var command = new DeleteConnectorCommand(chargeStationId, id);
        
        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
}