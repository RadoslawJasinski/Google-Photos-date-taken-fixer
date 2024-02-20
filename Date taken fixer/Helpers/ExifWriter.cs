using Date_taken_fixer.Models;
using ExifLibrary;

namespace Date_taken_fixer.Helpers
{
    public class ExifWriter
    {
        private static GeolocationDMS ConvertToDMS(double coordinate, bool isLatitude)
        {
            char direction = isLatitude ? (coordinate >= 0 ? 'N' : 'S') : (coordinate >= 0 ? 'E' : 'W');

            coordinate = Math.Abs(coordinate);

            int degrees = (int)Math.Floor(coordinate);
            double minutesAndSeconds = (coordinate - degrees) * 60;
            int minutes = (int)Math.Floor(minutesAndSeconds);
            double seconds = (minutesAndSeconds - minutes) * 60;

            return new GeolocationDMS { Degrees = degrees, Minutes = minutes, Seconds = seconds, Direction = direction };
        }

        public void Write(string filePath, double newLatitude, double newLongitude, double newAltitude)
        {
            ImageFile file = ImageFile.FromFile(filePath);

            GeolocationDMS latidudeDMS = ConvertToDMS(newLatitude, true);
            GeolocationDMS longitudeDMS = ConvertToDMS(newLongitude, false);

            file.Properties.Set(ExifTag.GPSLatitude, (float)latidudeDMS.Degrees, (float)latidudeDMS.Minutes, (float)latidudeDMS.Seconds);
            file.Properties.Set(ExifTag.GPSLatitudeRef, latidudeDMS.Direction == 'N' ? GPSLatitudeRef.North : GPSLatitudeRef.South);

            file.Properties.Set(ExifTag.GPSLongitude, (float)longitudeDMS.Degrees, (float)longitudeDMS.Minutes, (float)longitudeDMS.Seconds);
            file.Properties.Set(ExifTag.GPSLongitudeRef, longitudeDMS.Direction == 'W' ? GPSLongitudeRef.West : GPSLongitudeRef.East);

            file.Properties.Set(ExifTag.GPSAltitude, Math.Abs(newAltitude));
            file.Properties.Set(ExifTag.GPSAltitudeRef, newAltitude > 0 ? GPSAltitudeRef.AboveSeaLevel : GPSAltitudeRef.BelowSeaLevel);

            file.Save(filePath);
        }
    }
}
