using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

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
                mySSURL += "#" + fileShortFile[fileShortFile.Count() - 1].Replace(".png", "").Replace(".jpg", "");
                Console.WriteLine(mySSURL);
            }
            Console.ReadKey();
        }
    }
}
