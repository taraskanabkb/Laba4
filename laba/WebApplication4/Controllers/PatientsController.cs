using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using MedicalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public PatientsController(MedicalDbContext db)
    {
        _db = db;
    }

    // Один метод GetAll обрабатывает и "вернуть всех", и "отфильтровать по имени".
    // /api/patients          -> все пациенты
    // /api/patients?name=Иван -> только те, у кого имя содержит "Иван"
    [HttpGet]
    [ProducesResponseType(typeof(List<Patient>), StatusCodes.Status200OK)]
    public async Task<List<Patient>> GetAll(string? name = null)
    {
        IQueryable<Patient> query = _db.Patients;
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{name}%"));
        }
        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Patient>> GetById(int id)
    {
        Patient? patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }
        return patient;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Patient>> Create(Patient patient)
    {
        if (patient == null) { return BadRequest("Данные пациента не переданы."); }
        //if (patient.BirthDate > DateOnly.FromDateTime(DateTime.UtcNow)) { return BadRequest("Дата рождения не может быть в будущем."); }

        patient.Id = 0; // На всякий случай, чтобы не было попытки вставить конкретный Id

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update(int id, Patient updatedPatient)
    {
        if (id != updatedPatient.Id) return BadRequest("ID в URL и теле не совпадают.");

        Patient? existing = await _db.Patients.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name      = updatedPatient.Name;
        existing.BirthDate = updatedPatient.BirthDate;
        existing.BloodType = updatedPatient.BloodType;
        existing.Old       = updatedPatient.Old;
        existing.inn       = updatedPatient.inn;
        existing.Email = updatedPatient.Email;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        Patient? patient = await _db.Patients.FindAsync(id);
        if (patient == null) return NotFound();

        // 1. Проверяем, есть ли у пациента справки. Если есть — запрещаем удаление и отдаем 400
        bool hasSpravki = await _db.Spravki.AnyAsync(s => s.PatientId == id);
        if (hasSpravki)
        {
            return BadRequest("Нельзя удалить пациента, так как на него оформлены медицинские справки.");
        }

        // 2. Если справок нет, удаляем только приёмы (справки удалять больше не нужно)
        var priems = _db.Priems.Where(p => p.PatientId == id);
        _db.Priems.RemoveRange(priems);

        // 3. Удаляем самого пациента
        _db.Patients.Remove(patient);
        await _db.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("{id}/spravki")]
    [ProducesResponseType(typeof(List<Spravka>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<Spravka>>> GetSpravki(int id)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == id))
        {
            return NotFound();
        }

        List<Spravka> result = await _db.Spravki
            .Where(s => s.PatientId == id)
            .ToListAsync();

        return result; // 200 со списком
    }
}
