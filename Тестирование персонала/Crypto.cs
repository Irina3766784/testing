using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Тестирование_персонала
{
    class Crypto
    {
        private static string _salt = "*1234567890!@#$%^&*()14344*";
        public static string Encode(string text)
        {
            var TripleDesProvider = new TripleDESCryptoServiceProvider();
            var hashmd5 = new MD5CryptoServiceProvider();
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(text);

            byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(_salt));
            hashmd5.Clear();

            TripleDesProvider.Key = keyArray;
            TripleDesProvider.Mode = CipherMode.ECB;
            TripleDesProvider.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = TripleDesProvider.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decode(string text)
        {
            try
            {
                var TripleDesProvider = new TripleDESCryptoServiceProvider();

                var hashmd5 = new MD5CryptoServiceProvider();
                byte[] toEncryptArray = Convert.FromBase64String(text);

                byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(_salt));

                hashmd5.Clear();

                TripleDesProvider.Key = keyArray;
                TripleDesProvider.Mode = CipherMode.ECB;
                TripleDesProvider.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = TripleDesProvider.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                TripleDesProvider.Clear();

                return Encoding.UTF8.GetString(resultArray);
                //return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
