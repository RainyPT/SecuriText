using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace SecuriText
{
    public partial class Editor : Form
    {
        private String filePath;
        Aes encAes;
        RSACryptoServiceProvider encRSA;
        public Editor(String path)
        {
            InitializeComponent();
            filePath = path;
            encAes = Aes.Create();
            encRSA = new RSACryptoServiceProvider();
            encAes.Padding = PaddingMode.None;
        }

        private void Editor_Load(object sender, EventArgs e)
        {
            if (filePath != "")
            {
                authCheckBox.Visible =false;
                cypherCheckBox.Visible = false;
                String keyAndIV = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\key-and-iv.txt");
                String[] keyIV = keyAndIV.Split("|");

                if (Path.GetExtension(filePath) == ".auth")
                {
                    String originalHash = keyIV[2];
                    String receivedHash = DecryptAES(Convert.FromBase64String(File.ReadAllText(filePath)), Convert.FromBase64String(keyIV[0]), Convert.FromBase64String(keyIV[1]));
                    encAes.Key = Convert.FromBase64String(keyIV[0]);
                    encAes.IV = Convert.FromBase64String(keyIV[1]);

                    textHandle.Text = receivedHash;

                    try
                    {
                        if (verifyHash(receivedHash.Split("Hash:")[1], originalHash))
                        {
                            MessageBox.Show("MAC verificado com sucesso");
                        }
                        else
                        {
                            MessageBox.Show("MAC diferente!A claire anda a fazer das suas outra vez...");
                        }
                    }
                    catch (IndexOutOfRangeException id)
                    {
                        MessageBox.Show("Ficheiro corrompido!\n\n" + id);
                    }
                }
                if (Path.GetExtension(filePath) == ".enc")
                {
                    String privateKey = keyIV[0];
                    encRSA.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                    String decriptedText = Encoding.UTF8.GetString(encRSA.Decrypt(Convert.FromBase64String(File.ReadAllText(filePath)), RSAEncryptionPadding.Pkcs1));
                    textHandle.Text = decriptedText;
                }
                if (Path.GetExtension(filePath) == ".encAuth")
                {
                    //mais magias :D
                }

            }
        }
        private bool verifyHash(String newHash,String originalHash)
        {
            return newHash.Equals(originalHash);
        }

        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
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
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }
        static byte[] EncryptAES(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        static string DecryptAES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

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
            }

            return plaintext;
        }
        private string HaSHA256(String text)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        private string GerarMAC(String text, byte[] key, byte[] IV)
        {
            return Convert.ToBase64String(EncryptAES("Hash:"+HaSHA256(text), key, IV));
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
           
            if (filePath == "")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "enc";
                if (authCheckBox.Checked)
                {
                    sfd.Filter = "Authenticated File|*.auth";
                }
                if (authCheckBox.Checked && cypherCheckBox.Checked)
                {
                    sfd.Filter = "Encrypted&Authenticated|*.encAuth";
                }
                if (cypherCheckBox.Checked)
                {
                    sfd.Filter = "Encrypted File|*.enc";
                }
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (authCheckBox.Checked)
                    {
                        string keyandiv = Convert.ToBase64String(encAes.Key) + "|" + Convert.ToBase64String(encAes.IV)+"|"+ HaSHA256(textHandle.Text);
                        File.WriteAllText(sfd.FileName,GerarMAC(textHandle.Text, encAes.Key, encAes.IV));
                        File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\key-and-IV.txt",keyandiv);

                    }
                    if (cypherCheckBox.Checked)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(textHandle.Text);
                        File.WriteAllText(sfd.FileName, Convert.ToBase64String(encRSA.Encrypt(data,false)));
                        File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\key-and-IV.txt", Convert.ToBase64String(encRSA.ExportRSAPrivateKey()) + "|" + Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
                    }
                    if (cypherCheckBox.Checked && authCheckBox.Checked)
                    {
                        //Magias
                    }
                    MessageBox.Show("Ficheiro guardado com sucesso!");
                }
                else
                {
                    MessageBox.Show("Problema ao guardar o ficheiro!");
                }
            }
            else
            {
                if (Path.GetExtension(filePath) == ".auth")
                {
                    File.WriteAllText(filePath, Convert.ToBase64String(EncryptAES(textHandle.Text,encAes.Key,encAes.IV)));
                }
                if (Path.GetExtension(filePath) == ".enc")
                {
                    byte[] data = Encoding.UTF8.GetBytes(textHandle.Text);
                    File.WriteAllText(filePath, Convert.ToBase64String(encRSA.Encrypt(data, false)));
                    File.WriteAllText(Path.GetDirectoryName(filePath) + "\\key-and-IV.txt", Convert.ToBase64String(encRSA.ExportRSAPrivateKey()) + "|" + Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
                }
                if (Path.GetExtension(filePath) == ".encAuth")
                {
                    byte[] data = Encoding.UTF8.GetBytes(textHandle.Text);
                    File.WriteAllText(filePath, Convert.ToBase64String(encRSA.Encrypt(data, false)));
                }
                MessageBox.Show("Ficheiro guardado com sucesso!");
            }
            Editor.ActiveForm.Close();

        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("AQUI");
        }

    }
}
