using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAndG.Models;

namespace RAndG.Players
{
    internal class PlaylistPlayer
    {
        private readonly StandardPlayer player;
        public PlaylistPlayer()
        {
            player = new StandardPlayer();
        }
        public void Play(Track track)
        {
            player.Play(track.FilePath);
        }
        public void Stop()
        {
            player.Stop();
        }
    }
}
