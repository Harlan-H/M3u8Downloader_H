using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text;

namespace M3u8Downloader_H.Services
{
    public class SoundService : IDisposable
    {
        private readonly string SoundLocalDir = "Sounds/";
        private readonly SoundPlayer? SuccessSound;
        private readonly SoundPlayer? ErrorSound;
        public SoundService()
        {
            string successPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SoundLocalDir, "success.wav");
            string errorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SoundLocalDir, "error.wav");
            if (File.Exists(successPath) && File.Exists(errorPath))
            {
                SuccessSound = new SoundPlayer(successPath);
                SuccessSound?.LoadAsync();

                ErrorSound = new SoundPlayer(errorPath);
                ErrorSound?.LoadAsync();
            }
        }

        public void PlaySuccess(bool isPlay)
        {
            if (!isPlay) return;

            try
            {
                SuccessSound?.Stop();
                SuccessSound?.PlaySync();
            }
            catch (FileNotFoundException)
            {

            }
        }

        public void PlayError(bool isPlay)
        {
            if (!isPlay) return;

            try
            {
                ErrorSound?.Stop();
                ErrorSound?.PlaySync();
            }
            catch (FileNotFoundException)
            {

            }
        }

        public void Dispose()
        {
            SuccessSound?.Dispose();
            ErrorSound?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
