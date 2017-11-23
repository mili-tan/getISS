using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using Copernicus.SSURL;

namespace getISS
{
    class Program
    {
        static void Main(string[] args)
        {

            if (!Directory.Exists("./qrcode"))
            {
                Directory.CreateDirectory("./qrcode");
            }

            string myUrl = "";
            string[] fileShortFile = myUrl.Split('/');
            string fileName = string.Format(@".\qrcode\{0}", fileShortFile[fileShortFile.Count() - 1]);
            WebClient myWebClient = new WebClient();
            myWebClient.DownloadFile(myUrl, fileName);
            if (File.Exists(fileName))
            {
                QRCodeDecoder myDecoder = new QRCodeDecoder();
                string mySSURL =  myDecoder.decode(new QRCodeBitmapImage(new Bitmap(Image.FromFile(fileName))));
                string linkName = fileShortFile[fileShortFile.Count() - 1].Replace(".png", "").Replace(".jpg", "");
                string[]  linkInfo = SSURL.Parse(mySSURL);

            }

            Console.ReadKey();
        }
    }
}
