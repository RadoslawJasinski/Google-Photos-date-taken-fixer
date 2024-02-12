using Newtonsoft.Json;
using Date_taken_fixer.Models;
using Date_taken_fixer.Helpers;
using System.Globalization;
using System.Threading.Channels;

namespace Date_taken_fixer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? folderPathWithMedia = string.Empty;
            string? enteredPath = string.Empty;
            bool foundMediaFiles = false;

            do
            {
                Console.Clear();
                Console.WriteLine("Enter the path of the Takeout folder:");
                enteredPath = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(enteredPath) || !Directory.Exists(enteredPath))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please enter a valid folder path. Press any key to continue.");
                    Console.ReadKey();
                }
                else
                {
                    var subfolders = Directory.GetDirectories(enteredPath);
                    string[] extensions = new[] { ".jpg", "jpeg", ".tiff", ".bmp", ".mp4" };

                    if (subfolders.Count() == 0)
                    {
                        foundMediaFiles = Directory.EnumerateFiles(enteredPath, "*.*", SearchOption.AllDirectories)
                                                    .Any(file => extensions.Any(ext => string.Equals(Path.GetExtension(file), ext, StringComparison.OrdinalIgnoreCase)));

                        if (foundMediaFiles)
                            folderPathWithMedia = enteredPath;
                    }
                    else
                    {
                        foreach (string subfolder in subfolders)
                        {
                            foundMediaFiles = Directory.EnumerateFiles(subfolder, "*.*", SearchOption.AllDirectories)
                                                        .Any(file => extensions.Any(ext => string.Equals(Path.GetExtension(file), ext, StringComparison.OrdinalIgnoreCase)));

                            if (foundMediaFiles)
                            {
                                folderPathWithMedia = subfolder;
                                break;
                            }
                        }
                    }
                    if (!foundMediaFiles)
                    {
                        Console.WriteLine("Files not found. Press any key to continue.");
                        Console.ReadKey();
                    }
                }
            }
            while (foundMediaFiles == false);

            try
            {
                List<string> mediaFilesWithoutMetadata = new();
                string[] directoriesWithMedia = Directory.GetDirectories(folderPathWithMedia);

                int quantityOfMediaFiles = Directory.GetFiles(folderPathWithMedia, "*", SearchOption.AllDirectories)
                                                    .Where(file => !file.EndsWith(".json"))
                                                    .Count();

                Console.Clear();
                Console.WriteLine($"Found {quantityOfMediaFiles} media files.");
                Thread.Sleep(1000);
                Console.Clear();
                int counter = 1;

                foreach (string directoryWithMedia in directoriesWithMedia)
                {
                    string[] allFilesPath = Directory.GetFiles(directoryWithMedia);
                    string[] jsonMetadataPaths = allFilesPath.Where(file => file.EndsWith(".json")).ToArray();
                    string[] mediaFilePaths = allFilesPath.Except(jsonMetadataPaths).ToArray();


                    foreach (string mediaFilePath in mediaFilePaths)
                    {
                        Console.Clear();
                        Console.WriteLine($"{counter}/{quantityOfMediaFiles}");
                        string? jsonMetadataPath = jsonMetadataPaths.FirstOrDefault(x => x.Equals(mediaFilePath + ".json"));

                        if (jsonMetadataPath != null)
                        {
                            var jsonFile = File.ReadAllText(jsonMetadataPath);
                            PhotoMetadata photoData = JsonConvert.DeserializeObject<PhotoMetadata>(jsonFile) ?? new();

                            if (photoData.PhotoTakenTime != null)
                            {
                                long photoTimestamp = photoData.PhotoTakenTime.Timestamp;
                                var photoTakenDate = DateTimeOffset.FromUnixTimeSeconds(photoTimestamp).DateTime;
                                File.SetCreationTime(mediaFilePath, photoTakenDate);
                                File.SetLastWriteTime(mediaFilePath, photoTakenDate);
                            }
                            else
                            {
                                mediaFilesWithoutMetadata.Add(mediaFilePath);
                            }

                            double newLatitude = photoData.GeoData != null ? photoData.GeoData.Latitude : 0;
                            double newLongitude = photoData.GeoData != null ? photoData.GeoData.Longitude : 0;
                            double newAltitude = photoData.GeoData != null ? photoData.GeoData.Altitude : 0;

                            try
                            {
                                var exifWriter = new ExifWriter();
                                exifWriter.Write(mediaFilePath, newLatitude, newLongitude, newAltitude);
                            }
                            catch
                            {
                                //TODO: Add GPS support for movies
                            }


                        }
                        else
                        {
                            mediaFilesWithoutMetadata.Add(mediaFilePath);
                        }
                        counter++;
                    }

                }
                Console.Clear();
                if (mediaFilesWithoutMetadata.Count > 0)
                {
                    Console.WriteLine($"Not found {mediaFilesWithoutMetadata.Count} metadata files.");
                    Console.WriteLine("Skipped files:");
                    foreach ( var mediaFile in mediaFilesWithoutMetadata)
                    {
                        Console.WriteLine(Path.GetFileName(mediaFile));
                    }
                }
                Console.WriteLine($"Done. Converted {quantityOfMediaFiles - mediaFilesWithoutMetadata.Count} files. Press any key to exit.");
                Console.ReadKey();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

    }
}