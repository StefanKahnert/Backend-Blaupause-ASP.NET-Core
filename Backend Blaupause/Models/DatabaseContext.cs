using Backend_Blaupause.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;


namespace Backend_Blaupause.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        public DbSet<User> User { get; set; }
       
        public DbSet<Permission> Permission { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("pk_user_role");

                entity.ToTable("UserRole");

                entity.Property(e => e.UserId).HasColumnName("UserId");

                entity.Property(e => e.RoleId).HasColumnName("RoleId");

            });

            builder.HasSequence("user_seq");
            

            base.OnModelCreating(builder);

        }

        public long getNextUserId()
        {
            return GetSequenceNextVal("user_seq");
        }

        

        private long GetSequenceNextVal(string sequence)
        {
            using (DbCommand cmd = Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT nextval('\"" + sequence + "\"');";
                Database.OpenConnection();
                DbDataReader reader = cmd.ExecuteReader();
                reader.Read();
                long newID = (long)reader.GetValue(0);
                Database.CloseConnection();
                return newID;
            }
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }
    }
}
