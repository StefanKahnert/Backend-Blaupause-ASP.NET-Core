
using Backend_Blaupause.Models.DatabaseMigration;
using Backend_Blaupause.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;


namespace Backend_Blaupause.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        public DbSet<User> user { get; set; }
       
        public DbSet<Permission> permissions { get; set; }

        public DbSet<DatabaseVersion> databaseVersion { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {



            builder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(e => new { e.userId, e.permissionId }).HasName("pk_user_permission");

                entity.ToTable("user_permission");

                entity.Property(e => e.userId).HasColumnName("id_user");

                entity.Property(e => e.permissionId).HasColumnName("id_permission");

                entity.HasOne(d => d.user)
                    .WithMany(p => p.userPermissions)
                    .HasForeignKey(d => d.userId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user");

                entity.HasOne(d => d.permission)
                    .WithMany(p => p.userPermissions)
                    .HasForeignKey(d => d.permissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_permission");
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
