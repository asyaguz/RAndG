using System;
using System.Collections.Generic;

namespace RAndG.Player
{
    // Класс для воспроизведения одного аудиофайла
    public class StandardPlayer
    {
        private MusicPlayer internalPlayer;
        public StandardPlayer(MusicPlayer player)
        {
            this.internalPlayer = player;
        }
        public void PlaySingleFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) == false)
            {
                List<string> singleQueue = new List<string>();
                singleQueue.Add(filePath);
                internalPlayer.SetQueue(singleQueue, 0);
                internalPlayer.Play();
            }
        }
    }
}
