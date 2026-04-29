using Microsoft.EntityFrameworkCore;
using xablauBack.Domain.Entities;

namespace xablauBack.Infrastructure.Data{

    public class AppDbContext : DbContext /* ponte do backend com o banco */
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Carrinho> Carrinhos { get; set; }
        public DbSet<ItemCarrinho> ItensCarrinho { get; set; }
    }
}