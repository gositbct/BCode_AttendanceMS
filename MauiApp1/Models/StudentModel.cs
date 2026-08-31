using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Models
{
    public class StudentModel
    {
        public class Student
        {
            [PrimaryKey]
            public string Id { get; set; }

            public string Name { get; set; }
            public int Age { get; set; }
            public string Sex { get; set; }

            public string Barcode { get; set; }
            public string Block { get; set; }

            public string? Password { get; set; }

        }
    }
}
