using MauiApp1.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace MauiApp1.ViewModels
{
    
    public class MyProfileMethod : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _id;
        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        private string _block;
        public string Block
        {
            get => _block;
            set { _block = value; OnPropertyChanged(nameof(Block)); }
        }

        // The barcode always encodes the student's ID (same as StudentDashboard).
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

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand ChangePasswordCommand { get; }
        public ICommand BackToDashboardNav { get; }

        public MyProfileMethod()
        {
            _db = new DatabaseService();
            ChangePasswordCommand = new Command(async () => await ChangePassword());
            BackToDashboardNav = new Command(async () => await Shell.Current.GoToAsync("///StudentDashboard"));

            LoadProfile();
        }

        //SAME THING NAG LOLOAD NG DATA NG INDIVIDUAL STUDENT FROM DB
        private async void LoadProfile()
        {
            var studentId = SessionService.CurrentStudentId;
            if (string.IsNullOrEmpty(studentId))
                return;

            var student = await _db.GetStudentById(studentId);
            if (student == null)
                return;

            Name = student.Name;
            Id = student.Id;
            Block = student.Block;
            BarcodeValue = student.Barcode;
        }

        //CHANGE PASSWORD SA PROFILE
        private async Task ChangePassword()
        {
            var studentId = SessionService.CurrentStudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You are not logged in.", "OK");
                return;
            }

            var student = await _db.GetStudentById(studentId);
            if (student == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Student record not found.", "OK");
                return;
            }

            // Verify the current password before allowing a change.
            if (PasswordHasher.Hash(CurrentPassword ?? string.Empty) != student.Password)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Current password is incorrect.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 4)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "New password must be at least 4 characters.", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "New passwords do not match.", "OK");
                return;
            }

            try
            {
                student.Password = PasswordHasher.Hash(NewPassword);
                await _db.UpdateStudent(student);

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                OnPropertyChanged(nameof(CurrentPassword));
                OnPropertyChanged(nameof(NewPassword));
                OnPropertyChanged(nameof(ConfirmPassword));

                await Application.Current.MainPage.DisplayAlert("Success", "Password changed.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not change password: {ex.Message}", "OK");
            }
        }

        //SA DISPLAY LANG DIN NAG UUPDATE
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}