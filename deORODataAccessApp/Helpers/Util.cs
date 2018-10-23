using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.Helpers
{
    public class Util
    {
        public static string GetPasswordHash(string password, string salt)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(password + salt);
            SHA256 s = new SHA256CryptoServiceProvider();

            return BitConverter.ToString(s.ComputeHash(inputBytes)).Replace("-", "").ToLower();
        }

        public static string GetRandomSalt(int length = 25)
        {
            RNGCryptoServiceProvider rncCsp = new RNGCryptoServiceProvider();
            byte[] salt = new byte[length];
            rncCsp.GetBytes(salt);

            return BitConverter.ToString(salt).Replace("-", "");
        }

        public static string GetRandomPassword()
        {
            string path = Path.GetRandomFileName();
            return path.Replace(".", "");
        }
    }
}
