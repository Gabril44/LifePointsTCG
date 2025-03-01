namespace LifePointsTCG
{
    public partial class MainPage : ContentPage
    {
        AudioPlayerService audioPlayerService;
        AudioPlayerService healsound;
        AudioPlayerService evolution;
        Monster firstMonster;
        List<Monster> bench;
        private int monstruoActual = 0;

        public MainPage(int vida)
        {
            InitializeComponent();
            bench = new List<Monster>();
            for (int i = 0; i < 7; i++)
            {
                Monster monster = new Monster(0);
                bench.Add(monster);
            }
            firstMonster = new Monster(vida);
            bench[0] = firstMonster;
            bench[0].exists = true;
            HealthBar.Progress = 1.0;
            LifeLabel.Text = $"{vida}";
            LifeLabel.TextColor = Colors.Yellow;
            LifeLabel.FontSize = 24;
            LifeLabel.FontAttributes = FontAttributes.Bold;
            LifeLabel.Shadow = new Shadow { Offset = new Point(2, 2), Opacity = 1, Brush = Brush.Black };

            audioPlayerService = new AudioPlayerService("hit.mp3");
            healsound = new AudioPlayerService("heal.mp3");
            evolution = new AudioPlayerService("evolution.mp3");
            Bench1.BackgroundColor = Colors.Red;



        }

        private async Task AnimateHealthBar(double newProgress)
        {
            await HealthBar.ProgressTo(newProgress, 500, Easing.Linear);
        }

        private async void LiberarMonstruo()
        {
            if (bench[monstruoActual].isBurned == 1)
            {
                bench[monstruoActual].isBurned = 0;
                BurnEffectBtn.IsVisible = false;
            }
            if (bench[monstruoActual].isPoisoned == 1)
            {
                bench[monstruoActual].isPoisoned = 0;
                PoisonEffectBtn.IsVisible = false;
            }

            bench[monstruoActual].estado = 0;
            SleepBtn.BackgroundColor = Colors.White;
            SleepBtn.Text = "Sleep";
            ConfusedBtn.BackgroundColor = Colors.LightCoral;
            ConfusedBtn.Text = "Conf";
            ParalyzedBtn.BackgroundColor = Colors.LightGoldenrodYellow;
            ParalyzedBtn.Text = "Par";

            if (bench[monstruoActual].estado == 1)
            {
                await DisplayAlert("Despierto!", "Tu monstruo se encuentra ahora despierto", "Aceptar");
                SleepBtn.BackgroundColor = Colors.White;
                SleepBtn.Text = "Sleep";
                bench[monstruoActual].estado = 0;
            }

            if (bench[monstruoActual].estado == 3)
            {
                await DisplayAlert("Despierto!", "Tu monstruo ya no se encuentra confundido", "Aceptar");
                ConfusedBtn.BackgroundColor = Colors.LightCoral;
                ConfusedBtn.Text = "Conf";
                bench[monstruoActual].estado = 0;
                ConfusedEffectBtn.IsVisible = false;
            }

            if (bench[monstruoActual].estado == 2)
            {
                await DisplayAlert("Liberado!", "Tu monstruo ya no se encuentra paralizado", "Aceptar");
                ParalyzedBtn.BackgroundColor = Colors.LightGoldenrodYellow;
                ParalyzedBtn.Text = "Par";
                bench[monstruoActual].estado = 0;
            }

            await DisplayAlert("¡Liberado!", "Tu monstruo ya no tiene ningún estado especial.", "Aceptar");

        }

        private void UpdateHealthBarColor()
        {
            if (HealthBar.Progress > 0.6)
                HealthBar.ProgressColor = Color.FromArgb("#4CAF50");
            else if (HealthBar.Progress > 0.3)
                HealthBar.ProgressColor = Color.FromArgb("#FFEB3B");
            else
                HealthBar.ProgressColor = Color.FromArgb("#F44336");
        }

        void ReducirVida()
        {
            if (HealthBar.Progress <= 0.1)
            {
                HealthBar.Progress = 0;
                LifeLabel.Text = "0";
                DisplayAlert("Game Over", "¡Te has quedado sin vida!", "Aceptar");
                return;
            }

            audioPlayerService.Play();
            HealthBar.Progress -= 0.1;
            LifeLabel.Text = $"{(int)(HealthBar.Progress * 100)}";
            if (LifeLabel.Text == "0")
            {
                DisplayAlert("Game Over", "¡Te has quedado sin vida!", "Aceptar");
            }
        }

        private async void OnDamageButtonClicked(object sender, EventArgs e)
        {
            // Animar el Grid o BoxView
            if (sender is BoxView boxView)
            {
                var parent = boxView.Parent as Grid;
                if (parent != null)
                {
                    // Animar el contenedor Grid o BoxView
                    var originalScale = parent.Scale;
                    await parent.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                    await parent.ScaleTo(originalScale, 100); // Volver al tamaño original
                }
            }
            var action = await DisplayActionSheet("Select Damage", "Cancel", null, "10","20", "30", "50", "Custom...");

            if (action == "Cancel" || string.IsNullOrEmpty(action))
                return;

            if (action == "Custom...")
            {
                string result = await DisplayPromptAsync("Custom Damage", "Enter damage amount:", "OK", "Cancel", "e.g., 25", keyboard: Keyboard.Numeric);

                if (int.TryParse(result, out int customDamage) && customDamage > 0)
                {
                    ApplyDamage(customDamage);
                }
            }
            else if (int.TryParse(action, out int damage))
            {
                ApplyDamage(damage);
            }

            if (LifeLabel.Text == "0%")
            {
                await DisplayAlert("Game Over", "¡Te has quedado sin vida!", "Aceptar");
            }

        }

        private async void ApplyDamage(int damage)
        {
            for (int i = 0; i < bench.Count; i++)
            {
                if (monstruoActual == i)
                {
                    // Asegúrate de que currentLife sea el valor proporcional de vida en la barra de salud
                    double currentLife = HealthBar.Progress * bench[i].vida_original;
                    double newLife = Math.Max(currentLife - damage, 0);

                    // Aplica el daño al monstruo
                    bench[i].vida -= damage;
                    bench[i].damageRecived += damage;

                    // Calcula la nueva barra de salud
                    double newProgress = bench[i].vida / bench[monstruoActual].vida_original;
                    await HealthBar.ProgressTo(newLife / bench[monstruoActual].vida_original, 500, Easing.Linear);

                    // Actualiza la etiqueta de la vida
                    LifeLabel.Text = $"{(int)newLife}";
                    audioPlayerService.Play();

                    if (newLife <= 0.01)
                    {
                        await DisplayAlert("Game Over", "¡Te has quedado sin vida!", "Aceptar");
                    }
                    UpdateHealthBarColor();
                }
            }
        }

        private async void OnMonedaClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            var action = await DisplayActionSheet("Elegí una opción", "Cancel", null, "Moneda", "Dado");

            if (action == "Cancel" || string.IsNullOrEmpty(action))
                return;

            if (action == "Moneda")
            {
                string resultadoMoneda = new Random().Next(2) == 0 ? "¡Cara!" : "¡Cruz!";
                await DisplayAlert("Tiraste una moneda!", $"El resultado es: {resultadoMoneda}", "Aceptar");
                return;
            }

            if (action == "Dado")
            {
                int resultadoDado = new Random().Next(1, 7);
                await DisplayAlert("Tiraste un dado!", $"El resultado es: {resultadoDado}!", "Aceptar");
            }
        }

        private async void OnLifeClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            var action = await DisplayActionSheet("Selecciona cuánto curar", "Cancelar", null, "20", "30","40", "50", "Custom...");

            if (action == "Cancelar" || string.IsNullOrEmpty(action))
                return;

            int healAmount = 0;

            if (action == "Custom...")
            {
                string result = await DisplayPromptAsync("Curación Personalizada", "Ingresa la cantidad a curar:", "OK", "Cancelar", "Ej: 25", keyboard: Keyboard.Numeric);

                if (!int.TryParse(result, out healAmount) || healAmount <= 0)
                {
                    await DisplayAlert("Error", "Ingresa un número válido.", "Aceptar");
                    return;
                }
            }
            else if (int.TryParse(action, out int predefinedHeal))
            {
                healAmount = predefinedHeal;
            }

            for (int i = 0; i < bench.Count; i++)
            {
                if (i == monstruoActual)
                {
                    bench[i].vida += healAmount;
                    bench[i].damageRecived -= healAmount;
                    double currentLife = HealthBar.Progress * bench[monstruoActual].vida_original;
                    double newLife = Math.Min(currentLife + healAmount, bench[monstruoActual].vida_original);

                    HealthBar.Progress = newLife / bench[monstruoActual].vida_original;
                    LifeLabel.Text = $"{(int)newLife}";

                    await DisplayAlert("Vida", $"Curaste {healAmount} puntos de vida. Life Points: {LifeLabel.Text}", "Aceptar");
                    healsound.Play();
                    UpdateHealthBarColor();
                }
            }

        }

        private async void OnEvolutionClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            string result = await DisplayPromptAsync("Evolución", "Ingrese la vida máxima de la evolución:", "OK", "Cancel", "Ej: 120", keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int nuevaVidaMaxima) && nuevaVidaMaxima > 0)
            {
                bench[monstruoActual].vida_original = nuevaVidaMaxima;
                int vidaActual = Math.Max(nuevaVidaMaxima - bench[monstruoActual].damageRecived, 0);
                bench[monstruoActual].vida = vidaActual;
                HealthBar.Progress = (double)vidaActual / nuevaVidaMaxima;
                LifeLabel.Text = $"{vidaActual}";

                await DisplayAlert("Evolución!", $"Tu Pokémon ha evolucionado. Nueva vida: {vidaActual}/{nuevaVidaMaxima}", "Aceptar");
                evolution.Play();
            }
            else
            {
                await DisplayAlert("Error", "Ingrese un valor válido.", "OK");
            }

            UpdateHealthBarColor();
            LiberarMonstruo();
        }

        private async void ClickedBench3(object sender, EventArgs e)
        {
            // Aquí realizamos la animación de escala y cambio de color
            if (sender is Button button)
            {
                var originalColor = button.BackgroundColor;

                // Cambiar el color a un tono más oscuro (para dar efecto de presionado)
                button.BackgroundColor = Color.FromArgb("#D32F2F"); // Rojo más oscuro

                // Animación de escala
                await button.ScaleTo(0.9, 100); // Reducir tamaño
                await button.ScaleTo(1, 100); // Volver al tamaño original

                // Restaurar el color original
                button.BackgroundColor = originalColor;
            }
            if (bench[2].exists == false)
            {
                string result = await DisplayPromptAsync("Nuevo Monstruo!", "Ingresa la cantidad de vida:", "OK", "Cancelar", keyboard: Keyboard.Numeric);

                if (!int.TryParse(result, out int newLife) || newLife <= 0)
                {
                    await DisplayAlert("Error", "Ingresa un número válido.", "Aceptar");
                    return;
                }
                Monster newMonster = new Monster(newLife);
                await DisplayAlert("Vida Establecida", $"La vida del monstruo es {newLife} puntos.", "Aceptar");
                bench[2] = newMonster;
                bench[2].exists = true;
                Bench3.BackgroundColor = Colors.Red;
                ActualizarBotonSeleccionado();
            }
            else if (bench[2].exists == true)
            {
                LiberarMonstruo();
                // Obtener la vida original del monstruo en bench[1]
                double newLife = bench[2].vida;

                // Actualizamos la barra de salud al 100% de la vida de bench[1]
                if (bench[5].vida == bench[2].vida_original)
                {
                    HealthBar.Progress = 1.0;  // El valor 1.0 representa el 100% de la barra
                }
                else
                {
                    await HealthBar.ProgressTo(newLife / bench[2].vida_original, 500, Easing.Linear);
                }

                // Actualizamos el texto con la vida del monstruo
                LifeLabel.Text = $"{(int)newLife}";

                // Mostramos un mensaje de confirmación
                await DisplayAlert("Bench Monster: " + 3, $"La vida de la barra se ha actualizado a {newLife} puntos.", "Aceptar");

                // Actualizamos el color de la barra de vida según el nuevo valor
                UpdateHealthBarColor();

                monstruoActual = 2;
                ActualizarBotonSeleccionado();
            }
        }
        private async void ClickedBench2(object sender, EventArgs e)
        {
            // Aquí realizamos la animación de escala y cambio de color
            if (sender is Button button)
            {
                var originalColor = button.BackgroundColor;

                // Cambiar el color a un tono más oscuro (para dar efecto de presionado)
                button.BackgroundColor = Color.FromArgb("#D32F2F"); // Rojo más oscuro

                // Animación de escala
                await button.ScaleTo(0.9, 100); // Reducir tamaño
                await button.ScaleTo(1, 100); // Volver al tamaño original

                // Restaurar el color original
                button.BackgroundColor = originalColor;
            }
            if (bench[1].exists == false)
            {
                string result = await DisplayPromptAsync("Nuevo Monstruo!", "Ingresa la cantidad de vida:", "OK", "Cancelar", keyboard: Keyboard.Numeric);

                if (!int.TryParse(result, out int newLife) || newLife <= 0)
                {
                    await DisplayAlert("Error", "Ingresa un número válido.", "Aceptar");
                    return;
                }

                Monster newMonster = new Monster(newLife);
                await DisplayAlert("Vida Establecida", $"La vida del monstruo es {newLife} puntos.", "Aceptar");
                bench[1] = newMonster;
                bench[1].exists = true;
                Bench2.BackgroundColor = Colors.Red;
                ActualizarBotonSeleccionado();
            }
            else if (bench[1].exists == true)
            {
                LiberarMonstruo();
                // Obtener la vida original del monstruo en bench[1]
                double newLife = bench[1].vida;

                // Actualizamos la barra de salud al 100% de la vida de bench[1]
                if (bench[1].vida == bench[1].vida_original)
                {
                    HealthBar.Progress = 1.0;  // El valor 1.0 representa el 100% de la barra
                }
                else
                {
                    await HealthBar.ProgressTo(newLife / bench[1].vida_original, 500, Easing.Linear);
                }

                // Actualizamos el texto con la vida del monstruo
                LifeLabel.Text = $"{(int)newLife}";

                // Mostramos un mensaje de confirmación
                await DisplayAlert("Bench Monster: " + 2, $"La vida de la barra se ha actualizado a {newLife} puntos.", "Aceptar");

                // Actualizamos el color de la barra de vida según el nuevo valor
                UpdateHealthBarColor();

                monstruoActual = 1;
                ActualizarBotonSeleccionado();
                //Bench2.BackgroundColor = Colors.Grey;
            }
        }



        private async void ClickedBench1(object sender, EventArgs e)
        {
            // Aquí realizamos la animación de escala y cambio de color
            if (sender is Button button)
            {
                var originalColor = button.BackgroundColor;

                // Cambiar el color a un tono más oscuro (para dar efecto de presionado)
                button.BackgroundColor = Color.FromArgb("#D32F2F"); // Rojo más oscuro

                // Animación de escala
                await button.ScaleTo(0.9, 100); // Reducir tamaño
                await button.ScaleTo(1, 100); // Volver al tamaño original

                // Restaurar el color original
                button.BackgroundColor = originalColor;
            }
            if (bench[0].exists == true)
            {
                LiberarMonstruo();
                // Obtener la vida original del monstruo en bench[1]
                double newLife = bench[0].vida;

                // Actualizamos la barra de salud al 100% de la vida de bench[1]
                if (bench[5].vida == bench[0].vida_original)
                {
                    HealthBar.Progress = 1.0;  // El valor 1.0 representa el 100% de la barra
                }
                else
                {
                    await HealthBar.ProgressTo(newLife / bench[0].vida_original, 500, Easing.Linear);
                }

                // Actualizamos el texto con la vida del monstruo
                LifeLabel.Text = $"{(int)newLife}";

                // Mostramos un mensaje de confirmación
                await DisplayAlert("Bench Monster: " + 1, $"La vida de la barra se ha actualizado a {newLife} puntos.", "Aceptar");

                // Actualizamos el color de la barra de vida según el nuevo valor
                UpdateHealthBarColor();

                monstruoActual = 0;
                ActualizarBotonSeleccionado();
                //Bench1.BackgroundColor = Colors.Grey;
            }
        }
        private async void ClickedBench4(object sender, EventArgs e)
        {
            // Aquí realizamos la animación de escala y cambio de color
            if (sender is Button button)
            {
                var originalColor = button.BackgroundColor;

                // Cambiar el color a un tono más oscuro (para dar efecto de presionado)
                button.BackgroundColor = Color.FromArgb("#D32F2F"); // Rojo más oscuro

                // Animación de escala
                await button.ScaleTo(0.9, 100); // Reducir tamaño
                await button.ScaleTo(1, 100); // Volver al tamaño original

                // Restaurar el color original
                button.BackgroundColor = originalColor;
            }
            if (bench[3].exists == false)
            {
                string result = await DisplayPromptAsync("Nuevo Monstruo!", "Ingresa la cantidad de vida:", "OK", "Cancelar", keyboard: Keyboard.Numeric);

                if (!int.TryParse(result, out int newLife) || newLife <= 0)
                {
                    await DisplayAlert("Error", "Ingresa un número válido.", "Aceptar");
                    return;
                }

                Monster newMonster = new Monster(newLife);
                await DisplayAlert("Vida Establecida", $"La vida del monstruo es {newLife} puntos.", "Aceptar");
                bench[3] = newMonster;
                bench[3].exists = true;
                Bench4.BackgroundColor = Colors.Red;
                ActualizarBotonSeleccionado();
            }
            else if (bench[3].exists == true)
            {
                LiberarMonstruo();
                // Obtener la vida original del monstruo en bench[1]
                double newLife = bench[3].vida;

                // Actualizamos la barra de salud al 100% de la vida de bench[1]
                if (bench[3].vida == bench[3].vida_original)
                {
                    HealthBar.Progress = 1.0;  // El valor 1.0 representa el 100% de la barra
                }
                else
                {
                    await HealthBar.ProgressTo(newLife / bench[3].vida_original, 500, Easing.Linear);
                }

                // Actualizamos el texto con la vida del monstruo
                LifeLabel.Text = $"{(int)newLife}";

                // Mostramos un mensaje de confirmación
                await DisplayAlert("Bench Monster: " + 4, $"La vida de la barra se ha actualizado a {newLife} puntos.", "Aceptar");

                // Actualizamos el color de la barra de vida según el nuevo valor
                UpdateHealthBarColor();

                monstruoActual = 3;
                ActualizarBotonSeleccionado();
                //Bench4.BackgroundColor = Colors.Grey;
            }
        }
        private async void ClickedBench5(object sender, EventArgs e)
        {
            // Aquí realizamos la animación de escala y cambio de color
            if (sender is Button button)
            {
                var originalColor = button.BackgroundColor;

                // Cambiar el color a un tono más oscuro (para dar efecto de presionado)
                button.BackgroundColor = Color.FromArgb("#D32F2F"); // Rojo más oscuro

                // Animación de escala
                await button.ScaleTo(0.9, 100); // Reducir tamaño
                await button.ScaleTo(1, 100); // Volver al tamaño original

                // Restaurar el color original
                button.BackgroundColor = originalColor;
            }
            if (bench[4].exists == false)
            {
                string result = await DisplayPromptAsync("Nuevo Monstruo!", "Ingresa la cantidad de vida:", "OK", "Cancelar", keyboard: Keyboard.Numeric);

                if (!int.TryParse(result, out int newLife) || newLife <= 0)
                {
                    await DisplayAlert("Error", "Ingresa un número válido.", "Aceptar");
                    return;
                }

                Monster newMonster = new Monster(newLife);
                await DisplayAlert("Vida Establecida", $"La vida del monstruo es {newLife} puntos.", "Aceptar");
                bench[4] = newMonster;
                bench[4].exists = true;
                Bench5.BackgroundColor = Colors.Red;
                ActualizarBotonSeleccionado();
            }
            else if (bench[4].exists == true)
            {
                LiberarMonstruo();
                // Obtener la vida original del monstruo en bench[1]
                double newLife = bench[4].vida;

                // Actualizamos la barra de salud al 100% de la vida de bench[1]
                if (bench[5].vida == bench[4].vida_original)
                {
                    HealthBar.Progress = 1.0;  // El valor 1.0 representa el 100% de la barra
                }
                else
                {
                    await HealthBar.ProgressTo(newLife / bench[4].vida_original, 500, Easing.Linear);
                }

                // Actualizamos el texto con la vida del monstruo
                LifeLabel.Text = $"{(int)newLife}";

                // Mostramos un mensaje de confirmación
                await DisplayAlert("Bench Monster: " + 5, $"La vida de la barra se ha actualizado a {newLife} puntos.", "Aceptar");

                // Actualizamos el color de la barra de vida según el nuevo valor
                UpdateHealthBarColor();

                monstruoActual = 4;
                ActualizarBotonSeleccionado();
                //Bench5.BackgroundColor = Colors.Grey;
            }
        }
        private async void ClickedBench6(object sender, EventArgs e)
        {
            // Aquí realizamos la animación de escala y cambio de color
            if (sender is Button button)
            {
                var originalColor = button.BackgroundColor;

                // Cambiar el color a un tono más oscuro (para dar efecto de presionado)
                button.BackgroundColor = Color.FromArgb("#D32F2F"); // Rojo más oscuro

                // Animación de escala
                await button.ScaleTo(0.9, 100); // Reducir tamaño
                await button.ScaleTo(1, 100); // Volver al tamaño original

                // Restaurar el color original
                button.BackgroundColor = originalColor;
            }


            if (bench[5].exists == false)
            {
                string result = await DisplayPromptAsync("Nuevo Monstruo!", "Ingresa la cantidad de vida:", "OK", "Cancelar", keyboard: Keyboard.Numeric);

                if (!int.TryParse(result, out int newLife) || newLife <= 0)
                {
                    await DisplayAlert("Error", "Ingresa un número válido.", "Aceptar");
                    return;
                }

                Monster newMonster = new Monster(newLife);
                await DisplayAlert("Vida Establecida", $"La vida del monstruo es {newLife} puntos.", "Aceptar");
                bench[5] = newMonster;
                bench[5].exists = true;
                Bench6.BackgroundColor = Colors.Red;
                ActualizarBotonSeleccionado();
            }
            else if (bench[5].exists == true)
            {
                LiberarMonstruo();
                // Obtener la vida original del monstruo en bench[1]
                double newLife = bench[5].vida;

                // Actualizamos la barra de salud al 100% de la vida de bench[1]
                if (bench[5].vida == bench[5].vida_original)
                {
                    HealthBar.Progress = 1.0;  // El valor 1.0 representa el 100% de la barra
                }
                else
                {
                    await HealthBar.ProgressTo(newLife / bench[5].vida_original, 500, Easing.Linear);
                }

                // Actualizamos el texto con la vida del monstruo
                LifeLabel.Text = $"{(int)newLife}";

                // Mostramos un mensaje de confirmación
                await DisplayAlert("Bench Monster: " + 6, $"La vida de la barra se ha actualizado a {newLife} puntos.", "Aceptar");

                // Actualizamos el color de la barra de vida según el nuevo valor
                UpdateHealthBarColor();

                monstruoActual = 5;
                ActualizarBotonSeleccionado();
                //Bench6.BackgroundColor = Colors.Grey;
            }
        }

        private void ActualizarBotonSeleccionado()
        {
            switch (monstruoActual)
            {
                case 0: Bench1.BackgroundColor = Colors.Grey;
                    if (Bench2.BackgroundColor == Colors.Gray)
                    {
                        Bench2.BackgroundColor = Colors.Red;
                    }
                    if (Bench3.BackgroundColor == Colors.Gray)
                    {
                        Bench3.BackgroundColor = Colors.Red;
                    }
                    if (Bench4.BackgroundColor == Colors.Gray)
                    {
                        Bench4.BackgroundColor = Colors.Red;
                    }
                    if (Bench5.BackgroundColor == Colors.Gray)
                    {
                        Bench5.BackgroundColor = Colors.Red;
                    }
                    if (Bench6.BackgroundColor == Colors.Gray)
                    {
                        Bench6.BackgroundColor = Colors.Red;
                    }
                    break;
                case 1: Bench2.BackgroundColor = Colors.Grey;
                    if (Bench1.BackgroundColor == Colors.Gray)
                    {
                        Bench1.BackgroundColor = Colors.Red;
                    }
                    if (Bench3.BackgroundColor == Colors.Gray)
                    {
                        Bench3.BackgroundColor = Colors.Red;
                    }
                    if (Bench4.BackgroundColor == Colors.Gray)
                    {
                        Bench4.BackgroundColor = Colors.Red;
                    }
                    if (Bench5.BackgroundColor == Colors.Gray)
                    {
                        Bench5.BackgroundColor = Colors.Red;
                    }
                    if (Bench6.BackgroundColor == Colors.Gray)
                    {
                        Bench6.BackgroundColor = Colors.Red;
                    }
                    break;
                case 2: Bench3.BackgroundColor = Colors.Grey;
                    if (Bench1.BackgroundColor == Colors.Gray)
                    {
                        Bench1.BackgroundColor = Colors.Red;
                    }
                    if (Bench2.BackgroundColor == Colors.Gray)
                    {
                        Bench2.BackgroundColor = Colors.Red;
                    }
                    if (Bench4.BackgroundColor == Colors.Gray)
                    {
                        Bench4.BackgroundColor = Colors.Red;
                    }
                    if (Bench5.BackgroundColor == Colors.Gray)
                    {
                        Bench5.BackgroundColor = Colors.Red;
                    }
                    if (Bench6.BackgroundColor == Colors.Gray)
                    {
                        Bench6.BackgroundColor = Colors.Red;
                    }
                    break;
                case 3: Bench4.BackgroundColor = Colors.Grey;
                    if (Bench1.BackgroundColor == Colors.Gray)
                    {
                        Bench1.BackgroundColor = Colors.Red;
                    }
                    if (Bench3.BackgroundColor == Colors.Gray)
                    {
                        Bench3.BackgroundColor = Colors.Red;
                    }
                    if (Bench2.BackgroundColor == Colors.Gray)
                    {
                        Bench2.BackgroundColor = Colors.Red;
                    }
                    if (Bench5.BackgroundColor == Colors.Gray)
                    {
                        Bench5.BackgroundColor = Colors.Red;
                    }
                    if (Bench6.BackgroundColor == Colors.Gray)
                    {
                        Bench6.BackgroundColor = Colors.Red;
                    }
                    break;
                case 4: Bench5.BackgroundColor = Colors.Grey;
                    if (Bench2.BackgroundColor == Colors.Gray)
                    {
                        Bench2.BackgroundColor = Colors.Red;
                    }
                    if (Bench3.BackgroundColor == Colors.Gray)
                    {
                        Bench3.BackgroundColor = Colors.Red;
                    }
                    if (Bench4.BackgroundColor == Colors.Gray)
                    {
                        Bench4.BackgroundColor = Colors.Red;
                    }
                    if (Bench1.BackgroundColor == Colors.Gray)
                    {
                        Bench1.BackgroundColor = Colors.Red;
                    }
                    if (Bench6.BackgroundColor == Colors.Gray)
                    {
                        Bench6.BackgroundColor = Colors.Red;
                    }
                    break;
                case 5: Bench6.BackgroundColor = Colors.Grey;
                    if (Bench2.BackgroundColor == Colors.Gray)
                    {
                        Bench2.BackgroundColor = Colors.Red;
                    }
                    if (Bench3.BackgroundColor == Colors.Gray)
                    {
                        Bench3.BackgroundColor = Colors.Red;
                    }
                    if (Bench4.BackgroundColor == Colors.Gray)
                    {
                        Bench4.BackgroundColor = Colors.Red;
                    }
                    if (Bench5.BackgroundColor == Colors.Gray)
                    {
                        Bench5.BackgroundColor = Colors.Red;
                    }
                    if (Bench1.BackgroundColor == Colors.Gray)
                    {
                        Bench1.BackgroundColor = Colors.Red;
                    }
                    break;
            }
        }

        //Estados: 0:normal 1:Asleep 2:Paralyzed 3:Confused
        private async void OnPoisonClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }

            if (bench[monstruoActual].isPoisoned == 0) 
            {
                await DisplayAlert("Veneno!", "Tu monstruo se ha envenenado!", "Aceptar");
                PoisonEffectBtn.IsVisible = true;
                bench[monstruoActual].isPoisoned = 1;
            }
            else if (bench[monstruoActual].isPoisoned == 1) 
            {
                await DisplayAlert("Libre!", "Tu monstruo ya no se encuentra envenenado!", "Aceptar");
                PoisonEffectBtn.IsVisible = false;
                bench[monstruoActual].isPoisoned = 0;
            }

        }

        private async void OnBurnClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }

            if (bench[monstruoActual].isBurned == 0)
            {
                await DisplayAlert("Quemadura!", "Tu monstruo se encuentra Quemado!", "Aceptar");
                BurnEffectBtn.IsVisible = true;
                bench[monstruoActual].isBurned = 1;
            }
            else if (bench[monstruoActual].isBurned == 1)
            {
                await DisplayAlert("Libre!", "Tu monstruo ya no se encuentra quemado!", "Aceptar");
                BurnEffectBtn.IsVisible = false;
                bench[monstruoActual].isBurned = 0;
            }
        }

        private async void OnConfusedClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            if (ConfusedBtn.BackgroundColor == Colors.LightCoral && bench[monstruoActual].estado == 0)
            {
                await DisplayAlert("Confuso!", "Tu monstruo se encuentra confundido", "Aceptar");
                ConfusedBtn.BackgroundColor = Colors.LightSalmon;
                ConfusedBtn.Text = "❔";
                bench[monstruoActual].estado = 3;
                ConfusedEffectBtn.IsVisible = true;
            }
            else
            {
                if (bench[monstruoActual].estado == 3)
                {
                    await DisplayAlert("Despierto!", "Tu monstruo ya no se encuentra confundido", "Aceptar");
                    ConfusedBtn.BackgroundColor = Colors.LightCoral;
                    ConfusedBtn.Text = "Conf";
                    bench[monstruoActual].estado = 0;
                    ConfusedEffectBtn.IsVisible= false;
                }

                if (bench[monstruoActual].estado == 1 || bench[monstruoActual].estado == 2)
                {
                    await DisplayAlert("No se puede", "Tu monstruo ya tiene otro efecto! no se pueden acumular dormido, paralizado ni confundido", "Aceptar");
                }

            }

        }

        private async void OnSleepClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            if (SleepBtn.BackgroundColor == Colors.White && bench[monstruoActual].estado == 0)
            {
                await DisplayAlert("Sueño!", "Tu monstruo se encuentra dormido", "Aceptar");
                SleepBtn.BackgroundColor = Colors.Coral;
                SleepBtn.Text = "Zzz..";
                bench[monstruoActual].estado = 1;

            }
            else
            {
                if (bench[monstruoActual].estado == 1)
                {
                    await DisplayAlert("Despierto!", "Tu monstruo se encuentra ahora despierto", "Aceptar");
                    SleepBtn.BackgroundColor = Colors.White;
                    SleepBtn.Text = "Sleep";
                    bench[monstruoActual].estado = 0;
                }

                if (bench[monstruoActual].estado == 2 || bench[monstruoActual].estado == 3)
                {
                    await DisplayAlert("No se puede", "Tu monstruo ya tiene otro efecto! no se pueden acumular dormido, paralizado ni confundido", "Aceptar");
                }

            }
        }

        private async void OnParalyzedClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            if (ParalyzedBtn.BackgroundColor == Colors.LightGoldenrodYellow && bench[monstruoActual].estado == 0)
            {
                await DisplayAlert("Paralizado!", "Tu monstruo se encuentra paralizado!", "Aceptar");
                ParalyzedBtn.BackgroundColor = Colors.Yellow;
                ParalyzedBtn.Text = "⚡";
                bench[monstruoActual].estado = 2;
            }
            else
            {
                if (bench[monstruoActual].estado == 2)
                {
                    await DisplayAlert("Liberado!", "Tu monstruo ya no se encuentra paralizado", "Aceptar");
                    ParalyzedBtn.BackgroundColor = Colors.LightGoldenrodYellow;
                    ParalyzedBtn.Text = "Par";
                    bench[monstruoActual].estado = 0;
                }

                if (bench[monstruoActual].estado == 1 || bench[monstruoActual].estado == 3)
                {
                    await DisplayAlert("No se puede", "Tu monstruo ya tiene otro efecto! no se pueden acumular dormido, paralizado ni confundido", "Aceptar");
                }

            }

        }
        //botones invisibles
        private async void OnEffectPoisonClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            ApplyDamage(10);
        }

        private async void OnEffectBurnClicked(object sender, EventArgs e) 
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            ApplyDamage(20);
        }

        private async void OnEffectConfusedClicked(object sender, EventArgs e) 
        {
            if (sender is Button button)
            {
                // Animar el Button en sí
                var originalScale = button.Scale;
                await button.ScaleTo(0.9, 100); // Reducir tamaño temporalmente
                await button.ScaleTo(originalScale, 100); // Volver al tamaño original
            }
            ApplyDamage(30);
        }
    }
}
