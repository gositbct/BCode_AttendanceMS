using MauiApp1.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using static MauiApp1.Models.AttendanceModel;

namespace MauiApp1.ViewModels
{
    public class StudentProfileMethod : IQueryAttributable, INotifyPropertyChanged
    {

        //VIBE CODE MAALAAALAA
        private DatabaseService _db;
        public ICommand DeleteStudentCommand { get; }

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

        // The barcode content (== student ID) for the "reprint" section.
        private string _barcode;
        public string Barcode
        {
            get => _barcode;
            set { _barcode = value; OnPropertyChanged(nameof(Barcode)); }
        }

        public ObservableCollection<Attendance> AttendanceList { get; set; } = new();

        public ICommand BackToMasterlistNav { get; }

        public StudentProfileMethod()
        {
            _db = new DatabaseService();

            DeleteStudentCommand = new Command(async () => await DeleteStudent());
            BackToMasterlistNav = new Command(async () => await GoBack());

        }

        
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var studentId = query["studentId"].ToString();

            await _db.Init();

           
            var students = await _db.GetStudents();
            var selected = students.FirstOrDefault(s => s.Id == studentId);

            if (selected != null)
            {
                Name = selected.Name;
                Id = selected.Id;
                Block = selected.Block;
                Barcode = selected.Barcode;
            }

           
            var attendance = await _db.GetAttendance();
            var filtered = attendance.Where(a => a.StudentId == studentId)
                                      .OrderByDescending(a => a.Date)
                                      .ThenByDescending(a => a.TimeIn);

            AttendanceList.Clear();
            foreach (var a in filtered)
                AttendanceList.Add(a);
        }

        private async Task DeleteStudent()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm",
                "Are you sure you want to delete this student?",
                "Yes",
                "No"
            );

            if (!confirm)
                return;

            try
            {
                await _db.DeleteStudentWithAttendance(Id);

                await Application.Current.MainPage.DisplayAlert("Deleted", "Student removed", "OK");

                await Shell.Current.GoToAsync(".."); // back to masterlist
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not delete student: {ex.Message}", "OK");
            }
        }


        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}