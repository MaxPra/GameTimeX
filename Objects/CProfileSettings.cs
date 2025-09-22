using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTimeX.Objects
{
    internal class CProfileSettings : GTXComponent<CProfileSettings>
    {

        public bool HDREnabled { get; set; } = false;

        public CProfileSettings() { HDREnabled = false; }

        public CProfileSettings(string rawValue) : base(rawValue) { }

    }
}
