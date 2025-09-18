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
        
        public enum ViewModes
        {
            LIST,
            TILES
        }

        public BackupTypes BackupType { get; set; }
        public bool AutoBackup { get; set; }

        public string BackupPath { get; set; } = string.Empty;   
        public string BackUpImportPath {  get; set; } = string.Empty;

        public bool SessionGameTime { get; set; } = false;
        public KeyInput.VirtualKey MonitorShortcut { get; set; }
        public bool MonitorShortcutActive { get; set; }
        public bool ShowToastNotification { get; set; } = false;

        public ViewModes ViewMode { get; set; } = ViewModes.TILES;

        public bool AutoProfileSwitching { get; set; } = false;

        public bool BlackOutShortcutActive { get; set; } = false;
    }
}
