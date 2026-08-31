using SQLite;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MauiApp1.Models
{
    public class AttendanceModel
    {
        public class Attendance
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public string StudentId { get; set; }

            public DateTime TimeIn { get; set; }
            public DateTime TimeOut { get; set; }

            public DateTime Date { get; set; }
        }
    }
}
