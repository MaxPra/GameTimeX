using GameTimeX.Objects.baseClass;

namespace GameTimeX.Objects.Components
{
    internal class CHappyNewYearEvent : GTXComponent<CHappyNewYearEvent>
    {
        public bool Shown { get; set; } = false;
        public string Year { get; set; } = string.Empty;

        public CHappyNewYearEvent() : base() { }

        public CHappyNewYearEvent(string rawValue) : base(rawValue) { }


    }
}
