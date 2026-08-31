using MauiApp1.Services;
using MauiApp1.ViewModels;
using System.ComponentModel;

namespace MauiApp1.Views.StudentPages;

public partial class MyProfile : ContentPage
{
    private MyProfileMethod? _viewModel;

    public MyProfile()
    {
        InitializeComponent();
    }

    //GANUN DIN PARA IBA IBA NA DIDISPLAY PER STUDNT
    protected override void OnAppearing()
    {
        base.OnAppearing();
  
        if (_viewModel != null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

        _viewModel = new MyProfileMethod();
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        BindingContext = _viewModel;

        _ = RefreshBarcodeAsync();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MyProfileMethod.BarcodeValue))
        {
            _ = RefreshBarcodeAsync();
        }
    }

    // Switched from BarcodeGeneratorView to BarcodeGenerator.WriteToStreamAsync
    // + a plain Image control (same approach as StudentDashboard) to avoid the
    // BarcodeGeneratorView WinUI/Code128 bug.
    //
    // Actual generation + race-safe assignment now lives in BarcodeImageHelper
    // (see that file for why the barcode used to intermittently not show up).
    private async Task RefreshBarcodeAsync()
    {
        await BarcodeImageHelper.SetBarcodeAsync(BarcodeImage, _viewModel?.BarcodeValue);
    }
}
