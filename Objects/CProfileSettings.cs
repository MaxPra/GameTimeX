namespace GameTimeX.Objects
{
    internal class CProfileSettings : GTXComponent<CProfileSettings>
    {

        public bool HDREnabled { get; set; } = false;
        public string SteamGameArgs { get; set; } = string.Empty;

        public CProfileSettings() { }

        public CProfileSettings(string rawValue) : base(rawValue) { }

    }
}
