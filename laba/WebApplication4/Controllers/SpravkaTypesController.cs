using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using MedicalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public SpravkaTypesController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SpravkaType>), StatusCodes.Status200OK)]
    public async Task<List<SpravkaType>> GetAll()
    {
        return await _db.SpravkaTypes.ToListAsync();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpravkaType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpravkaType>> GetById(int id)
    {
        SpravkaType? type = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == id);
        if (type == null)
        {
            return NotFound();
        }
        return type;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SpravkaType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SpravkaType>> Create(SpravkaType type)
    {
        type.Id = 0; // На всякий случай, чтобы не было попытки вставить конкретный Id

        _db.SpravkaTypes.Add(type);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update(int id, SpravkaType updated)
    {
        if (id != updated.Id) return BadRequest("ID в URL и теле не совпадают.");

        SpravkaType? existing = await _db.SpravkaTypes.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name  = updated.Name;
        existing.Price = updated.Price;
        existing.Date  = updated.Date;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(int id)
    {
        SpravkaType? type = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == id);
        if (type == null)
        {
            return NotFound();
        }

        bool isUsed = await _db.Spravki.AnyAsync(s => s.SpravkaTypeId == id);
        if (isUsed)
        {
            return BadRequest("Нельзя удалить вид справки, так как по нему уже оформлены справки.");
        }

        _db.SpravkaTypes.Remove(type);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
