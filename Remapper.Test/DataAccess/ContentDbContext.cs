using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remapper.Test.DataAccess
{
    class ContentDbContext : DbContext
    {
        public DbSet<ContentItemDTO> ContentItemDTOs { get; set; }

        public DbSet<BlogPostDTO> BlogPostDTOs { get; set; }

        public DbSet<ProductReviewDTO> ProductReviewDTOs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.Mode = SqliteOpenMode.Memory;

            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            optionsBuilder.UseSqlite(connection);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
