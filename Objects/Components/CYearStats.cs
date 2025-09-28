using GameTimeX.Objects.baseClass;

namespace GameTimeX.Objects.Components
{
    public class CYearStats : GTXComponent<CYearStats>
    {

        public bool Shown { get; set; } = false;
        public string Year { get; set; } = string.Empty;

        public CYearStats() : base() { }

        public CYearStats(string rawValue) : base(rawValue) { }
    }
}
