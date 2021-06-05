using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Creaxu.Core.Helpers
{
    public static class ImageHelper
    {
        public static byte[] CropToSquare(byte[] input, int size)
        {
            var ms = new MemoryStream(input);
            var bmp = Image.FromStream(ms) as Bitmap;
            
            var target = new Bitmap(size, size);
            
            using (var g = Graphics.FromImage(target))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, size, size);
                
                int t = 0, l = 0;
                if (bmp.Height > bmp.Width)
                {
                    t = (bmp.Height - bmp.Width) / 2;
                }
                else
                {
                    l = (bmp.Width - bmp.Height) / 2;
                }

                g.DrawImage(bmp, new Rectangle(0, 0, size, size),
                    new Rectangle(l, t, bmp.Width - l * 2, bmp.Height - t * 2), GraphicsUnit.Pixel);
                    
                var output = new MemoryStream();
                target.Save(output, ImageFormat.Png);
                output.Seek(0, SeekOrigin.Begin);
                
                var result = new byte[output.Length];
                output.Read(result, 0, (int)output.Length);
                
                return result;
            }
        }
        
        private static Bitmap CopyDataToBitmap(byte[] data)
        {
            //Here create the Bitmap to the know height, width and format
            Bitmap bmp = new Bitmap( 352, 288, PixelFormat.Format24bppRgb);  

            //Create a BitmapData and Lock all pixels to be written 
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),   
                ImageLockMode.WriteOnly, bmp.PixelFormat);
 
            //Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
                
                
            //Unlock the pixels
            bmp.UnlockBits(bmpData);


            //Return the bitmap 
            return bmp;
        }
        
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
