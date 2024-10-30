using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCharge.Commands;
using SmartCharge.Commands.Connector;
using SmartCharge.Repository;

namespace SmartCharge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConnectorController : Controller
{
    private readonly ISender _sender;
    private readonly IConnectorRepository _connectorRepository;
    
    public ConnectorController(ISender sender, IConnectorRepository connectorRepository)
    {
        _sender = sender;
        _connectorRepository = connectorRepository;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddConnector(CreateConnectorCommand command)
    {
        var connector = await _sender.Send(command);
        return Ok(connector);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateConnector(UpdateConnectorCommand command)
    {
        var connector = await _sender.Send(command);
        return Ok(connector);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteConnectorById(DeleteConnectorCommand command)
    {
        await _sender.Send(command);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllConnectors()
    {
        var connectors = await _connectorRepository.GetAllConnectors();
        return Ok(connectors);
    }
}