using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models.DatabaseMigration
{
    public class DatabaseMigrationService
    {

        private readonly DatabaseContext db;

        public DatabaseMigrationService(DatabaseContext context)
        {
            this.db = context;
        }

        public bool dbVersionTableExists()
        {
            bool exists;

            try
            {
                db.DatabaseVersion.FirstOrDefault();
                exists = true;
            }
            catch
            {
                exists = false;
            }

            return exists;
        }

        public void updateDatabaseToCurrentVersion()
        {
            DatabaseVersion dbVersion;

            try
            {
                dbVersion = db.DatabaseVersion.FirstOrDefault();
            }
            catch
            {
                dbVersion = new DatabaseVersion();
                dbVersion.version = -1;
            }
            string query = ""; 

            if (dbVersion.version == -1)
            {
                query += DatabaseInitial.getInitialDatabase();
            }

            query += DatabaseUpdates.getUpdates(dbVersion.version);        
            
            if(query != "")
            {
                db.Database.ExecuteSqlRaw(query);
            }

            db.Database.ExecuteSqlRaw("Delete From public.database_version");
            
            dbVersion.version = DatabaseUpdates.getNewDbVersion();

            db.DatabaseVersion.Add(dbVersion);

            db.SaveChanges();
        }

    }
}
