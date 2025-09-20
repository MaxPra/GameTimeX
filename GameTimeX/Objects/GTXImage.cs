using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameTimeX.Objects
{
    internal class GTXImage : Image
    {
        public int PID { get; set; }
        public bool Selected { get; set; } = false;
        public MainWindow MainWnd { get; set; }

        public bool DoBorderEffect { get; set; } = true;
        public Border Border { get; set; }
       
    }
}
