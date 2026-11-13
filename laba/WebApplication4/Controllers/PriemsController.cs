using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using MedicalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriemsController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public PriemsController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Priem>), StatusCodes.Status200OK)]
    public async Task<List<Priem>> GetAll()
    {
        return await _db.Priems.ToListAsync();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Priem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Priem>> GetById(int id)
    {
        Priem? priem = await _db.Priems.FirstOrDefaultAsync(p => p.Id == id);
        if (priem == null)
        {
            return NotFound();
        }
        return priem;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Priem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Priem>> Create(Priem priem)
    {
        priem.Id = 0; // На всякий случай, чтобы не было попытки вставить конкретный Id

        // Проверяем, что справка существует в БД
        bool spravkaExists = await _db.Spravki.AnyAsync(s => s.Id == priem.SpravkaId);
        if (!spravkaExists)
        {
            return BadRequest($"Справка с Id = {priem.SpravkaId} не найдена");
        }

        bool spravkaValid = await _db.Spravki.AnyAsync(s => s.Id == priem.SpravkaId && s.PatientId == priem.PatientId);
        if (!spravkaValid)
        {
            return BadRequest($"Справка с Id = {priem.SpravkaId} не найдена или принадлежит другому пациенту.");
        }

        _db.Priems.Add(priem);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = priem.Id }, priem);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update(int id, Priem updatedPriem)
    {
        if (id != updatedPriem.Id) return BadRequest("ID в URL и теле не совпадают.");

        Priem? existing = await _db.Priems.FindAsync(id);
        if (existing == null) return NotFound();

        // Проверяем новые FK перед сохранением
        bool spravkaExists = await _db.Spravki.AnyAsync(s => s.Id == updatedPriem.SpravkaId);
        if (!spravkaExists)
        {
            return BadRequest($"Справка с Id = {updatedPriem.SpravkaId} не найдена");
        }

        bool spravkaValid = await _db.Spravki.AnyAsync(s => s.Id == updatedPriem.SpravkaId && s.PatientId == updatedPriem.PatientId);
        if (!spravkaValid)
        {
            return BadRequest($"Справка с Id = {updatedPriem.SpravkaId} не найдена или принадлежит другому пациенту.");
        }


        existing.PatientId = updatedPriem.PatientId;
        existing.SpravkaId = updatedPriem.SpravkaId;
        existing.Date      = updatedPriem.Date;
        existing.Doctor    = updatedPriem.Doctor;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        Priem? priem = await _db.Priems.FirstOrDefaultAsync(p => p.Id == id);
        if (priem == null)
        {
            return NotFound();
        }

        _db.Priems.Remove(priem);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
