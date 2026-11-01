using System;
using System.IO;

namespace RAndG.Services
{
    public class AudioFileValidator
    {
        // Проверка физического присутствия файла предотвращает аварийные сбои декодера при чтении
        public bool ValidateFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }
            return File.Exists(filePath);
        }
    }
}
