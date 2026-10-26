using System;

namespace RAndG.Players

{
    public class PlaylistPlayer : IDisposable
    {
        private readonly StandardPlayer player = new();

        public bool IsPlaying => player.IsPlaying;

        public bool IsPaused => player.IsPaused;

        public TimeSpan CurrentTime => player.CurrentTime;

        public TimeSpan TotalTime => player.TotalTime;

        public void Play(string filePath)

        {
            player.Play(filePath);

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
