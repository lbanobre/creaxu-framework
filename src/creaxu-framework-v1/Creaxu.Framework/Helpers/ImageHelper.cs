using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Creaxu.Framework.Helpers
{
    public static class ImageHelper
    {
        public static Stream Resize(Stream source, int maxWidth, CompositingQuality compositingQuality = CompositingQuality.HighSpeed)
        {
            using (var image = new Bitmap(source))
            {
                int width = image.Width, height = image.Height;
                if (image.Width > maxWidth)
                {
                    width = maxWidth;
                    height = Convert.ToInt32(image.Height * maxWidth / (double)image.Width);
                }
                
                var resized = new Bitmap(width, height);

                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingQuality = compositingQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(image, 0, 0, width, height);
                    var output = new MemoryStream();
                    resized.Save(output, ImageFormat.Png);
                    output.Seek(0, SeekOrigin.Begin);
                    return output;
                }
            }
        }

        public static byte[] Resize(byte[] source, int maxWidth, CompositingQuality compositingQuality = CompositingQuality.HighSpeed)
        {
            var stream = Resize(new MemoryStream(source), maxWidth, compositingQuality);

            byte[] result = new byte[stream.Length];
            stream.Read(result, 0, (int)stream.Length);

            return result;
        }

        public static string ConvertToBase64(Stream input)
        {
           var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
