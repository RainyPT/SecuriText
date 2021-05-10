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
        Aes encAes;
        RSACryptoServiceProvider encRSA;
        string filePath="";
        public Editor()
        {
            InitializeComponent();
            encAes = Aes.Create();
            encRSA = new RSACryptoServiceProvider();
            encAes.Padding = PaddingMode.None;
        }
        #region AES Related
        static byte[] EncryptAES(string plainText, Aes aes)
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
        private void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void abrirItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Encrypted/Authed Files|*.enc;*.auth;*.encAuth";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filePath = dialog.FileName;
                if (Path.GetExtension(filePath) == ".auth")
                {
                    string[] keysAndIVSplitted = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt").Split("|");
                    Aes aes = Aes.Create();
                    aes.Key = Convert.FromBase64String(keysAndIVSplitted[1]);
                    aes.IV = Convert.FromBase64String(keysAndIVSplitted[2]);
                    byte[] keyHMAC = Convert.FromBase64String(keysAndIVSplitted[0]);
                    byte[] cypherText = Convert.FromBase64String(File.ReadAllText(filePath));
                    string decryptedString = DecryptAES(cypherText, aes);
                    if (VerifyFile(keyHMAC, filePath, decryptedString))
                        MessageBox.Show("Os HMACs são iguais tanto no ficheiro Original, como no ficheiro recebido.");
                    else
                        MessageBox.Show("Os HMACs diferem! A Claire está a fazer das suas...");

                    textoBox.Text = decryptedString;
                }
                if (Path.GetExtension(filePath) == ".enc")
                {
                    String PKSK = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\SK-PK.pem");
                    string[] keyIV = PKSK.Split("|");
                    String privateKey = keyIV[0];
                    encRSA.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                    String decriptedText = Encoding.UTF8.GetString(encRSA.Decrypt(Convert.FromBase64String(File.ReadAllText(filePath)), RSAEncryptionPadding.Pkcs1));
                    textoBox.Text = decriptedText;
                }
                if (Path.GetExtension(filePath) == ".encAuth")
                {
                    string[] keysAndIVSplitted = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt").Split("|");
                    String PKSK = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\SK-PK.pem");
                    string[] keyIV = PKSK.Split("|");
                    String privateKey = keyIV[0];
                    encRSA.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                    byte[] decriptedText = encRSA.Decrypt(Convert.FromBase64String(File.ReadAllText(filePath)), RSAEncryptionPadding.Pkcs1);
                    Aes aes = Aes.Create();
                    aes.Key = Convert.FromBase64String(keysAndIVSplitted[1]);
                    aes.IV = Convert.FromBase64String(keysAndIVSplitted[2]);
                    byte[] keyHMAC = Convert.FromBase64String(keysAndIVSplitted[0]);
                    byte[] cypherText = Convert.FromBase64String(File.ReadAllText(filePath));
                    string decryptedString = DecryptAES(decriptedText, aes);
                    if (VerifyFile(keyHMAC, filePath, decryptedString))
                        MessageBox.Show("Os HMACs são iguais tanto no ficheiro Original, como no ficheiro recebido.");
                    else
                        MessageBox.Show("Os HMACs diferem! A Claire está a fazer das suas...");
                    textoBox.Text = decryptedString;
                }
                this.Text = Path.GetFileName(filePath) + " - SecuriText";
            }
        }

        private void novoItem_Click(object sender, EventArgs e)
        {
            textoBox.Text = "";
            filePath = "";
            this.Text = "Sem Titulo - SecuriText";
        }

        private void guardarItem_Click(object sender, EventArgs e)
        {
            if (!textoBox.Text.Equals(""))
            {
                if (filePath == "")
                {
                    SaveOptions svO = new SaveOptions();
                    svO.ShowDialog();
                    String saveMode = svO.getSaveMode();
                    SaveFileDialog sfd = new SaveFileDialog();
                    if (saveMode.Equals("Ambos"))
                    {
                        sfd.Filter = "Encrypted&Authenticated|*.encAuth";
                    }
                    else
                    {
                        if (saveMode.Equals("Autenticar"))
                        {
                            sfd.Filter = "Authenticated File|*.auth";
                        }
                        if (saveMode.Equals("Cifrar"))
                        {
                            sfd.Filter = "Encrypted File|*.enc";
                        }
                    }
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        filePath = sfd.FileName;
                        if (saveMode.Equals("Ambos"))
                        {
                            HMACSHA256 hmac = new HMACSHA256();
                            Aes aes = Aes.Create();
                            byte[] data = EncryptAES(textoBox.Text, aes);
                            File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\HMAC.txt", SignFile(hmac, textoBox.Text, sfd.FileName));
                            File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\keys-and-IV.txt", Convert.ToBase64String(hmac.Key) + "|" + Convert.ToBase64String(aes.Key) + "|" + Convert.ToBase64String(aes.IV));
                            File.WriteAllText(sfd.FileName, Convert.ToBase64String(encRSA.Encrypt(data, false)));
                            File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\SK-PK.pem", Convert.ToBase64String(encRSA.ExportRSAPrivateKey()) + "|" + Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
                        }
                        else
                        {
                            if (saveMode.Equals("Autenticar"))
                            {
                                HMACSHA256 hmac = new HMACSHA256();
                                Aes aes = Aes.Create();
                                File.WriteAllText(sfd.FileName, Convert.ToBase64String(EncryptAES(textoBox.Text, aes)));
                                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\HMAC.txt", SignFile(hmac, textoBox.Text, sfd.FileName));
                                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\keys-and-IV.txt", Convert.ToBase64String(hmac.Key) + "|" + Convert.ToBase64String(aes.Key) + "|" + Convert.ToBase64String(aes.IV));

                            }
                            if (saveMode.Equals("Cifrar"))
                            {
                                byte[] data = Encoding.UTF8.GetBytes(textoBox.Text);
                                File.WriteAllText(sfd.FileName, Convert.ToBase64String(encRSA.Encrypt(data, false)));
                                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\SK-PK.pem", Convert.ToBase64String(encRSA.ExportRSAPrivateKey()) + "|" + Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
                            }
                        }
                        this.Text = Path.GetFileName(filePath) + " - SecuriText";
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
                        aes.Key = Convert.FromBase64String(keysAndIVSplitted[1]);
                        aes.IV = Convert.FromBase64String(keysAndIVSplitted[2]);
                        File.WriteAllText(filePath, Convert.ToBase64String(EncryptAES(textoBox.Text, aes)));
                        File.WriteAllText(Path.GetDirectoryName(filePath) + "\\keys-and-IV.txt", Convert.ToBase64String(hmac.Key) + "|" + Convert.ToBase64String(aes.Key) + "|" + Convert.ToBase64String(aes.IV));
                    }
                    if (Path.GetExtension(filePath) == ".enc")
                    {
                        byte[] data = Encoding.UTF8.GetBytes(textoBox.Text);
                        File.WriteAllText(filePath, Convert.ToBase64String(encRSA.Encrypt(data, false)));
                        File.WriteAllText(Path.GetDirectoryName(filePath) + "\\SK-PK.pem", Convert.ToBase64String(encRSA.ExportRSAPrivateKey()) + "|" + Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
                    }
                    if (Path.GetExtension(filePath) == ".encAuth")
                    {
                        string[] keysAndIVSplitted = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt").Split("|");
                        Aes aes = Aes.Create();
                        aes.Key = Convert.FromBase64String(keysAndIVSplitted[1]);
                        aes.IV = Convert.FromBase64String(keysAndIVSplitted[2]);
                        byte[] encryptedAES = EncryptAES(textoBox.Text, aes);
                        File.WriteAllText(filePath, Convert.ToBase64String(encRSA.Encrypt(encryptedAES, false)));
                        File.WriteAllText(Path.GetDirectoryName(filePath) + "\\SK-PK.pem", Convert.ToBase64String(encRSA.ExportRSAPrivateKey()) + "|" + Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
                    }
                    MessageBox.Show("Ficheiro guardado com sucesso!");
                }
            }
            else
            {
                MessageBox.Show("Nenhum texto foi introduzido!");
            }
        }

        private void verajudaItem_Click_1(object sender, EventArgs e)
        {
            HelpForm hp = new HelpForm();
            hp.ShowDialog();
        }

    }
}
