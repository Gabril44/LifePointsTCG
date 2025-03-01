namespace LifePointsTCG;

public partial class Welcome : ContentPage
{
	public Welcome()
	{
		InitializeComponent();
	}

    // Este es el m�todo que se llama cuando la p�gina aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Animaci�n de desvanecimiento para los elementos de la p�gina
        await Task.WhenAll(
            TitleLabel.FadeTo(1, 1000), // Fade in el t�tulo
            VidaEntry.FadeTo(1, 1000),  // Fade in el campo de entrada
            StartButton.FadeTo(1, 1000), // Fade in el bot�n de inicio
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
            await DisplayAlert("Error", "Ingrese un n�mero v�lido de vida.", "OK");
        }
    }
}