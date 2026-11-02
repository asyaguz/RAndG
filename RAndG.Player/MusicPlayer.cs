using System;
using System.Collections.Generic;
using NAudio.Wave;

namespace RAndG.Player
{
    // Основной класс аудиопроигрывателя. Управляет воспроизведением, очередью композиций, громкостью и переключением треков
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
        // Устанавливает очередь воспроизведения и выбирает начальную композицию
        public void SetQueue(List<string> filePaths, int startIndex)
        {
            currentQueue.Clear();
            foreach (string path in filePaths)
            {
                currentQueue.Add(path);
            }
            currentIndex = startIndex;
        }
        // Возвращает путь к текущей композиции. Если композиция не выбрана, возвращает null
        public string GetCurrentTrackPath()
        {
            if (currentIndex >= 0 && currentIndex < currentQueue.Count)
            {
                return currentQueue[currentIndex];
            }
            return null;
        }
        // Запускает воспроизведение текущей композиции
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
        // Останавливает воспроизведение и освобождает используемые ресурсы
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
        // Возвращает текущую позицию воспроизведения в секундах
        public double GetCurrentPositionSeconds()
        {
            if (fileReader != null)
            {
                return fileReader.CurrentTime.TotalSeconds;
            }
            return 0.0;
        }
        // Возвращает полную продолжительность текущей композиции в секундах
        public double GetTotalDurationSeconds()
        {
            if (fileReader != null)
            {
                return fileReader.TotalTime.TotalSeconds;
            }
            return 0.0;
        }
        // Устанавливает новую позицию воспроизведения композиции
        public void SetPositionSeconds(double seconds)
        {
            if (fileReader != null)
            {
                fileReader.CurrentTime = TimeSpan.FromSeconds(seconds);
            }
        }
    }
}
