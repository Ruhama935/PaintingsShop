using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class DatabaseFixture : IDisposable
    {
        public PaintingsShopContext Context { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<PaintingsShopContext>()
                .UseSqlServer("Data Source=LAP14520;Initial Catalog=PaintingsShopTests;Integrated Security=True;Encrypt=False")
                .Options;

            Context = new PaintingsShopContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
