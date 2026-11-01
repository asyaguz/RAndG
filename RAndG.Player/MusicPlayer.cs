using System;
using System.Collections.Generic;
using NAudio.Wave;

namespace RAndG.Player
{
    public class MusicPlayer
    {
        private AudioFileReader fileReader;
        private WaveOutEvent outputDevice;
        private List<string> currentQueue;
        private int currentIndex;
        private int currentVolumePercent;

        public bool LoopTrack { get; set; }
        public bool Shuffle { get; set; }

        public MusicPlayer()
        {
            currentQueue = new List<string>();
            currentIndex = -1;
            currentVolumePercent = 50;
        }

        public void SetQueue(List<string> filePaths, int startIndex)
        {
            currentQueue.Clear();
            foreach (string path in filePaths)
            {
                currentQueue.Add(path);
            }
            currentIndex = startIndex;
        }

        public string GetCurrentTrackPath()
        {
            if (currentIndex >= 0 && currentIndex < currentQueue.Count)
            {
                return currentQueue[currentIndex];
            }
            return null;
        }

        public void Play()
        {
            string currentPath = GetCurrentTrackPath();
            if (string.IsNullOrEmpty(currentPath) == false)
            {
                Stop();

                outputDevice = new WaveOutEvent();
                fileReader = new AudioFileReader(currentPath);

                float volumeFloat = currentVolumePercent / 100f;
                fileReader.Volume = volumeFloat;

                outputDevice.Init(fileReader);
                outputDevice.Play();
            }
        }

        public void Pause()
        {
            if (outputDevice != null)
            {
                outputDevice.Pause();
            }
        }

        public void Resume()
        {
            if (outputDevice != null)
            {
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
            if (fileReader != null)
            {
                fileReader.Dispose();
                fileReader = null;
            }
        }

        public void Next()
        {
            if (currentQueue.Count == 0) return;

            if (Shuffle)
            {
                Random rand = new Random();
                currentIndex = rand.Next(0, currentQueue.Count);
            }
            else
            {
                currentIndex = currentIndex + 1;
                if (currentIndex >= currentQueue.Count)
                {
                    currentIndex = 0;
                }
            }
            Play();
        }

        public void Previous()
        {
            if (currentQueue.Count == 0) return;

            currentIndex = currentIndex - 1;
            if (currentIndex < 0)
            {
                currentIndex = currentQueue.Count - 1;
            }
            Play();
        }

        public void SetVolume(int volumePercent)
        {
            currentVolumePercent = volumePercent;
            if (fileReader != null)
            {
                float volumeFloat = volumePercent / 100f;
                fileReader.Volume = volumeFloat;
            }
        }

        public double GetCurrentPositionSeconds()
        {
            if (fileReader != null)
            {
                return fileReader.CurrentTime.TotalSeconds;
            }
            return 0.0;
        }

        public double GetTotalDurationSeconds()
        {
            if (fileReader != null)
            {
                return fileReader.TotalTime.TotalSeconds;
            }
            return 0.0;
        }

        public void SetPositionSeconds(double seconds)
        {
            if (fileReader != null)
            {
                fileReader.CurrentTime = TimeSpan.FromSeconds(seconds);
            }
        }
    }
}
