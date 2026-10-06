using Microsoft.AspNetCore.Mvc;
using Luilton_P1_P4.Services;
using Luilton_P1_P4.Models;

namespace Luilton_P1_P4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutoresController(AutorService service, ILogger<AutoresController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AutorGet>> Post(AutorSet request)
    {
        var guardado = await service.SaveAsync(request);
        logger.LogInformation("Guardado registro {IdAutor}", guardado.IdAutor);
        return CreatedAtAction(nameof(Get), new { id = guardado.IdAutor }, guardado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, AutorSet request)
    {
        var ok = await service.UpdateAsync(id, request);
        if (!ok) logger.LogWarning("Intento de actualizar Id {IdAutor} que no existe", id);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AutorGet>> Get(int id) =>
        await service.GetByIdAsync(id) is { } r ? Ok(r) : NotFound();

    [HttpGet]
    public async Task<ActionResult<List<AutorGet>>> List() =>
        Ok(await service.GetListAsync());


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminado = await service.DeleteAsync(id);

        if (!eliminado)
            return NotFound($"No se encontró el autor con ID {id}.");

        return NoContent();
    }
}
