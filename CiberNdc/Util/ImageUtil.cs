using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CiberNdc.Util
{
    public static class ImageUtil
    {
        public static Stream ToStream(this Image image, ImageFormat formaw)
        {
            var stream = new MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }

        public static Stream ResizeImage(this Image imgToResize, Size size, ImageFormat imageFormat)
        {
            return ResizeImage(imgToResize, size, imageFormat, false);
        }

        public static Stream ResizeImage(this Image imgToResize, Size size, ImageFormat imageFormat, bool crop)
        {
            if (!crop)
            {
            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;

            float nPercent;
            var nPercentW = (size.Width / (float)sourceWidth);
            var nPercentH = (size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);
            var testHeight = (int)destHeight;

            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, testHeight);
            g.Dispose();

            return b.ToStream(imageFormat);
                
            }else
            {
                var sourceWidth = imgToResize.Width;
                var sourceHeight = imgToResize.Height;

                float nPercent;
                var nPercentW = (size.Width / (float)sourceWidth);
                var nPercentH = (size.Height / (float)sourceHeight);
                var hStart = 0;
                var vStart = 0;

                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentW;
                    vStart = (int)(((float)sourceHeight / 2)*nPercent - (float)size.Height / 2 );
                }
                else
                {
                    nPercent = nPercentH;
                    hStart =(int)((((float)sourceWidth / 2)*nPercent) - (float)size.Width / 2 );
                }

                var scaleWidth = (int)(sourceWidth * nPercent);
                var scaleHeight = (int)(sourceHeight * nPercent);
                
                var b = new Bitmap(size.Width, size.Height);
                var g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(imgToResize, -hStart, -vStart, scaleWidth, scaleHeight);
                g.Dispose();

                return b.ToStream(imageFormat);
            }
        }

        public static string[] AllowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/pjpeg" };

        public static ImageFormat GetImageFormatFromFileExtension(string fileExtension)
        {
            switch (fileExtension.ToUpperInvariant())
            {
                case "JPG":
                    return ImageFormat.Jpeg;
                case "PNG":
                    return ImageFormat.Png;
                case "GIF":
                    return ImageFormat.Gif;
            }
            return ImageFormat.Jpeg;
        }

        public static ImageFormat GetImageFormat(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                case "image/pjpeg":
                    return ImageFormat.Jpeg;
                case "image/png":
                    return ImageFormat.Png;
                case "image/gif":
                    return ImageFormat.Gif;
            }
            return ImageFormat.Png;
        }

        public static string GetImageContentType(string contentType)
        {
            switch (contentType.ToUpperInvariant())
            {
                case "JPEG":
                case "JPG":
                    return "image/jpeg";
                case "PNG":
                    return "image/png";
                case "GIF":
                    return "image/gif";
            }
            return "image/jpeg";
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            var returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}