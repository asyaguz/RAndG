using System;

using NAudio.Wave;

namespace RAndG.Players

{

    internal class MusicPlayer : IDisposable

    {

        private WaveOutEvent? outputDevice;

        private AudioFileReader? audioFile;

        public bool IsPlaying =>

            outputDevice?.PlaybackState == PlaybackState.Playing;

        public bool IsPaused =>

            outputDevice?.PlaybackState == PlaybackState.Paused;

        public TimeSpan CurrentTime =>

            audioFile?.CurrentTime ?? TimeSpan.Zero;

        public TimeSpan TotalTime =>

            audioFile?.TotalTime ?? TimeSpan.Zero;

        public void Play(string filePath)

        {

            Stop();

            audioFile = new AudioFileReader(filePath);

            outputDevice = new WaveOutEvent();

            outputDevice.Init(audioFile);

            outputDevice.Play();

        }

        public void Pause()

        {

            outputDevice?.Pause();

        }

        public void Resume()

        {

            outputDevice?.Play();

        }

        public void Stop()

        {

            outputDevice?.Stop();

            outputDevice?.Dispose();

            outputDevice = null;

            audioFile?.Dispose();

            audioFile = null;

        }

        public void SetVolume(float volume)

        {

            if (audioFile != null)

            {

                audioFile.Volume = Math.Clamp(volume, 0f, 1f);

            }

        }

        public void Seek(TimeSpan position)

        {

            if (audioFile == null)

                return;

            double milliseconds = Math.Clamp(

                position.TotalMilliseconds,

                0,

                audioFile.TotalTime.TotalMilliseconds

            );

            audioFile.CurrentTime =

                TimeSpan.FromMilliseconds(milliseconds);

        }

        public void Dispose()

        {

            Stop();

        }

    }

}