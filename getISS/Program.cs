using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using Newtonsoft.Json.Linq;
using Copernicus.SSURL;
using System.Diagnostics;
using System.IO.Compression;
using System.Windows.Forms;

namespace getISS
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            JArray clientArray = new JArray();
            if (!Directory.Exists("./qrcode"))
            {
                Directory.CreateDirectory("./qrcode");
            }

            WebClient myWebClient = new WebClient();

            if (!File.Exists("./list-ssqrcode.text"))
            {
                try
                {
                    myWebClient.DownloadFile("https://g-mi.gear.host/ss/ss.txt", "./list-ssqrcode.text");
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show("获取列表文件失败\n\r" + e.Message);
                }

            }

            string[] qrCodeURLs = File.ReadAllLines("./list-ssqrcode.text");

            foreach (string item in qrCodeURLs)
            {
                myWebClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.2767.0 Safari/537.36";
                string myUrl = item;
                string[] fileShortFile = myUrl.Split('/');
                string fileName = string.Format(@".\qrcode\{0}", fileShortFile[fileShortFile.Count() - 1]);

                try
                {
                    myWebClient.DownloadFile(myUrl, fileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("获取二维码失败,请尝试重新获取列表文件\n\r" + e.Message);
                }

                if (File.Exists(fileName))
                {
                    QRCodeDecoder myDecoder = new QRCodeDecoder();
                    string mySSURL = myDecoder.decode(new QRCodeBitmapImage(new Bitmap(Image.FromFile(fileName))));
                    string linkNameMark = fileShortFile[fileShortFile.Count() - 1].Replace(".png", "").Replace(".jpg", "").Replace("xxoo", "");
                    string[] linkInfo = SSURL.Parse(mySSURL);

                    SSClientInfo client = new SSClientInfo();
                    client.remarks = linkNameMark;
                    client.method = linkInfo[0];
                    client.password = linkInfo[1];
                    client.server = linkInfo[2];
                    client.server_port = Convert.ToInt32(linkInfo[3]);
                    client.timeout = 5;
                    clientArray.Add(JObject.FromObject(client));
                }
            }

            JObject configs = new JObject();
            configs["configs"] = clientArray;
            configs["enabled"] = true;
            File.WriteAllText("gui-config.json", configs.ToString());

            if (File.Exists("./Shadowsocks.exe"))
            {
                Process.Start("Shadowsocks.exe");
                Process.Start("explorer.exe", "https://www.google.com/");
            }
            else
            {
                myWebClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.2767.0 Safari/537.36";
                string ssInfo = myWebClient.DownloadString("https://api.github.com/repos/shadowsocks/shadowsocks-windows/releases/latest");
                JObject ssInfoJObj = JObject.Parse(ssInfo);
                JArray assets = JArray.Parse(ssInfoJObj["assets"].ToString());
                JObject assets1 = JObject.Parse(assets[0].ToString());
                string dlURL = assets1["browser_download_url"].ToString();


                if (MessageBox.Show("没有找到Shadowsocks客户端主程序,需要下载吗？\n\r下载可能需要数分钟的时间,请坐和放宽。"+dlURL, "没有找到Shadowsocks主程序", MessageBoxButtons.OKCancel,MessageBoxIcon.Information) == DialogResult.OK)
                {
                    try
                    {
                        myWebClient.DownloadFile(dlURL, "./ss.zip");
                    }
                    catch (Exception e)
                    {
                        if (MessageBox.Show("下载失败，要更换载点重新下载吗？\n\r" + e.Message, "Shadowsocks主程序下载失败", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            myWebClient.DownloadFile("http://160.16.231.71/ss-3.4.3.zip", "./ss.zip");
                        }
                    }
                    if (File.Exists("./ss.zip"))
                    {
                        ZipFile.ExtractToDirectory("./ss.zip", "./");
                    }
                    if (File.Exists("./Shadowsocks.exe"))
                    {
                        Process.Start("Shadowsocks.exe");
                        Process.Start("explorer.exe", "https://www.google.com/");
                        File.Delete("./ss.zip");
                    }
                }
            }
        }

        class SSClientInfo
        {
            public string server { get; set; }
            public string password { get; set; }
            public string method { get; set; }
            public string remarks { get; set; }
            public int server_port { get; set; }
            public int timeout { get; set; }
        }
    }
}
