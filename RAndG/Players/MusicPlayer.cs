using System;
using NAudio.Wave;
namespace RAndG.Players
{
    internal class MusicPlayer : IDisposable
    {
        private WaveOutEvent? outputDevice;
        private AudioFileReader? audioFile;
        private float volume = 0.7f;
        public bool IsPlaying => outputDevice?.PlaybackState == PlaybackState.Playing;
        public bool IsPaused => outputDevice?.PlaybackState == PlaybackState.Paused;
        public TimeSpan CurrentTime => audioFile?.CurrentTime ?? TimeSpan.Zero;
        public TimeSpan TotalTime => audioFile?.TotalTime ?? TimeSpan.Zero;
        public void Play(string filePath)
        {
            Stop();
            try
            {
                audioFile = new AudioFileReader(filePath);
                audioFile.Volume = volume;
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();
            }
            catch
            {
                Stop();
                throw;
            }
        }
        public void Pause()
        {
            outputDevice?.Pause();
        }
        public void Resume()
        {
            if (outputDevice != null && audioFile != null)
            {
                if (audioFile.CurrentTime >= audioFile.TotalTime)
                    audioFile.CurrentTime = TimeSpan.Zero;
                outputDevice.Play();
            }
        }
        public void Stop()
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }
            audioFile?.Dispose();
            audioFile = null;
        }
        public void SetVolume(float value)
        {
            volume = Math.Clamp(value, 0f, 1f);
            if (audioFile != null)
                audioFile.Volume = volume;
        }
        public void Seek(TimeSpan position)
        {
            if (audioFile == null)
                return;
            double milliseconds = Math.Clamp(position.TotalMilliseconds, 0, audioFile.TotalTime.TotalMilliseconds);
            audioFile.CurrentTime = TimeSpan.FromMilliseconds(milliseconds);
        }
        public void Dispose()
        {
            Stop();
        }
    }
}