using Microsoft.AspNetCore.Mvc;
using MedicalApi.Data;
using MedicalApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public StatsController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StatsDto>> Get()
    {
        int count = await _db.Spravki.CountAsync();
        decimal total = await _db.Spravki.SumAsync(s => s.Price);

        return new StatsDto { SpravkiCount = count, Total = total };
    }
}
