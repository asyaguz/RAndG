using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAndG.Players
{
    internal class StandardPlayer
    {
        private readonly MusicPlayer musicPlayer;
        public StandardPlayer()
        {
            musicPlayer = new MusicPlayer();
        }
        public void Play(string filePath)
        {
            musicPlayer.Play(filePath);
        }
        public void Stop()
        {
            musicPlayer.Stop();
        }
    }
}
