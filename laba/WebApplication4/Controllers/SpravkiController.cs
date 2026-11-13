using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using MedicalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public SpravkiController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Spravka>), StatusCodes.Status200OK)]
    public async Task<List<Spravka>> GetAll()
    {
        return await _db.Spravki.ToListAsync();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Spravka), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Spravka>> GetById(int id)
    {
        Spravka? spravka = await _db.Spravki.FirstOrDefaultAsync(s => s.Id == id);
        if (spravka == null)
        {
            return NotFound();
        }
        return spravka;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Spravka), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Spravka>> Create(Spravka spravka)
    {
        spravka.Id = 0; // На всякий случай, чтобы не было попытки вставить конкретный Id

        // Проверяем, что пациент существует в БД
        bool patientExists = await _db.Patients.AnyAsync(p => p.Id == spravka.PatientId);
        if (!patientExists)
        {
            return BadRequest($"Пациент с Id = {spravka.PatientId} не найден");
        }

        // Проверяем, что тип справки существует в БД, и подтягиваем из него данные
        SpravkaType? type = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == spravka.SpravkaTypeId);
        if (type == null)
        {
            return BadRequest($"Тип справки с Id = {spravka.SpravkaTypeId} не найден");
        }

        // Цена и название берутся из типа справки
        spravka.Price = type.Price;
        spravka.Name  = type.Name;

        _db.Spravki.Add(spravka);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update(int id, Spravka updatedSpravka)
    {
        if (id != updatedSpravka.Id) return BadRequest("ID в URL и теле не совпадают.");

        Spravka? existing = await _db.Spravki.FindAsync(id);
        if (existing == null) return NotFound();

        // Проверяем новые FK
        bool patientExists = await _db.Patients.AnyAsync(p => p.Id == updatedSpravka.PatientId);
        if (!patientExists)
        {
            return BadRequest($"Пациент с Id = {updatedSpravka.PatientId} не найден");
        }
        SpravkaType? type = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == updatedSpravka.SpravkaTypeId);
        if (type == null)
        {
            return BadRequest($"Тип справки с Id = {updatedSpravka.SpravkaTypeId} не найден");
        }

        existing.PatientId     = updatedSpravka.PatientId;
        existing.SpravkaTypeId = updatedSpravka.SpravkaTypeId;
        existing.Price         = type.Price;   // всегда берём из типа
        existing.Name          = type.Name;    // всегда берём из типа

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        Spravka? spravka = await _db.Spravki.FirstOrDefaultAsync(s => s.Id == id);
        if (spravka == null)
        {
            return NotFound();
        }

        _db.Spravki.Remove(spravka);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
