using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models.DatabaseMigration
{
    public class DatabaseUpdates
    {
        private static readonly Dictionary<long, string> updates = new Dictionary<long, string>
            {
                {2,"INSERT INTO permission(id, name) VALUES(1, 'ADMINISTRATOR');"},
                {3,"INSERT INTO permission(id, name) VALUES(2, 'USER');"},
                {4,"INSERT INTO \"user\" (id, username, first_name, last_name, password, active, hidden) VALUES(1, 'ADMIN', 'SYS', 'ADMIN', " +
                    "'c7ad44cbad762a5da0a452f9e854fdc1e0e7a52a38015f23f3eab1d80b931dd472634dfac71cd34ebc35d16ab7fb8a90c81f975113d6c7538dc69dd8de9077ec', " + // Password = admin
                    "true, false );"},
                {5, "ALTER TABLE \"user\" ADD COLUMN token text; "},
                {6, "INSERT INTO user_permission(id_user, id_permission) VALUES(1,2);" }
            };

        public static string getUpdates(long dbVersion)
        {
           string query = "";
           updates.Where(update => update.Key > dbVersion).Select(update => update.Value).ToList().ForEach(value => query += (" " + value));
           return query;
        }

        public static long getNewDbVersion()
        {
            if(updates.Keys.Count == 0)
            {
                return 1;
            } else
            {
                return updates.Keys.Max();
            }
            
        }

    }
}
