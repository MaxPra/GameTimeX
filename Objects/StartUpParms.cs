using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTimeX.Objects
{
    class StartUpParms
    {

        public enum BackupTypes
        {
            NO_BACKUP = 0,
            CREATE_BACKUP = 1,
            IMPORT_BACKUP = 2
        }

        public BackupTypes BackupType { get; set; }
        public string BackupPath { get; set; } = string.Empty;   
     
        public string BackUpImportPath {  get; set; } = string.Empty;  
    }
}
