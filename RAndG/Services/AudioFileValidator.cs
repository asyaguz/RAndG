using System;
using System.IO;

namespace RAndG.Services
{
    public class AudioFileValidator
    {
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
