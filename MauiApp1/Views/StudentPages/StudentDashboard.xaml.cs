using MauiApp1.Services;
using MauiApp1.ViewModels;
using System.ComponentModel;

namespace MauiApp1.Views.StudentPages;

public partial class StudentDashboard : ContentPage
{
    private StudentDashboardMethod? _viewModel;

    public StudentDashboard()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
      
        if (_viewModel != null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

        _viewModel = new StudentDashboardMethod();
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        BindingContext = _viewModel;

        _ = RefreshBarcodeAsync();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StudentDashboardMethod.BarcodeValue))
        {
            _ = RefreshBarcodeAsync();
        }
    }

    private async Task RefreshBarcodeAsync()
    {
        await BarcodeImageHelper.SetBarcodeAsync(BarcodeImage, _viewModel?.BarcodeValue);
    }
}
