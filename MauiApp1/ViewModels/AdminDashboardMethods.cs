using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MauiApp1.ViewModels
{
    internal class AdminDashboardMethods
    {

        //DITO SA MGA ICOMMAND BINABIND MGA BUTTON
        public ICommand AddStudentNav { get; }

        public ICommand MasterlistNav { get; }
        public ICommand LogOutNav { get; }

        public ICommand ScannerNav { get; }

        //CONSTRUCTOR OR TUMATAWAG SA MGA BUTTON
        public AdminDashboardMethods()
        {
            AddStudentNav = new Command(async () => await AddStudent());
            ScannerNav = new Command(async () => await Scanner());
            MasterlistNav = new Command(async () => await Masterlist());
            LogOutNav = new Command(async () => await LogOut());
        }


        //ETONG MGA TO SA ROUTING KUNG SAN PAPUNTA BAWAT BUTTON
        private async Task AddStudent()
        {
            await Shell.Current.GoToAsync("///AddStudent");
        }

        private async Task Scanner()
        {
            await Shell.Current.GoToAsync("///AdminScanner");
        }

        private async Task Masterlist()
        {
            await Shell.Current.GoToAsync("///Masterlist");
        }

        private async Task LogOut()
        {
            MauiApp1.Services.SessionService.ClearSession();
            await Shell.Current.GoToAsync("///LoginPage");
        }
    }
}
