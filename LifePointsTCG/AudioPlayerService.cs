using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.Audio;

namespace LifePointsTCG
{
    public class AudioPlayerService
    {
        private readonly IAudioPlayer _player;
        private string audio;
        public AudioPlayerService(string audio)
        {
            this.audio = audio;
            try
            {
                var streamTask = FileSystem.OpenAppPackageFileAsync(audio);
                streamTask.Wait(); // Esperar a que el archivo cargue
                var stream = streamTask.Result;

                _player = AudioManager.Current.CreatePlayer(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando el audio: {ex.Message}");
            }

        }

        public void Play()
        {
            _player.Volume = 0.5;
            _player?.Play();
        }

        public void Stop()
        {
            _player?.Stop();
        }
    }
}
