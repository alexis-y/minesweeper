using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading.Tasks;

namespace Minesweeper.Model
{
    public class MinesweeperContext : IdentityDbContext, IPersistedGrantDbContext
    {
        public MinesweeperContext(DbContextOptions options, OperationalStoreOptions storeOptions) : base(options) {
            StoreOptions = storeOptions;
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ConfigurePersistedGrantContext(StoreOptions);
        }

        private OperationalStoreOptions StoreOptions { get; }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }
    }
}
