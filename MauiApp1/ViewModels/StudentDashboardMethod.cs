using MauiApp1.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace MauiApp1.ViewModels
{
    public class StudentDashboardMethod : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        // Drives the status bar: what state today's attendance is in.
        private string _statusText = "Not Timed In";
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(nameof(StatusText)); }
        }

        // Hex color string bound to the status bar's background.
        private string _statusColor = "#9E9E9E"; // gray = not timed in
        public string StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(nameof(StatusColor)); }
        }

        // The barcode always encodes the student's ID (set that way when the
        // student was enlisted - see AddStudentMethod.SaveStudent).
        private string _barcodeValue;
        public string BarcodeValue
        {
            get => _barcodeValue;
            set
            {
                _barcodeValue = value;
                OnPropertyChanged(nameof(BarcodeValue));
                OnPropertyChanged(nameof(HasBarcodeValue));
            }
        }

        public bool HasBarcodeValue => !string.IsNullOrEmpty(BarcodeValue);

        //COMMANDS FOR ROUTING NA BINABIND SA BUTTONS
        public ICommand AttendanceHistoryNav { get; }
        public ICommand MyProfileNav { get; }
        public ICommand LogOutNav { get; }


        //CONSTRUCTOR 
        public StudentDashboardMethod()
        {
            _db = new DatabaseService();

            AttendanceHistoryNav = new Command(async () => await AttendanceHistory());
            MyProfileNav = new Command(async () => await MyProfile());
            LogOutNav = new Command(async () => await LogOut());

            LoadStudent();
        }


        //NAGLOLOAD NG REAL DATA FROM DB
        private async void LoadStudent()
        {
            var studentId = SessionService.CurrentStudentId;
            if (string.IsNullOrEmpty(studentId))
                return;

            var student = await _db.GetStudentById(studentId);
            if (student == null)
                return;

            Name = student.Name;
            BarcodeValue = student.Barcode;

            await RefreshStatus();
        }

        // Public so the page's OnAppearing can call this after returning
        // from the scanner having timed in/out, to keep the status current.
        public async Task RefreshStatus()
        {
            var studentId = SessionService.CurrentStudentId;
            if (string.IsNullOrEmpty(studentId))
                return;

            var today = await _db.GetTodayAttendanceForStudent(studentId);

            if (today == null)
            {
                StatusText = "Not Timed In";
                StatusColor = "#9E9E9E"; // gray
            }
            else if (today.TimeOut == default)
            {
                StatusText = "Timed In";
                StatusColor = "#27AE60"; // green
            }
            else
            {
                StatusText = "Timed Out";
                StatusColor = "#2F80ED"; // blue
            }
        }

        //ROUTING NG BUTTON
        private async Task AttendanceHistory()
        {
            await Shell.Current.GoToAsync("///AttendanceHistory");
        }

        private async Task MyProfile()
        {
            await Shell.Current.GoToAsync("///MyProfile");
        }

        private async Task LogOut()
        {
            SessionService.ClearSession();
            await Shell.Current.GoToAsync("///LoginPage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}