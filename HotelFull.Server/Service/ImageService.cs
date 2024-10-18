using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
public class ImageService
    {
        public Image ReturnPhoto(byte[] streamByte)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(streamByte);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            return img;
        }


    }
