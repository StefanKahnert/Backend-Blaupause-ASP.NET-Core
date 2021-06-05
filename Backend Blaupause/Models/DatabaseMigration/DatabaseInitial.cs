using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models.DatabaseMigration
{
    public class DatabaseInitial
    {
        public static string getInitialDatabase()
        {
            string query = File.ReadAllText(Path.Combine("Models", "DatabaseMigration", "InitialDatabase.txt")).Replace("\n", "");

            return query;
        }
    }
}
