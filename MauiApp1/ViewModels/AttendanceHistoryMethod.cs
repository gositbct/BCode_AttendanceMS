using MauiApp1.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using static MauiApp1.Models.AttendanceModel;
using static MauiApp1.Models.StudentModel;

namespace MauiApp1.ViewModels
{
    public class AttendanceHistoryMethod
    {
        public ObservableCollection<Attendance> AttendanceList { get; set; } = new();//ITO NAG COCOLECT NG INFO NA IDIDISPLAY
        private DatabaseService _db; //DATABASE TO

        public ICommand GoBackNav { get; }


        //CONSTRUCTOR ULET 
        public AttendanceHistoryMethod()
        {
            _db = new DatabaseService();
            LoadData();

            GoBackNav = new Command(async () => await GoBack());

        }

        //ITO YUNG NAGTATAWAG NG REAL DATA FROM DB NA IDIDISPLAY
        private async void LoadData()
        {
            await _db.Init();

            var studentId = SessionService.CurrentStudentId;
            if (string.IsNullOrEmpty(studentId))
                return;

            var data = await _db.GetAttendance();
            var ownRecords = data.Where(a => a.StudentId == studentId)
                                  .OrderByDescending(a => a.Date)
                                  .ThenByDescending(a => a.TimeIn);

            AttendanceList.Clear();
            foreach (var a in ownRecords)
                AttendanceList.Add(a);
        }

        //WALA BUTTON LANG ULIT SA ROUTING
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("///StudentDashboard");
        }

    }

}
