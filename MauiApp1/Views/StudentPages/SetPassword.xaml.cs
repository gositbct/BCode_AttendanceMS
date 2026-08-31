using MauiApp1.ViewModels;
namespace MauiApp1.Views.StudentPages;

public partial class SetPassword : ContentPage
{
    public SetPassword()
    {
        InitializeComponent();
        BindingContext = new SetPasswordMethod();
    }
}