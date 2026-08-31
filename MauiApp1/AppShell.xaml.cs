using MauiApp1.Views;
using MauiApp1.Views.AdminPages;
using MauiApp1.Views.StudentPages;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("loginpage", typeof(LoginPage));
            Routing.RegisterRoute("studentdashboard", typeof(StudentDashboard));
            Routing.RegisterRoute("attendancehistory", typeof(AttendanceHistory));
            Routing.RegisterRoute("setpassword", typeof(SetPassword));
            Routing.RegisterRoute("myprofile", typeof(MyProfile));

            Routing.RegisterRoute("admindashboard", typeof(AdminDashboard));
            Routing.RegisterRoute("addstudent", typeof(AddStudent));

            Routing.RegisterRoute("masterlist", typeof(Masterlist));
            Routing.RegisterRoute("studentprofile", typeof(StudentProfile));
            Routing.RegisterRoute("adminscanner", typeof(AdminScanner));
        }
    }
}
