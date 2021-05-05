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
                if (Path.GetExtension(filePath) == ".auth")
                {
                    string[] keysAndIVSplitted = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt").Split("|");
                    Aes aes = Aes.Create();
                    aes.Key = Convert.FromBase64String(keysAndIVSplitted[1]);
                    aes.IV = Convert.FromBase64String(keysAndIVSplitted[2]);
                    byte[] keyHMAC = Convert.FromBase64String(keysAndIVSplitted[0]);
                    byte[] cypherText= Convert.FromBase64String(File.ReadAllText(filePath));
                    string decryptedString = DecryptAES(cypherText, aes);
                    if (VerifyFile(keyHMAC, filePath, decryptedString))
                        MessageBox.Show("Os HMACs são iguais tanto no ficheiro Original, como no ficheiro recebido.");
                    else
                        MessageBox.Show("Os HMACs diferem! A Claire está a fazer das suas...");

                    textHandle.Text = decryptedString;
                }
                if (Path.GetExtension(filePath) == ".enc")
                {
                    String PKSK = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\PK-SK.txt");
                    string[] keyIV = PKSK.Split("|");
                    String privateKey = keyIV[0];
                    encRSA.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                    String decriptedText = Encoding.UTF8.GetString(encRSA.Decrypt(Convert.FromBase64String(File.ReadAllText(filePath)), RSAEncryptionPadding.Pkcs1));
                    textHandle.Text = decriptedText;
                }
                if (Path.GetExtension(filePath) == ".encAuth")
                {
                    //MoreMagics
                }

            }
        }
        #region AES Related
        static byte[] EncryptAES(string plainText,Aes aes)
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

        static string DecryptAES(byte[] cipherText, Aes aes)
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
        public static void SignFile(HMACSHA256 hMac, String text, String destFile)
        {
            HMACSHA256 hmac = hMac;
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
            File.WriteAllText(Path.GetDirectoryName(destFile)+"\\HMAC.txt", Convert.ToBase64String(hashValue));
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
                        HMACSHA256 hmac = new HMACSHA256();
                        Aes aes = Aes.Create();
                        SignFile(hmac, textHandle.Text, sfd.FileName);
                        File.WriteAllText(sfd.FileName,Convert.ToBase64String(EncryptAES(textHandle.Text,aes)));
                        File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\keys-and-IV.txt",Convert.ToBase64String(hmac.Key)+"|"+ Convert.ToBase64String(aes.Key)+ "|" + Convert.ToBase64String(aes.IV));

                    }
                    if (cypherCheckBox.Checked)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(textHandle.Text);
                        File.WriteAllText(sfd.FileName, Convert.ToBase64String(encRSA.Encrypt(data,false)));
                        File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\PK-SK.txt", Convert.ToBase64String(encRSA.ExportRSAPrivateKey()) + "|" + Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
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
                    string[] keysAndIVSplitted = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt").Split("|");
                    HMACSHA256 hmac = new HMACSHA256();
                    Aes aes = Aes.Create();
                    //SignFile(hmac, textHandle.Text, filePath);
                    aes.Key = Convert.FromBase64String(keysAndIVSplitted[1]);
                    aes.IV = Convert.FromBase64String(keysAndIVSplitted[2]);
                    File.WriteAllText(filePath, Convert.ToBase64String(EncryptAES(textHandle.Text,aes)));
                    File.WriteAllText(Path.GetDirectoryName(filePath) + "\\keys-and-IV.txt", Convert.ToBase64String(hmac.Key) + "|" + Convert.ToBase64String(aes.Key) + "|" + Convert.ToBase64String(aes.IV));
                }
                if (Path.GetExtension(filePath) == ".enc")
                {
                    byte[] data = Encoding.UTF8.GetBytes(textHandle.Text);
                    File.WriteAllText(filePath, Convert.ToBase64String(encRSA.Encrypt(data, false)));
                    File.WriteAllText(Path.GetDirectoryName(filePath) + "\\PK-SK.txt", Convert.ToBase64String(encRSA.ExportRSAPrivateKey()) + "|" + Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
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
            HelpForm help = new HelpForm();
            help.Show();
        }

    }
}
