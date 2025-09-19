using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameTimeX.Objects
{
    public class Executable : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private bool _isActive;

        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        public bool IsActive
        {
            get => _isActive;
            set { if (_isActive != value) { _isActive = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
