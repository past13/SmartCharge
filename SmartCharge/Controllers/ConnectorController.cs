using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Requests.Connector;
using SmartCharge.Repository;

namespace SmartCharge.Controllers;

[ApiController]
[Route("[controller]")]
public class ConnectorController : Controller
{
    private readonly ISender _sender;
    private readonly IConnectorRepository _connectorRepository;
    
    public ConnectorController(
        ISender sender, 
        IConnectorRepository connectorRepository)
    {
        _sender = sender;
        _connectorRepository = connectorRepository;
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllConnectors()
    {
        var connectors = await _connectorRepository.GetAllConnectors();
        return Ok(connectors);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> AddConnector(Guid id)
    {
        var command = new GetConnectorByIdQuery(id);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
    
    [HttpPut("chargestation/{chargeStationId:guid}")]
    public async Task<IActionResult> AddConnector(Guid chargeStationId, [FromBody]CreateConnectorRequest request)
    {
        var command = new CreateConnectorCommand(request.Name, request.CapacityInAmps, chargeStationId);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
    
    [HttpPut("{id:guid}/chargestation/{chargeStationId:guid}")]
    public async Task<IActionResult> UpdateConnector(Guid id, Guid chargeStationId, [FromBody]UpdateConnectorRequest request)
    {
        var command = new UpdateConnectorCommand(id, chargeStationId, request.Name, request.MaxCurrentInAmps);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);

    }
    
    [HttpDelete("{id:guid}/chargestation/{chargeStationId:guid}")]
    public async Task<IActionResult> DeleteConnectorById(Guid chargeStationId, Guid id)
    {
        var command = new DeleteConnectorCommand(chargeStationId, id);
        
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
}