using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AWSRekognitionDemo.Converters
{
    public class BitmapToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                ((System.Drawing.Bitmap)value).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = new MemoryStream(ms.ToArray());
                image.EndInit();

                return image;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
