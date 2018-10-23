using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace deOROservice.Classes
{
    public static class EncryptionHelper
    {
        public static string SHA256Encrypt(string _StringToEncrypt, string _SALTkey)
        {
            //string _Salt = "6D9988BEC92B957A6FBB64F1F0EA7C5414D406CBC81B622BB0";
            string _SaltAndPassword = String.Concat(_StringToEncrypt, _SALTkey);
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(_SaltAndPassword));
            string hashedPassword = byteArrayToString(hashedDataBytes);
            return hashedPassword.ToLower();
        }

        public static string byteArrayToString(byte[] inputArray)
        {
            StringBuilder output = new StringBuilder("");
            for (int i = 0; i < inputArray.Length; i++)
            {
                output.Append(inputArray[i].ToString("X2"));
            }
            return output.ToString();
        }

        private static string CreateSalt(string UserName)
        {
            string username = UserName;
            byte[] userBytes;
            string salt;
            userBytes = ASCIIEncoding.ASCII.GetBytes(username);
            long XORED = 0x00;

            foreach (int x in userBytes)
                XORED = XORED ^ x;

            Random rand = new Random(Convert.ToInt32(XORED));
            salt = rand.Next().ToString();
            salt += rand.Next().ToString();
            salt += rand.Next().ToString();
            salt += rand.Next().ToString();
            return salt;
        }
    }
}