using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace RAndG.Players
{
    internal class MusicPlayer
    {
        private WaveOutEvent? outputDevice;
        private AudioFileReader? audioFile;
        public void Play(string filePath)
        {
            Stop();
            audioFile = new AudioFileReader(filePath);
            outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            outputDevice.Play();
        }
        public void Stop()
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;
        }
    }
}
