using MauiApp1.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using static MauiApp1.Models.StudentModel;

namespace MauiApp1.ViewModels
{
    internal class MasterlistMethod : INotifyPropertyChanged
    {
        private List<Student> _allStudents;
        public ObservableCollection<Student> Students { get; set; } = new();

        private DatabaseService _db;

        public ICommand DashboardNav { get; }

        public MasterlistMethod()
        {
            _db = new DatabaseService();
            LoadStudents();

            DashboardNav = new Command(async () => await GoToDashboard());
        }


        //SAME THING NAG LOLOAD NG LIVE DATA FROM DATABASE
        private async void LoadStudents()
        {
            await _db.Init();

            var data = await _db.GetStudents();
            _allStudents = data;

            Students.Clear();
            foreach (var s in data)
                Students.Add(s);
        }

        //ROUTING
        private async Task GoToDashboard()
        {
            await Shell.Current.GoToAsync("///AdminDashboard");
        }

        //SEARCH FEATURE SA MASTERLIST
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;

                _searchText = value;
                OnPropertyChanged(nameof(SearchText));

                FilterStudents();
            }
        }

        //ETO NAG FIFILTER NG SINESEARCH
        public void FilterStudents()
        {
            if (_allStudents == null) return;

            Students.Clear();

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allStudents
                : _allStudents.Where(s =>
                    s.Name.ToLower().Contains(SearchText.ToLower()) ||
                    s.Id.Contains(SearchText));

            foreach (var s in filtered)
                Students.Add(s);
        }
        public async void RefreshStudents()
        {
            await _db.Init();

            var data = await _db.GetStudents();

            _allStudents = data;

            // Re-apply whatever search filter was active instead of always
            // showing the full list - without this, returning to the
            // Masterlist (which refreshes on OnAppearing) would silently
            // drop the search the admin had typed, even though the search
            // box still showed the text.  SABI NYA YAN EH
            FilterStudents();
        }

        //ETO NAG UUPDATE PAG MAY NABABAGO SA MASTERLIST
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
