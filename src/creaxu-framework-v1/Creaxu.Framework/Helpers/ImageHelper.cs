using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Creaxu.Framework.Helpers
{
    public static class ImageHelper
    {
        public static Stream Resize(Stream source, int maxWidth)
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
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
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
    }
}
