using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SecuriText
{
    class Crypto
    {
        Aes encAes;
        RSACryptoServiceProvider encRSA;
        public Crypto()
        {
            encAes = Aes.Create();
            encRSA = new RSACryptoServiceProvider();
        }
        #region AES Related
        public static byte[] EncryptAES(string plainText, Aes aes)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            byte[] encrypted;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }

        public string DecryptAES(byte[] cipherText, Aes aes)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            string plaintext = null;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }
        #endregion
        #region HMAC Related
        public static string SignFile(HMACSHA256 hMac, String text, String destFile)
        {
            HMACSHA256 hmac = hMac;
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(hashValue);
        }
        public static bool VerifyFile(byte[] key, String filePath, string cypherText)
        {
            HMACSHA256 hmac = new HMACSHA256(key);
            string computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(cypherText)));
            string storedHash = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\HMAC.txt");
            if (computedHash == storedHash)
            {
                return true;
            }
            else
            {
                return false;
            }
        } //end VerifyFile
        #endregion
        #region RSA Related
        public static bool VerifySignedRSA(string clearText, string SignedData, RSACryptoServiceProvider encRSA)
        {
            try
            {
                byte[] textoAssinado = Convert.FromBase64String(SignedData);
                byte[] textolimpo = Encoding.UTF8.GetBytes(clearText);
                return encRSA.VerifyData(textolimpo, SHA256.Create(), textoAssinado);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }
        public static string RSASign(string texto, RSACryptoServiceProvider encRSA)
        {
            return Convert.ToBase64String(encRSA.SignData(Encoding.UTF8.GetBytes(texto), SHA256.Create()));
        }
        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }
        #endregion

        //getters
        public Aes getAES()
        {
            return encAes;
        }
        public RSACryptoServiceProvider getRSA()
        {
            return encRSA;
        }
    }
}
