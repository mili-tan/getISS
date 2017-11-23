using System;
using System.Text;

namespace Copernicus.SSURL
{
    public class SSURL
    {

        public static string[] Parse(string ssURL)
        {
            string urlString = deCodeBase64(ssURL.Replace("ss://", ""));
            string[] ssArray = urlString.Split(new char[2] { '@', ':' });
            return ssArray;
        }

        public static string Create(string encryptStr, string passStr, string serverIpStr, int port)
        {
            string linkStr = string.Format("{0}:{1}@{2}:{3}", encryptStr, passStr, serverIpStr, port.ToString());
            string ssUrlStr = string.Format("ss://{0}", enCodeBase64(linkStr));
            return ssUrlStr;
        }

        private static string enCodeBase64(string sourceStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(sourceStr);
            string enCode = Convert.ToBase64String(bytes);
            return enCode;
        }

        private static string deCodeBase64(string resultStr)
        {
            byte[] bytes = Convert.FromBase64String(resultStr);
            string deCode = Encoding.UTF8.GetString(bytes);
            return deCode;
        }
    }
}
