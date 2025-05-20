// BufeContext.cs javított változat
using BufeBackEnd.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BufeBackEnd
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class BufeContext : DbContext
    {
        public BufeContext() : base("name=BufeContext") { }

        public virtual DbSet<Dolgozo> Dolgozok { get; set; }
        public virtual DbSet<Kategoria> Kategoriak { get; set; }
        public virtual DbSet<Kosar> Kosarak { get; set; }
        public virtual DbSet<Kosarba> KosarbaTetelek { get; set; }
        public virtual DbSet<Szamla> Szamlak { get; set; }
        public virtual DbSet<Termek> Termekek { get; set; }
        public virtual DbSet<Vasarlo> Vasarlok { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kosar>()
                .HasRequired(k => k.Vasarlo)
                .WithMany(v => v.Kosarak)
                .HasForeignKey(k => k.Email)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Kosar>()
                .HasRequired(k => k.Dolgozo)
                .WithMany()
                .HasForeignKey(k => k.Felnev)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Kosarba>()
                .HasKey(k => new { k.Kkod, k.Tid });

            modelBuilder.Entity<Kosarba>()
                .HasRequired(k => k.Kosar)
                .WithMany(k => k.KosarbaTetelek)
                .HasForeignKey(k => k.Kkod)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Kosarba>()
                .HasRequired(k => k.Termek)
                .WithMany()
                .HasForeignKey(k => k.Tid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Szamla>()
                .HasRequired(s => s.Vasarlo)
                .WithMany(v => v.Szamlak)
                .HasForeignKey(s => s.Email)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Termek>()
                .HasRequired(t => t.Kategoria)
                .WithMany(k => k.Termekek)
                .HasForeignKey(t => t.Kid)
                .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
