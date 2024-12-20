using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameTimeX
{
    internal class GameTimeXImage : Image
    {
        private int pid;

        public int Pid { get => pid; set => pid = value; }
    }
}
