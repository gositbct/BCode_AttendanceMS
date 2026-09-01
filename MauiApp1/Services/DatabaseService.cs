using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MauiApp1.Models;
using static MauiApp1.Models.AttendanceModel;
using static MauiApp1.Models.StudentModel;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace MauiApp1.Services
{

    // nag hahandle ng time in time out sa ano rin to status
    
    public enum AttendanceResult
    {
        TimeIn,
        TimeOut,
        AlreadyCompleted 
    }

    
    //dito naka initialize mga table
    
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;


        //INITIALIZAION TO NG MGA COMMAND BASTA YUNG TINATAWAG SA CONSTRUCTOR PARA MAG FUNCTION

        public async Task Init()
        {
            if (_db != null) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "attendance.db");


            Console.WriteLine($"DATABASE PATH: {dbPath}");

            _db = new SQLiteAsyncConnection(dbPath);
            if (File.Exists(dbPath))
            {
                Process.Start("explorer.exe", $"/select,\"{dbPath}\"");
            }

            await _db.CreateTableAsync<Student>();
            await _db.CreateTableAsync<Attendance>();
            await _db.CreateTableAsync<Admin>();

            await SeedDefaultAdmin();
    
        }

        //default acc ng admin eto yung admin tas admin123
        private async Task SeedDefaultAdmin()
        {
            var existing = await _db.Table<Admin>().FirstOrDefaultAsync();
            if (existing == null)
            {
                await _db.InsertAsync(new Admin
                {
                    Username = "admin",
                    Password = PasswordHasher.Hash("admin123")
                });
            }
        }

        public async Task AddStudent(Student student)
        {
            await _db.InsertAsync(student);
        }

        /* Generates a random, unique 4-digit ID for the current year, e.g.
         "2026-0472". This same string is reused as the barcode content, so
         a student's ID card and their login username are always identical.
         Randomized (rather than sequential max+1) so IDs don't reveal
         enrollment order and don't keep climbing forever as students are
         deleted - it retries on collision until a free number is found.*/


        //UNIQUE ID GENERATOR TO KADA NAG AADD NG ACC
        public async Task<string> GenerateNextStudentId()
        {
            await Init();

            string yearPrefix = DateTime.Now.Year.ToString();

            var existingIds = (await _db.Table<Student>().ToListAsync())
                .Select(s => s.Id)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToHashSet();

            var random = new Random();
            string candidate;
            int attempts = 0;
            const int maxAttempts = 10000; // 4-digit space only has 10,000 values

            do
            {
                int sequence = random.Next(0, 10000); // 0000-9999
                candidate = $"{yearPrefix}-{sequence:D4}";
                attempts++;

                if (attempts >= maxAttempts)
                    throw new InvalidOperationException("No more unique student IDs available for this year.");

            } while (existingIds.Contains(candidate));

            return candidate;
        }

        public async Task AddAdmin(Admin admin)
        {
            await _db.InsertAsync(admin);
        }

        /* Generates a random, unique admin username, e.g. "ADM-0472". Admins
         have no barcode, so this only needs to be unique, not tied to a
         year like the student ID. Randomized for the same reason as
         GenerateNextStudentId - no predictable ordering, and deleting an
         admin doesn't make numbers keep climbing.*/

        // SAME THING PERO PANG ADMIN
        public async Task<string> GenerateNextAdminId()
        {
            await Init();

            var existingUsernames = (await _db.Table<Admin>().ToListAsync())
                .Select(a => a.Username)
                .Where(u => !string.IsNullOrEmpty(u))
                .ToHashSet();

            var random = new Random();
            string candidate;
            int attempts = 0;
            const int maxAttempts = 10000; // 4-digit space only has 10,000 values

            do
            {
                int sequence = random.Next(0, 10000); // 0000-9999
                candidate = $"ADM-{sequence:D4}";
                attempts++;

                if (attempts >= maxAttempts)
                    throw new InvalidOperationException("No more unique admin usernames available.");

            } while (existingUsernames.Contains(candidate));

            return candidate;
        }

        //ETO YUNG MGA TINATAWAG SA CONSTRUCTOR PARANG MGA CLASS METHOD

        public async Task<List<Student>> GetStudents()
        {
            return await _db.Table<Student>().ToListAsync();
        }

        public async Task AddAttendance(Attendance record)
        {
            await _db.InsertAsync(record);
        }

        public async Task<List<Attendance>> GetAttendance()
        {
            return await _db.Table<Attendance>().ToListAsync();
        }


        public async Task<AttendanceResult> HandleAttendance(string studentId)
        {
            await Init();

            var today = DateTime.Today;

            var existing = await _db.Table<Attendance>()
                .Where(a => a.StudentId == studentId && a.Date == today)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await _db.InsertAsync(new Attendance
                {
                    StudentId = studentId,
                    TimeIn = DateTime.Now,
                    Date = today
                });
                return AttendanceResult.TimeIn;
            }
            else if (existing.TimeOut == default)
            {
                existing.TimeOut = DateTime.Now;
                await _db.UpdateAsync(existing);
                return AttendanceResult.TimeOut;
            }
            else
            {
                return AttendanceResult.AlreadyCompleted;
            }
        }

        /* Returns today's attendance record for a student, or null if they
         haven't timed in yet today. Used to drive the status bar on the
         student dashboard (Not Timed In / Timed In / Timed Out).*/
        public async Task<Attendance?> GetTodayAttendanceForStudent(string studentId)
        {
            await Init();

            var today = DateTime.Today;

            return await _db.Table<Attendance>()
                .Where(a => a.StudentId == studentId && a.Date == today)
                .FirstOrDefaultAsync();
        }

        public async Task<Student?> GetStudentById(string studentId)
        {
            await Init();
            return await _db.Table<Student>()
                             .Where(s => s.Id == studentId)
                             .FirstOrDefaultAsync();
        }

        public async Task UpdateStudent(Student student)
        {
            await Init();
            await _db.UpdateAsync(student);
        }

        public async Task<Admin?> GetAdminByUsername(string username)
        {
            await Init();
            return await _db.Table<Admin>()
                             .Where(a => a.Username == username)
                             .FirstOrDefaultAsync();
        }

        public async Task UpdateAdmin(Admin admin)
        {
            await Init();
            await _db.UpdateAsync(admin);
        }

        public async Task DeleteStudent(string studentId)
        {
            await Init();

            var student = await _db.FindAsync<Student>(studentId);

            if (student != null)
            {
                await _db.DeleteAsync(student);
            }
        }

        public async Task DeleteStudentWithAttendance(string studentId)
        {
            await Init();

            var student = await _db.FindAsync<Student>(studentId);

            if (student != null)
                await _db.DeleteAsync(student);

            var records = await _db.Table<Attendance>()
                                   .Where(a => a.StudentId == studentId)
                                   .ToListAsync();

            foreach (var r in records)
                await _db.DeleteAsync(r);
        }
    }

}