using MauiApp1.Services;
using MauiApp1.ViewModels;
using System.ComponentModel;

namespace MauiApp1.Views.AdminPages;

public partial class StudentProfile : ContentPage
{

    //VIBE CODEE
    private readonly StudentProfileMethod _viewModel;

    public StudentProfile()
    {
        InitializeComponent();
        _viewModel = new StudentProfileMethod();
        BindingContext = _viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StudentProfileMethod.Barcode))
        {
            _ = RefreshBarcodeAsync();
        }
    }

    // The barcode section on this page was previously an empty placeholder
    // ContentView with nothing ever drawn into it - this generates the
    // student's barcode the same way StudentDashboard/AddStudent/MyProfile do,
    // and refreshes it whenever ApplyQueryAttributes loads a different student.
    //
    // Actual generation + race-safe assignment now lives in BarcodeImageHelper
    // (see that file for why the barcode used to intermittently not show up).
    private async Task RefreshBarcodeAsync()
    {
        await BarcodeImageHelper.SetBarcodeAsync(BarcodeImage, _viewModel.Barcode);
    }
}
