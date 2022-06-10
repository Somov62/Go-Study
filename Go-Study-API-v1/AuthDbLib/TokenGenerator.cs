using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using DataBaseCore;

namespace AuthDbLib
{
    public class TokenGenerator
    {
        private readonly Random _random = new Random();
        private readonly string _symbols = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
        public  string GenerateToken(User user)
        {
            string data = RemixString(user.Login, user.Login.Length);
            data += RemixString(user.UserName, user.UserName.Length);
            data += RemixString(_symbols, 15);

            return Encrypt(data);
        }


        private string RemixString(string str, int outputLength)
        {
            string output = string.Empty;
            List<int> strIndexes = new List<int>();
            for (int i = 0; i < outputLength; i++)
            {
                int index = _random.Next(str.Length);
                while (strIndexes.Contains(index))
                {
                    index = _random.Next(str.Length);
                }
                output += str[index];
            }
            return output;
        }

        private string Encrypt(string token)
        {
            token = Convert.ToBase64String(Encoding.ASCII.GetBytes(token)).Replace("=", "");
            MD5CryptoServiceProvider mD5 = new MD5CryptoServiceProvider();
            var hashArray = mD5.ComputeHash(Encoding.UTF8.GetBytes(token));
            string hashString = string.Empty;
            foreach (var item in hashArray)
            {
                hashString += item.ToString("X2");
            }
            return hashString;
        }
    }
}
