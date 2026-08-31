using MauiApp1.ViewModels;

namespace MauiApp1.Views.StudentPages;

public partial class AttendanceHistory : ContentPage
{
	public AttendanceHistory()
	{
		InitializeComponent();
    }

    //SA ANO TO PARA IBANG DATA ANG NADIDISPLAY KADA STUDNET
    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new AttendanceHistoryMethod();
    }
}
