
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

        // Устанавливает очередь воспроизведения
        public void SetQueue(List<string> filePaths, int startIndex)
        {
            Stop();

            currentQueue.Clear();

            foreach (string path in filePaths)
            {
                currentQueue.Add(path);
            }

            if (currentQueue.Count > 0 && startIndex >= 0 && startIndex < currentQueue.Count)
            {
                currentIndex = startIndex;
            }
            else
            {
                currentIndex = -1;
            }
        }

        // Возвращает путь текущего трека
        public string GetCurrentTrackPath()
        {
            if (currentIndex >= 0 && currentIndex < currentQueue.Count)
            {
                return currentQueue[currentIndex];
            }

            return null;
        }

        // Запускает воспроизведение
        public void Play()
        {
            string currentPath = GetCurrentTrackPath();

            if (string.IsNullOrEmpty(currentPath))
            {
                return;
            }

            Stop();

            fileReader = new AudioFileReader(currentPath);
            fileReader.Volume = currentVolumePercent / 100f;

            outputDevice = new WaveOutEvent();
            outputDevice.Init(fileReader);
            outputDevice.Play();
        }

        // Проверяет, закончился ли трек
        public bool IsTrackFinished()
        {
            if (fileReader == null || outputDevice == null)
            {
                return false;
            }

            if (outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                double current = fileReader.CurrentTime.TotalSeconds;
                double total = fileReader.TotalTime.TotalSeconds;

                if (total > 0 && current >= total - 0.5)
                {
                    return true;
                }
            }

            return false;
        }

        // Пауза
        public void Pause()
        {
            if (outputDevice != null)
            {
                outputDevice.Pause();
            }
        }

        // Продолжение воспроизведения
        public void Resume()
        {
            if (outputDevice != null)
            {
                outputDevice.Play();
            }
        }

        // Остановка и освобождение ресурсов
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

        // Следующий трек
        public void Next()
        {
            if (currentQueue.Count == 0)
            {
                return;
            }

            if (Shuffle && currentQueue.Count > 1)
            {
                Random rand = new Random();

                int newIndex = currentIndex;

                while (newIndex == currentIndex)
                {
                    newIndex = rand.Next(0, currentQueue.Count);
                }

                currentIndex = newIndex;
            }
            else
            {
                currentIndex++;

                if (currentIndex >= currentQueue.Count)
                {
                    currentIndex = 0;
                }
            }

            Play();
        }

        // Предыдущий трек
        public void Previous()
        {
            if (currentQueue.Count == 0)
            {
                return;
            }

            currentIndex--;

            if (currentIndex < 0)
            {
                currentIndex = currentQueue.Count - 1;
            }

            Play();
        }

        // Изменение громкости
        public void SetVolume(int volumePercent)
        {
            if (volumePercent < 0)
            {
                volumePercent = 0;
            }

            if (volumePercent > 100)
            {
                volumePercent = 100;
            }

            currentVolumePercent = volumePercent;

            if (fileReader != null)
            {
                fileReader.Volume = currentVolumePercent / 100f;
            }
        }

        // Текущая позиция трека
        public double GetCurrentPositionSeconds()
        {
            if (fileReader != null)
            {
                return fileReader.CurrentTime.TotalSeconds;
            }

            return 0.0;
        }

        // Продолжительность трека
        public double GetTotalDurationSeconds()
        {
            if (fileReader != null)
            {
                return fileReader.TotalTime.TotalSeconds;
            }

            return 0.0;
        }

        // Перемотка трека
        public void SetPositionSeconds(double seconds)
        {
            if (fileReader != null)
            {
                double total = fileReader.TotalTime.TotalSeconds;

                if (seconds < 0)
                {
                    seconds = 0;
                }

                if (seconds > total)
                {
                    seconds = total;
                }

                fileReader.CurrentTime = TimeSpan.FromSeconds(seconds);
            }
        }
    }
}
