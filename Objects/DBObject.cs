using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTimeX
{
    public class DBObject
    {
        private string gameName;
        private long gameTime;
        private DateTime firstPlay;
        private DateTime lastPlay;
        private string profilePicFileName;
        private DateTime createdAt;
        private DateTime changedAt;
        private int profileID;
        private string extGameFolder;

        public string GameName { get => gameName; set => gameName = value; }
        public long GameTime { get => gameTime; set => gameTime = value; }
        public DateTime FirstPlay { get => firstPlay; set => firstPlay = value; }
        public DateTime LastPlay { get => lastPlay; set => lastPlay = value; }
        public string ProfilePicFileName { get => profilePicFileName; set => profilePicFileName = value; }
        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
        public DateTime ChangedAt { get => changedAt; set => changedAt = value; }
        public int ProfileID { get => profileID; set => profileID = value; }
        public string ExtGameFolder { get => extGameFolder; set => extGameFolder = value; }
    }
}
