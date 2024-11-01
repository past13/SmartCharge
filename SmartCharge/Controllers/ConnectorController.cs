using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Requests.Connector;
using SmartCharge.Repository;

namespace SmartCharge.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        
        var connector = await _sender.Send(command);
        return Ok(connector);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddConnector([FromBody]CreateConnectorRequest request)
    {
        var command = new CreateConnectorCommand(request.Name, request.CapacityInAmps, request.ChargeStationId);
        
        var connector = await _sender.Send(command);
        return Ok(connector);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateConnector([FromBody]UpdateConnectorRequest request)
    {
        var command = new UpdateConnectorCommand(request.Id, request.ChargeStationId, request.Name, request.MaxCurrentInAmps);
        
        var connector = await _sender.Send(command);
        return Ok(connector);
    }
    
    [HttpDelete("{chargeStationId:guid}/{id:guid}")]
    public async Task<IActionResult> DeleteConnectorById(Guid chargeStationId, Guid id)
    {
        var command = new DeleteConnectorCommand(chargeStationId, id);
        
        await _sender.Send(command);
        return NoContent();
    }
}