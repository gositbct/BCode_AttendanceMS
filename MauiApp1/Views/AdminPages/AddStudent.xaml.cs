using MauiApp1.Services;
using MauiApp1.ViewModels;
using System.ComponentModel;

namespace MauiApp1.Views.AdminPages;

public partial class AddStudent : ContentPage
{
    private readonly AddStudentMethod _viewModel;

    //VIBE CODE ULITT
    public AddStudent()
    {
        InitializeComponent();
        _viewModel = new AddStudentMethod();
        BindingContext = _viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AddStudentMethod.GeneratedId) ||
            e.PropertyName == nameof(AddStudentMethod.IsStudentRole))
        {
            _ = RefreshBarcodeAsync();
        }
    }

    // Switched from BarcodeGeneratorView to BarcodeGenerator.WriteToStreamAsync
    // + a plain Image control. BarcodeGeneratorView has a known, still-open bug
    // on WinUI/Windows for Code128 that throws
    // "System.NotSupportedException: Unable to expand length of this stream
    // beyond its capacity." (github.com/Redth/ZXing.Net.Maui/issues/168).
    // Generating a PNG in memory and displaying it as a normal Image sidesteps
    // that control entirely and works the same on every platform.
    //
    // Actual generation + race-safe assignment now lives in BarcodeImageHelper
    // (see that file for why the barcode used to intermittently not show up).
    private async Task RefreshBarcodeAsync()
    {
        var value = _viewModel.ShowBarcode ? _viewModel.GeneratedId : null;
        await BarcodeImageHelper.SetBarcodeAsync(BarcodeImage, value);
    }
}
