using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using OrtopediaDM;

namespace OrtopediaDM.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<OrtopediaDM.Marca> Marca { get; set; }
        public DbSet<OrtopediaDM.Seccion> Seccion { get; set; }
        public DbSet<OrtopediaDM.Tipo> Tipo { get; set; }
        public DbSet<OrtopediaDM.Producto> Producto { get; set; }
    }
}
