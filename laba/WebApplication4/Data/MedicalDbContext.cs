using MedicalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Data;

public class MedicalDbContext : DbContext
{
    public MedicalDbContext(DbContextOptions<MedicalDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Spravka> Spravki { get; set; }
    public DbSet<SpravkaType> SpravkaTypes { get; set; }
    public DbSet<Priem> Priems { get; set; } // таблица модуля приёмов
    // сюда же таблица вашего модуля

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // у справки один пациент, у пациента много справок, связь через PatientId
        modelBuilder.Entity<Spravka>()
            .HasOne<Patient>()
            .WithMany()
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.Restrict);



        // у справки один вид, у вида много справок, связь через SpravkaTypeId
        modelBuilder.Entity<Spravka>()
            .HasOne<SpravkaType>()
            .WithMany()
            .HasForeignKey(s => s.SpravkaTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // у приёма один пациент, у пациента много приёмов
        modelBuilder.Entity<Priem>()
            .HasOne<Patient>()
            .WithMany()
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // у приёма одна справка, у справки много приёмов
        modelBuilder.Entity<Priem>()
            .HasOne<Spravka>()
            .WithMany()
            .HasForeignKey(p => p.SpravkaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
