using MauiApp1.ViewModels;
using static MauiApp1.Models.StudentModel;
using System.Linq;

namespace MauiApp1.Views.AdminPages;

public partial class Masterlist : ContentPage
{
	public Masterlist()
	{
		InitializeComponent();
		BindingContext = new MasterlistMethod();
	}
    private async void OnStudentSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedStudent = e.CurrentSelection.FirstOrDefault() as Student;

        if (selectedStudent == null)
            return;

        // Clear the selection right away so re-selecting the same row after
        // coming back (SelectedItem otherwise wouldn't "change") still fires
        // SelectionChanged next time.
        studentsCollectionView.SelectedItem = null;

        await Shell.Current.GoToAsync($"studentprofile?studentId={selectedStudent.Id}");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MasterlistMethod vm)
        {
            vm.RefreshStudents();
        }
    }
}