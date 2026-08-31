using MauiApp1.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using static MauiApp1.Models.StudentModel;
using AdminModel = MauiApp1.Models.Admin;

namespace MauiApp1.ViewModels
{
    public class AddStudentMethod : INotifyPropertyChanged //ADD STUDENT NG ADMIN KADA ADD NAG GEGENERATE NARIN NG ID AT BARCODE, ID LANG PAG ADMIN
    {
       
        private string _generatedId;
        public string GeneratedId
        {
            get => _generatedId;
            set
            {
                _generatedId = value;
                OnPropertyChanged(nameof(GeneratedId));
                OnPropertyChanged(nameof(HasGeneratedId));
            }
        }

        // True once a record has been saved this session, so the barcode
        // view (which needs a non-empty Value) only renders after that.
        public bool HasGeneratedId => !string.IsNullOrEmpty(GeneratedId);

        // Admins have no barcode, so the barcode view should never show for them,
        // even if GeneratedId is set from a student added earlier this session.
        public bool ShowBarcode => HasGeneratedId && IsStudentRole;

        // Role toggle: which kind of account is being created.
        // Defaults to Student since that's the far more common case.
        private bool _isStudentRole = true;
        public bool IsStudentRole
        {
            get => _isStudentRole;
            set
            {
                _isStudentRole = value;
                OnPropertyChanged(nameof(IsStudentRole));
                OnPropertyChanged(nameof(IsAdminRole));
                OnPropertyChanged(nameof(ShowBarcode));
            }
        }

        public bool IsAdminRole
        {
            get => !_isStudentRole;
            set
            {
                IsStudentRole = !value;
            }
        }

        public string Name { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string Block { get; set; }

        public ICommand DashboardNav { get; }
        public ICommand SaveCommand { get; }
        private DatabaseService _db;

        public AddStudentMethod()  //SAME THING CONSTRUCTOR DITO TINATAWAG MGA PUBLIC COMMAND NA NAKACONNECT SA IBANG PAGE AT FILE
        {
            _db = new DatabaseService();
            SaveCommand = new Command(async () => await Save());

            DashboardNav = new Command(async () => await GoToDashboard());
        }

        //SAVE BUTTON OR FUNCTION
        private async Task Save()
        {
            if (IsStudentRole)
                await SaveStudent();
            else
                await SaveAdmin();
        }

        //ITO YUNG TINATAWAG SA SAVE PAG STUDENT NAGLALAGAY NG DATABASE SA STUDENT DB
        private async Task SaveStudent()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Application.Current.MainPage.DisplayAlert("Missing info", "Name is required.", "OK");
                return;
            }

            if (Age <= 0 || Age > 120)
            {
                await Application.Current.MainPage.DisplayAlert("Invalid info", "Enter a valid age.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Sex))
            {
                await Application.Current.MainPage.DisplayAlert("Missing info", "Sex is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Block))
            {
                await Application.Current.MainPage.DisplayAlert("Missing info", "Block is required.", "OK");
                return;
            }

            try
            {
                await _db.Init();

                string newId = await _db.GenerateNextStudentId();

                await _db.AddStudent(new Student
                {
                    Id = newId,
                    Name = Name,
                    Age = Age,
                    Sex = Sex,
                    Block = Block,
                    Barcode = newId,   // barcode content = student ID
                    Password = null    // forces "set password" on first login
                });

                GeneratedId = newId;

                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    $"Student added.\nGenerated ID / Username: {newId}",
                    "OK");

                ClearForm();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not save student: {ex.Message}", "OK");
            }
        }

        //ITO YUNG TINATAWAG SA SAVE PAG ADMIN NAGLALAGAY NG DATABASE SA ADMIN
        private async Task SaveAdmin()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Application.Current.MainPage.DisplayAlert("Missing info", "Name is required.", "OK");
                return;
            }

            
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm",
                $"Create a new ADMIN account for \"{Name}\"? This grants full admin access.",
                "Yes",
                "No"
            );

            if (!confirm)
                return;

            try
            {
                await _db.Init();

                string newUsername = await _db.GenerateNextAdminId();

                await _db.AddAdmin(new AdminModel
                {
                    Username = newUsername,
                    Password = null // forces "set password" on first login
                });

                GeneratedId = newUsername;

                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    $"Admin added.\nGenerated Username: {newUsername}",
                    "OK");

                ClearForm();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not create admin: {ex.Message}", "OK");
            }
        }

        // Clears the form for the next entry, but keeps GeneratedId so the
        // page can still show/print the last student's barcode (or display
        // the last admin's username).
        private void ClearForm()
        {
            Name = string.Empty;
            Age = 0;
            Sex = string.Empty;
            Block = string.Empty;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Age));
            OnPropertyChanged(nameof(Sex));
            OnPropertyChanged(nameof(Block));
        }

        //BUTTON L COMMAND LANG TO SA ROUTING
        private async Task GoToDashboard()
        {
            await Shell.Current.GoToAsync("///AdminDashboard");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
