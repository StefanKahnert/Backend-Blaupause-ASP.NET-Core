using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models.DatabaseMigration
{
    [Table("database_version")]
    public class DatabaseVersion
    {
        public DatabaseVersion()
        {

        }

        [Key, Required]
        public long version { get; set; }
    }
}
