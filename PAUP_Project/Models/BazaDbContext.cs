using PAUP_Project.Models;
using System.Data.Entity;

public class BazaDbContext : DbContext
{
    public DbSet<Motori> Motori { get; set; }
    public DbSet<PrijavaKorisnika> PrijavaKorisnika { get; set; }
    public DbSet<RegistracijaKorisnika> RegistracijaKorisnika { get; set; }
    public DbSet<Reklamacija> Reklamacije { get; set; }
    public DbSet<Racun> Racuni { get; set; }
    public DbSet<RacunStavka> RacunStavke { get; set; }


    // ✅ DODAJ OVO:
    public DbSet<KosaricaStavka> KosaricaStavke { get; set; }

    public BazaDbContext() : base("name=PAUP_Project") { }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Motori>().ToTable("motori");
        modelBuilder.Entity<RegistracijaKorisnika>().ToTable("registracijakorisnika");
        modelBuilder.Entity<PrijavaKorisnika>().ToTable("prijavakorisnika");
        modelBuilder.Entity<Racun>().ToTable("racuni");
        modelBuilder.Entity<RacunStavka>().ToTable("racunstavke");
        modelBuilder.Entity<Reklamacija>().ToTable("reklamacije");
        modelBuilder.Entity<KosaricaStavka>().ToTable("kosaricastavke");

        modelBuilder.Entity<RacunStavka>()
            .HasKey(rs => rs.Id);

        modelBuilder.Entity<Racun>()
            .HasKey(r => r.RacunID);

        modelBuilder.Entity<Racun>()
            .HasMany(r => r.RacunStavke)
            .WithRequired(rs => rs.Racun)
            .HasForeignKey(rs => rs.RacunID);

        modelBuilder.Entity<Reklamacija>()
            .HasKey(r => r.ReklamacijaID);

        modelBuilder.Entity<Reklamacija>()
            .HasRequired(r => r.RacunStavka)
            .WithMany()
            .HasForeignKey(r => r.RacunStavkaID);
    }
}
