using System;
using System.Collections.Generic;

namespace RAndG.Player
{
    public class PlaylistPlayer
    {
        private MusicPlayer internalPlayer;
        public PlaylistPlayer(MusicPlayer player)
        {
            this.internalPlayer = player;
        }
        public void PlayPlaylistFiles(List<string> filePaths)
        {
            if (filePaths != null && filePaths.Count > 0)
            {
                internalPlayer.SetQueue(filePaths, 0);
                internalPlayer.Play();
            }
        }
    }
}
