using System;

using RAndG.Models;

namespace RAndG.Players

{

    internal class PlaylistPlayer : IDisposable

    {

        private readonly StandardPlayer player;

        public bool IsPlaying => player.IsPlaying;

        public bool IsPaused => player.IsPaused;

        public TimeSpan CurrentTime => player.CurrentTime;

        public TimeSpan TotalTime => player.TotalTime;

        public PlaylistPlayer()

        {

            player = new StandardPlayer();

        }

        public void Play(Track track)

        {

            player.Play(track.FilePath);

        }

        public void Pause()

        {

            player.Pause();

        }

        public void Resume()

        {

            player.Resume();

        }

        public void Stop()

        {

            player.Stop();

        }

        public void SetVolume(float volume)

        {

            player.SetVolume(volume);

        }

        public void Seek(TimeSpan position)

        {

            player.Seek(position);

        }

        public void Dispose()

        {

            player.Dispose();

        }

    }

}


