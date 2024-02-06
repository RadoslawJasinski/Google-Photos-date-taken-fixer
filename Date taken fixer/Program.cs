using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System;
using ExifLibrary;
using System.IO;

namespace Date_taken_fixer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Podaj sciezke zdjec:");
            string? folderPath = Console.ReadLine();

            try
            {
                string[] files = Directory.GetFiles(folderPath);

                foreach (string filePath in files)
                {
                    if (File.Exists(filePath))
                    {
                        DateTime modifiedDate = File.GetLastWriteTime(filePath);

                        File.SetCreationTime(filePath, modifiedDate);
                        File.SetLastWriteTime(filePath, modifiedDate);
                        File.SetLastAccessTime(filePath, modifiedDate);

                    }
                    else
                    {
                        Console.WriteLine("File not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }

    }
}