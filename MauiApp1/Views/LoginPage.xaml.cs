using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        BindingContext = new LoginPageMethod();

        AnimateCardIn();
    }

    private async void AnimateCardIn()
    {
        try
        {
            LoginCard.Opacity = 0;
            LoginCard.TranslationY = 15;

            await Task.WhenAll(
                LoginCard.FadeTo(
                    1,
                    200,
                    Easing.CubicOut),

                LoginCard.TranslateTo(
                    0,
                    0,
                    200,
                    Easing.CubicOut)
            );
        }
        catch
        {
            LoginCard.Opacity = 1;
            LoginCard.TranslationY = 0;
        }
    }


    // SHOW / HIDE PASSWORD

    private void ShowPassword_Clicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

        if (PasswordEntry.IsPassword)
        {
            ShowPasswordButton.Text = "Show";
        }
        else
        {
            ShowPasswordButton.Text = "Hide";
        }
    }
}