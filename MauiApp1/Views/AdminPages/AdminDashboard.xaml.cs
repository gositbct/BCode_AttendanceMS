using MauiApp1.ViewModels;

namespace MauiApp1.Views.AdminPages;

public partial class AdminDashboard : ContentPage
{
    public AdminDashboard()
    {
        InitializeComponent();
        BindingContext = new AdminDashboardMethods();
    }
}