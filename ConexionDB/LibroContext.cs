using System.Data.Entity;


namespace ConexionDB
{
    public class LibroContext : DbContext
    {
        public DbSet<LIBRO> Libro { get; set; }

        public LibroContext() : base("name=LibreriaImaginaEntities") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LIBRO>()
                .ToTable("LIBRO");

            modelBuilder.Entity<LIBRO>()
                .HasKey(l => l.ID_LIBRO);
            modelBuilder.Entity<LIBRO>().Property(p => p.NOMBRE_LIBRO).IsRequired();
            modelBuilder.Entity<LIBRO>().Property(p => p.DESCRIPCION).IsRequired();
            modelBuilder.Entity<LIBRO>().Property(p => p.AUTOR).IsRequired();
            modelBuilder.Entity<LIBRO>().Property(p => p.EDITORIAL).IsRequired();
            modelBuilder.Entity<LIBRO>().Property(p => p.PRECIO_UNITARIO).IsRequired();
            modelBuilder.Entity<LIBRO>().Property(p => p.CANTIDAD_DISPONIBLE).IsRequired();
            modelBuilder.Entity<LIBRO>().Property(p => p.PORTADA).IsRequired();
            modelBuilder.Entity<LIBRO>().Property(p => p.FECHA_PUBLICACION).IsRequired();
            modelBuilder.Entity<LIBRO>().Property(p => p.CATEGORIA).IsRequired();
        }

    }
}
