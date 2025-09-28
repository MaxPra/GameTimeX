using GameTimeX.Objects.baseClass;

namespace GameTimeX.Objects.Components
{
    internal class CHalloweenEvent : GTXComponent<CHalloweenEvent>
    {

        public bool Shown { get; set; } = false;
        public string Year { get; set; } = string.Empty;

        public CHalloweenEvent() : base() { }

        public CHalloweenEvent(string rawValue) : base(rawValue) { }
    }
}
