using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.Objects;
using GameTimeX.Objects;
using GameTimeX.Objects.Components;

namespace GameTimeX.XApplication.SubDisplays
{
    public partial class ManageExecutables : Window, INotifyPropertyChanged
    {
        private DBO_Profile dbo_Profile = null!;
        private CExecutables cExecutables = null!;

        public ObservableCollection<Executable> Executables { get; } = new();

        private bool _isAllChecked;
        public bool IsAllChecked
        {
            get => _isAllChecked;
            set
            {
                if (_isAllChecked == value) return;
                _isAllChecked = value;
                OnPropertyChanged();
                foreach (var e in Executables) e.IsActive = value;
            }
        }

        public ManageExecutables(int pid)
        {
            InitializeComponent();

            dbo_Profile = DM_Profile.ReadPID(pid);
            cExecutables = new CExecutables(dbo_Profile.Executables).Dezerialize();

            if (cExecutables.KeyValuePairs == null)
                return;

            foreach (var kv in cExecutables.KeyValuePairs)
                Executables.Add(new Executable { Name = kv.Key, IsActive = kv.Value });

            Executables.CollectionChanged += Executables_CollectionChanged;
            foreach (var e in Executables) e.PropertyChanged += Item_PropertyChanged;

            UpdateIsAllChecked();

            DataContext = this;
        }

        private void Executables_CollectionChanged(object? s, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Executable it in e.NewItems) it.PropertyChanged += Item_PropertyChanged;
            if (e.OldItems != null)
                foreach (Executable it in e.OldItems) it.PropertyChanged -= Item_PropertyChanged;
            UpdateIsAllChecked();
        }

        private void Item_PropertyChanged(object? s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Executable.IsActive))
                UpdateIsAllChecked();
        }

        private void UpdateIsAllChecked()
        {
            var all = Executables.Count > 0 && Executables.All(x => x.IsActive);
            if (_isAllChecked != all)
            {
                _isAllChecked = all;
                OnPropertyChanged(nameof(IsAllChecked));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            cExecutables.KeyValuePairs = Executables.ToDictionary(x => x.Name, x => x.IsActive);
            dbo_Profile.Executables = cExecutables.Serialize();
            DM_Profile.Save(dbo_Profile);
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));


        private static readonly string[] IgnoreSubstrings =
        {
            // Launcher / Starter
            "launcher","bootstrap","starter","client","host","kickoff",

            // Update / Patch / Install
            "update","updater","upgrade","patch","patcher",
            "install","installer","setup","maintenancetool",

            // Crash / Report / Error / Debug
            "crash","error","report","reporter","dumploader","dump",
            "diagnostic","logger","trace","debug","profiler","profiling",

            // Anti-Cheat
            "eac","easyanticheat","battleye","anticheat",
            "vac","punkbuster","fairfight","nprotect","xac","ggxx",

            // Stores / Plattformen
            "steam","epicgames","egs","origin","uplay","connect",
            "gog","galaxy","battlenet","blizzard","bethesda",
            "rockstar","rslauncher","riotclient","valorantclient",

            // Helper / Services / Tools
            "helper","service","daemon","tool","config","settings",
            "options","manager","overlay","telemetry","watchdog",
            "metrics","monitor","observer","notifier","autoconfig",

            // Benchmark / Test / QA
            "benchmark","stress","loadtest","qa","prototype",
            "unittest","staging","sandbox","testbuild",

            // Editor / SDK / Dev
            "editor","sdk","devtool","development","modtool",
            "builder","workshop","assettool","leveltool",

            // Konsole / CLI / Shell
            "console","shell","command","cmd","cli","powershell",

            // Netzwerk / Voice / Misc
            "voice","chat","server","dedicated","relay","proxy",
            "upnp","matchmaking","auth","login","connectivity",
            "p2p","multiplayer","browser"
        };
    }
}
