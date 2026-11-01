using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditoryBookings;

[ApiController]
[Route("api/v1/auditories")]
public sealed class AuditoryController : ControllerBase
{
    private readonly CampusDbContext _db;
    private static readonly ActivitySource Source = new("Campus.AuditoryBookings");

    public AuditoryController(CampusDbContext db) => _db = db;

    [HttpPut("{id:guid}/capacity")]
    public async Task<IActionResult> UpdateCapacity(
        Guid id, [FromBody] UpdateCapacityRequest request,
        CancellationToken cancellationToken)
    {
        using var activity = Source.StartActivity("auditory.capacity.update",
            ActivityKind.Server) ?? Activity.Current;

        activity?.SetTag("jira.issue", "CAMPUS-102");
        activity?.SetTag("campus.auditory_id", id.ToString());
        activity?.SetTag("campus.capacity", request.Capacity);

        if (request.Capacity < 0)
            return BadRequest("Capacity must be zero or greater.");

        var auditory = await _db.Auditories.SingleOrDefaultAsync(
            x => x.Id == id, cancellationToken);

        if (auditory is null)
            return NotFound();

        // Expand-фаза: dual-write в старую smallint и новую integer колонки.
        // Пока старая колонка существует, действует ограничение short.MaxValue.
        if (request.Capacity > short.MaxValue)
            return Conflict($"Capacity cannot exceed {short.MaxValue} before Contract.");

        auditory.Capacity = checked((short)request.Capacity);
        auditory.CapacityInt = request.Capacity;
        await _db.SaveChangesAsync(cancellationToken);

        activity?.SetTag("campus.capacity_write_mode", "dual");
        return Ok(new { auditory.Id, Capacity = auditory.CapacityInt });
    }
}

public sealed record UpdateCapacityRequest(int Capacity);

public sealed class Auditory
{
    public Guid Id { get; set; }
    public short Capacity { get; set; } // legacy
    public int CapacityInt { get; set; } // expanded column
}

public sealed class CampusDbContext(DbContextOptions<CampusDbContext> options)
    : DbContext(options)
{
    public DbSet<Auditory> Auditories => Set<Auditory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auditory>(e =>
        {
            e.ToTable("Auditories");
            e.HasKey(x => x.Id);
            e.Property(x => x.Capacity).HasColumnName("Capacity");
            e.Property(x => x.CapacityInt).HasColumnName("CapacityInt");
        });
    }
}
