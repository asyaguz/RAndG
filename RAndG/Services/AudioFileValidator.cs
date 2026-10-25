using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;

namespace RAndG.Services
{
    public static class AudioFileValidator
    {
        public static bool TryValidate(string? filePath, out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(filePath))
            {
                error = "Не указан путь к музыкальному файлу.";
                return false;
            }
            try
            {
                if (!File.Exists(filePath))
                {
                    error = "Файл отсутствует или был перемещён.";
                    return false;
                }
                string extension = Path.GetExtension(filePath).ToLowerInvariant();
                if (extension != ".mp3" && extension != ".wav" && extension != ".aiff" && extension != ".aif" && extension != ".wma")
                {
                    error = "Формат файла не поддерживается. Выберите MP3, WAV, AIFF или WMA.";
                    return false;
                }
                // Проверяем, что аудиофайл действительно открывается декодером.
                using AudioFileReader reader = new(filePath);
                if (reader.TotalTime <= TimeSpan.Zero)
                {
                    error = "Аудиофайл пустой или повреждён.";
                    return false;
                }
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException or InvalidOperationException or System.Runtime.InteropServices.COMException)
            {
                error = GetFriendlyError(ex);
                return false;
            }
        }
        public static string GetFriendlyError(Exception ex)
        {
            return ex switch
            {
                FileNotFoundException => "Файл не найден. Возможно, он был удалён или перемещён.",
                DirectoryNotFoundException => "Папка с музыкальным файлом больше не существует.",
                UnauthorizedAccessException => "Нет доступа к файлу. Проверьте разрешения Windows.",
                PathTooLongException => "Путь к музыкальному файлу слишком длинный.",
                InvalidDataException => "Аудиофайл повреждён или содержит некорректные данные.",
                NotSupportedException => "Формат аудиофайла не поддерживается.",
                System.Runtime.InteropServices.COMException => "Windows не смогла открыть аудиофайл или аудиоустройство.",
                IOException => "Ошибка чтения файла. Возможно, он занят другой программой или недоступен.",
                ArgumentException => "Указан некорректный путь или формат аудиофайла.",
                _ => "Не удалось выполнить операцию. Проверьте файл и попробуйте ещё раз."
            };
        }
    }
}
