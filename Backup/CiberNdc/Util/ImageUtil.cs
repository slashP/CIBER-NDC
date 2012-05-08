using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CiberNdc.Util
{
    public static class ImageUtil
    {
        private static Stream ToStream(this Image image, ImageFormat formaw)
        {
            var stream = new MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }

        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            return ReadFully(image.ToStream(format));
        }

        public static Image Resize(this Image image, int width, int height)
        {
            var sourceWidth = image.Width;
            var sourceHeight = image.Height;
            var widthRatio = (width/(float) sourceWidth);
            var heightRatio = (height/(float) sourceHeight);
            var resizeRatio = heightRatio < widthRatio ? heightRatio : widthRatio;
            var destWidth = (int) (sourceWidth*resizeRatio);
            var destHeight = (int) (sourceHeight*resizeRatio);
            var resizedImage = new Bitmap(destWidth, destHeight);
            using (var g = Graphics.FromImage(resizedImage))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.DrawImage(image, new Rectangle(0, 0, resizedImage.Width, resizedImage.Height));
                return resizedImage;
            }
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            var returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[5*1024*1024]; // 5 MB.
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}