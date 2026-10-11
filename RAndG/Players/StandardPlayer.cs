using System;

namespace RAndG.Players

{

    internal class StandardPlayer : IDisposable

    {

        private readonly MusicPlayer musicPlayer;

        public bool IsPlaying => musicPlayer.IsPlaying;

        public bool IsPaused => musicPlayer.IsPaused;

        public TimeSpan CurrentTime => musicPlayer.CurrentTime;

        public TimeSpan TotalTime => musicPlayer.TotalTime;

        public StandardPlayer()

        {

            musicPlayer = new MusicPlayer();

        }

        public void Play(string filePath)

        {

            musicPlayer.Play(filePath);

        }

        public void Pause()

        {

            musicPlayer.Pause();

        }

        public void Resume()

        {

            musicPlayer.Resume();

        }

        public void Stop()

        {

            musicPlayer.Stop();

        }

        public void SetVolume(float volume)

        {

            musicPlayer.SetVolume(volume);

        }

        public void Seek(TimeSpan position)

        {

            musicPlayer.Seek(position);

        }

        public void Dispose()

        {

            musicPlayer.Dispose();

        }

    }

}

