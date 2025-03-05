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
        TitleLabel.FadeTo(1, 1000),
        VidaEntry.FadeTo(1, 1000),
        NombreEntry.FadeTo(1, 1000), // Nuevo campo
        StartButton.FadeTo(1, 1000),
        PhraseLabel.FadeTo(1, 1000)
);

    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        string nombre = NombreEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            await DisplayAlert("Error", "Ingrese un nombre para su monstruo.", "OK");
            return;
        }

        if (int.TryParse(VidaEntry.Text, out int vida) && vida > 0)
        {
            await Navigation.PushAsync(new MainPage(nombre, vida));
        }
        else
        {
            await DisplayAlert("Error", "Ingrese un número válido de vida.", "OK");
        }
    }

}