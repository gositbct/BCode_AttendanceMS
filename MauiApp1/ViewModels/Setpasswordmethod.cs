using MauiApp1.Services;
using System.Windows.Input;

namespace MauiApp1.ViewModels
{
    public class SetPasswordMethod : IQueryAttributable
    {
        private readonly DatabaseService _db;
        private string? _studentId;
        private string? _adminUsername;

        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand SaveCommand { get; }

        public SetPasswordMethod()
        {
            _db = new DatabaseService();
            SaveCommand = new Command(async () => await Save());
        }

        // Receives either ?studentId=... (from LoginPageMethod.LoginAsStudent)
        // or ?adminUsername=... (from LoginPageMethod.LoginAsAdmin).
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("studentId"))
                _studentId = query["studentId"].ToString();

            if (query.ContainsKey("adminUsername"))
                _adminUsername = query["adminUsername"].ToString();
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 4)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Password must be at least 4 characters.", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if (!string.IsNullOrEmpty(_studentId))
            {
                await SaveStudentPassword();
            }
            else if (!string.IsNullOrEmpty(_adminUsername))
            {
                await SaveAdminPassword();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No account specified.", "OK");
            }
        }

        //METHOD NA NAG TATAKE NG INPUT PAPUNTANG  DB STUDENT PASS TO
        private async Task SaveStudentPassword()
        {
            var student = await _db.GetStudentById(_studentId);
            if (student == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Student record not found.", "OK");
                return;
            }

            student.Password = PasswordHasher.Hash(NewPassword);
            await _db.UpdateStudent(student);

            SessionService.CurrentStudentId = student.Id;

            await Application.Current.MainPage.DisplayAlert("Success", "Password set. You're logged in.", "OK");
            await Shell.Current.GoToAsync("///StudentDashboard");
        }

        //METHOD NA NAG TATAKE NG INPUT PAPUNTANG  DB ADMIN PASS TO
        private async Task SaveAdminPassword()
        {
            var admin = await _db.GetAdminByUsername(_adminUsername);
            if (admin == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Admin record not found.", "OK");
                return;
            }

            admin.Password = PasswordHasher.Hash(NewPassword);
            await _db.UpdateAdmin(admin);

            SessionService.CurrentAdminUsername = admin.Username;

            await Application.Current.MainPage.DisplayAlert("Success", "Password set. You're logged in.", "OK");
            await Shell.Current.GoToAsync("///AdminDashboard");
        }
    }
}
