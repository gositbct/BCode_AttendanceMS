using MauiApp1.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MauiApp1.ViewModels
{
    internal class LoginPageMethod
    {
       //DATA FIELDS TO ITO YUNG KINUKUHA SA INPUT
        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsStudent { get; set; }
        public bool IsAdmin { get; set; }

        public ICommand LoginCommand { get; }

        private readonly DatabaseService _db;

        public LoginPageMethod()
        {
            _db = new DatabaseService();
            LoginCommand = new Command(async () => await Login());
        }

        //ITO YUNG NAGHAHANDLE NG LOGIN KASAMA RADIO BUTTON
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Enter your ID/username.", "OK");
                return;
            }

            if (IsStudent)
            {
                await SafeLogin(LoginAsStudent);
            }
            else if (IsAdmin)
            {
                await SafeLogin(LoginAsAdmin);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Select a role", "OK");
            }
        }

        private async Task SafeLogin(Func<Task> loginAction)
        {
            try
            {
                await loginAction();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }
        }

        //LOGIN FOR STUDENT KASAMA ROUTING
        private async Task LoginAsStudent()
        {
            var student = await _db.GetStudentById(Username.Trim());

            if (student == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No student found with that ID.", "OK");
                return;
            }

            // First login ever (Add Student leaves Password null) -> force set-password.
            if (string.IsNullOrEmpty(student.Password))
            {
                await Shell.Current.GoToAsync($"///SetPassword?studentId={student.Id}");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Enter your password.", "OK");
                return;
            }

            string enteredHash = PasswordHasher.Hash(Password);
            if (enteredHash != student.Password)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Incorrect password.", "OK");
                return;
            }

            SessionService.CurrentStudentId = student.Id;
            await Shell.Current.GoToAsync("///StudentDashboard");
        }

        //LOGIN AS ADMIN KASAMA DIN ROUTING 
        private async Task LoginAsAdmin()
        {
            var admin = await _db.GetAdminByUsername(Username.Trim());

            if (admin == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid admin username.", "OK");
                return;
            }

            // First login ever (newly added admin) -> force set-password.
            if (string.IsNullOrEmpty(admin.Password))
            {
                await Shell.Current.GoToAsync($"///SetPassword?adminUsername={admin.Username}");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Enter your password.", "OK");
                return;
            }

            //SA PASSWORD TO AUTH
            string enteredHash = PasswordHasher.Hash(Password);
            if (enteredHash != admin.Password)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Incorrect password.", "OK");
                return;
            }

            SessionService.CurrentAdminUsername = admin.Username;
            await Shell.Current.GoToAsync("///AdminDashboard");
        }
    }
}