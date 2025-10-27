using Microsoft.AspNetCore.Mvc;
using Ambev.DeveloperEvaluation.WebApi.Services;
using Ambev.DeveloperEvaluation.Domain.DTOs;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Vendas;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly VendaService _service;

    public VendasController(VendaService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = _service.Listar().Select(v => _service.ToResponse(v));
        return Ok(list);
    }   

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var v = _service.Obter(id);
        if (v == null) return NotFound();
        return Ok(_service.ToResponse(v));
    }

    [HttpPost]
    public IActionResult Create([FromBody] VendaCreateRequest req)
    {
        try
        {
            var venda = _service.CriarVenda(req);
            return CreatedAtAction(nameof(Get), new { id = venda.Id }, _service.ToResponse(venda));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] VendaUpdateRequest req)
    {
        try
        {
            var updated = _service.Atualizar(id, req);
            return Ok(_service.ToResponse(updated));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/cancel")]
    public IActionResult CancelSale(Guid id)
    {
        try
        {
            _service.CancelarVenda(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{vendaId:guid}/itens/{itemId:guid}/cancel")]
    public IActionResult CancelItem(Guid vendaId, Guid itemId)
    {
        try
        {
            _service.CancelarItem(vendaId, itemId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}