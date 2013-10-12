using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Db.Model;

namespace Db.Data
{
    public class ExamContext : DbContext
    {
        //connection string
        //db.Model reference may be a problem

        public ExamContext()
            : base("ExamDB")
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Comments> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .Property(usr => usr.SessionKey)
                .IsFixedLength()
                .HasMaxLength(50);
            modelBuilder.Entity<Posts>()
                .Property(p => p.PostDate).IsOptional();
            modelBuilder.Entity<Comments>()
                .Property(c => c.PostDate).IsOptional();

            base.OnModelCreating(modelBuilder);
        }
    }
}
