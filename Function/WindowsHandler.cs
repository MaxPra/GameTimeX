using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTimeX.Function
{
    internal class WindowsHandler
    {
        public static void EnableHDR()
        {
            // HDR aktivieren
            HdrToggler.SetHdrForAllActiveDisplays(true);
        }
    }
}
