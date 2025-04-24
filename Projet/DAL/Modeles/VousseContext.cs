using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Modeles
{
    public partial class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Artiste> Artistes { get; set; }
        public virtual DbSet<Billet> Billets { get; set; }
        public virtual DbSet<GroupesSpectacle> GroupesSpectacles { get; set; }
        public virtual DbSet<GroupesSpectaclesOrganisation> GroupesSpectaclesOrganisations { get; set; }
        public virtual DbSet<Programmation> Programmations { get; set; }
        public virtual DbSet<Spectacle> Spectacles { get; set; }
        public virtual DbSet<TarifsGroupe> TarifsGroupes { get; set; }
        public virtual DbSet<TarifsSpectacle> TarifsSpectacles { get; set; }
        public virtual DbSet<TypesTarif> TypesTarifs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=Localhost;Initial Catalog=InscriptionsSpectacles;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artiste>(entity =>
            {
                entity.HasKey(e => e.ArtisteId).HasName("PK__Artistes__6635EC444C40FE8D");

                entity.Property(e => e.ArtisteId).HasColumnName("ArtisteID");
                entity.Property(e => e.Nom)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasMany(d => d.Spectacles).WithMany(p => p.Artistes)
                    .UsingEntity<Dictionary<string, object>>(
                        "ArtistesSpectacle",
                        r => r.HasOne<Spectacle>().WithMany()
                            .HasForeignKey("SpectacleId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__ArtistesS__Spect__4222D4EF"),
                        l => l.HasOne<Artiste>().WithMany()
                            .HasForeignKey("ArtisteId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__ArtistesS__Artis__412EB0B6"),
                        j =>
                        {
                            j.HasKey("ArtisteId", "SpectacleId").HasName("PK__Artistes__23683982BC8F2C45");
                            j.ToTable("ArtistesSpectacles");
                            j.IndexerProperty<int>("ArtisteId").HasColumnName("ArtisteID");
                            j.IndexerProperty<int>("SpectacleId").HasColumnName("SpectacleID");
                        });
            });

            modelBuilder.Entity<Billet>(entity =>
            {
                entity.HasKey(e => e.BilletId).HasName("PK__Billets__39A625695730C264");

                entity.Property(e => e.BilletId).HasColumnName("BilletID");
                entity.Property(e => e.Civilite)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.Nom)
                    .HasMaxLength(12)
                    .IsUnicode(false);
                entity.Property(e => e.Prenom)
                    .HasMaxLength(12)
                    .IsUnicode(false);
                entity.Property(e => e.PrixAchat).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.ProgrammationId).HasColumnName("ProgrammationID");
                entity.Property(e => e.TarifId).HasColumnName("TarifID");

                entity.HasOne(d => d.Programmation).WithMany(p => p.Billets)
                    .HasForeignKey(d => d.ProgrammationId)
                    .HasConstraintName("FK__Billets__Program__48CFD27E");

                entity.HasOne(d => d.Tarif).WithMany(p => p.Billets)
                    .HasForeignKey(d => d.TarifId)
                    .HasConstraintName("FK__Billets__TarifID__47DBAE45");
            });

            modelBuilder.Entity<GroupesSpectacle>(entity =>
            {
                entity.HasKey(e => e.GroupeId).HasName("PK__GroupesS__5C811B3043F4298F");

                entity.Property(e => e.GroupeId).HasColumnName("GroupeID");
                entity.Property(e => e.NomGroupe)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasMany(d => d.Spectacles).WithMany(p => p.Groupes)
                    .UsingEntity<Dictionary<string, object>>(
                        "SpectaclesGroupe",
                        r => r.HasOne<Spectacle>().WithMany()
                            .HasForeignKey("SpectacleId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__Spectacle__Spect__4E88ABD4"),
                        l => l.HasOne<GroupesSpectacle>().WithMany()
                            .HasForeignKey("GroupeId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__Spectacle__Group__4D94879B"),
                        j =>
                        {
                            j.HasKey("GroupeId", "SpectacleId").HasName("PK__Spectacl__19DCCEF6096A829E");
                            j.ToTable("SpectaclesGroupes");
                            j.IndexerProperty<int>("GroupeId").HasColumnName("GroupeID");
                            j.IndexerProperty<int>("SpectacleId").HasColumnName("SpectacleID");
                        });
            });

            modelBuilder.Entity<GroupesSpectaclesOrganisation>(entity =>
            {
                entity.HasKey(e => e.GroupeId).HasName("PK__GroupesS__5C811B3044AF5A05");

                entity.ToTable("GroupesSpectaclesOrganisation");

                entity.Property(e => e.GroupeId)
                    .ValueGeneratedNever()
                    .HasColumnName("GroupeID");
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.TypeSpectacle)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Groupe).WithOne(p => p.GroupesSpectaclesOrganisation)
                    .HasForeignKey<GroupesSpectaclesOrganisation>(d => d.GroupeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GroupesSp__Group__5535A963");
            });

            modelBuilder.Entity<Programmation>(entity =>
            {
                entity.HasKey(e => e.ProgrammationId).HasName("PK__Programm__3AFFDBE06F636068");

                entity.ToTable("Programmation");

                entity.Property(e => e.ProgrammationId).HasColumnName("ProgrammationID");
                entity.Property(e => e.Lieu)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.SpectacleId).HasColumnName("SpectacleID");

                entity.HasOne(d => d.Spectacle).WithMany(p => p.Programmations)
                    .HasForeignKey(d => d.SpectacleId)
                    .HasConstraintName("FK__Programma__Spect__44FF419A");
            });

            modelBuilder.Entity<Spectacle>(entity =>
            {
                entity.HasKey(e => e.SpectacleId).HasName("PK__Spectacl__55DD5C655BDE6A97");

                entity.Property(e => e.SpectacleId).HasColumnName("SpectacleID");
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.Titre)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.Saison)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.DeconseilleAuxEnfants)
                    .HasDefaultValue(false);

                entity.HasOne(d => d.SpectacleEnfant1)
                    .WithMany(p => p.SpectaclesParent1)
                    .HasForeignKey(d => d.SpectacleEnfant1Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Spectacle_SpectacleEnfant1");

                entity.HasOne(d => d.SpectacleEnfant2)
                    .WithMany(p => p.SpectaclesParent2)
                    .HasForeignKey(d => d.SpectacleEnfant2Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Spectacle_SpectacleEnfant2");

                entity.HasOne(d => d.SpectacleEnfant3)
                    .WithMany(p => p.SpectaclesParent3)
                    .HasForeignKey(d => d.SpectacleEnfant3Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Spectacle_SpectacleEnfant3");
            });

            modelBuilder.Entity<TarifsGroupe>(entity =>
            {
                entity.HasKey(e => new { e.TarifId, e.GroupeId }).HasName("PK__TarifsGr__425F0DE2907FCD83");

                entity.Property(e => e.TarifId).HasColumnName("TarifID");
                entity.Property(e => e.GroupeId).HasColumnName("GroupeID");
                entity.Property(e => e.Prix).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.Groupe).WithMany(p => p.TarifsGroupes)
                    .HasForeignKey(d => d.GroupeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TarifsGro__Group__52593CB8");

                entity.HasOne(d => d.Tarif).WithMany(p => p.TarifsGroupes)
                    .HasForeignKey(d => d.TarifId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TarifsGro__Tarif__5165187F");
            });

            modelBuilder.Entity<TarifsSpectacle>(entity =>
            {
                entity.HasKey(e => new { e.TarifId, e.SpectacleId }).HasName("PK__TarifsSp__12CAC99767A5BAE6");

                entity.Property(e => e.TarifId).HasColumnName("TarifID");
                entity.Property(e => e.SpectacleId).HasColumnName("SpectacleID");
                entity.Property(e => e.Prix).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.Spectacle).WithMany(p => p.TarifsSpectacles)
                    .HasForeignKey(d => d.SpectacleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TarifsSpe__Spect__3E52440B");

                entity.HasOne(d => d.Tarif).WithMany(p => p.TarifsSpectacles)
                    .HasForeignKey(d => d.TarifId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TarifsSpe__Tarif__3D5E1FD2");
            });

            modelBuilder.Entity<TypesTarif>(entity =>
            {
                entity.HasKey(e => e.TarifId).HasName("PK__TypesTar__57971C5133704716");

                entity.Property(e => e.TarifId).HasColumnName("TarifID");
                entity.Property(e => e.NomTarif)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}