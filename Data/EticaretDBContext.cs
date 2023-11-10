using EticaretProje.Models;
using Microsoft.EntityFrameworkCore;

namespace EticaretProje.Data
{
    public class EticaretDBContext : DbContext
    {
        //DbSet: veri seti: verileri tutmak için tablo sqlde oluştur
        //<ClassAdı> ... çalıştıgımız classın adı
        //Uruns :sql tarafında oluşacak olan tablonun Adını buraya yazıyoruz.

        public EticaretDBContext(DbContextOptions<EticaretDBContext> options)
     : base(options)
        {
        }


        public DbSet<Album> Albums { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Sepet> Sepets { get; set; }

    }
}
