namespace LifePointsTCG;

public partial class Welcome : ContentPage
{
	public Welcome()
	{
		InitializeComponent();
	}

    // Este es el método que se llama cuando la página aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Animación de desvanecimiento para los elementos de la página
        await Task.WhenAll(
            TitleLabel.FadeTo(1, 1000), // Fade in el título
            VidaEntry.FadeTo(1, 1000),  // Fade in el campo de entrada
            StartButton.FadeTo(1, 1000), // Fade in el botón de inicio
            PhraseLabel.FadeTo(1,1000)
        );
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        if (int.TryParse(VidaEntry.Text, out int vida) && vida > 0)
        {
            await Navigation.PushAsync(new MainPage(vida));
        }
        else
        {
            await DisplayAlert("Error", "Ingrese un número válido de vida.", "OK");
        }
    }
}