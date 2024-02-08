using Newtonsoft.Json;
using Date_taken_fixer.Models;
using Date_taken_fixer.Helpers;

namespace Date_taken_fixer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path of the photos:");
            string? folderPath = Console.ReadLine();

            try
            {
                if (folderPath == null)
                    return;

                string[] allFilesPath = Directory.GetFiles(folderPath);
                string[] jsonMetadataPaths = allFilesPath.Where(file => file.EndsWith(".json")).ToArray();
                string[] mediaFilePaths = allFilesPath.Except(jsonMetadataPaths).ToArray();

                List<string> mediaFilesWithoutMetadata = new();

                foreach (string mediaFilePath in mediaFilePaths)
                {
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

                        var exifWriter = new ExifWriter();
                        exifWriter.Write(mediaFilePath, newLatitude, newLongitude, newAltitude);

                    }
                    else
                    {
                        mediaFilesWithoutMetadata.Add(mediaFilePath);
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