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
using Microsoft.VisualBasic;

namespace SecuriText
{
    public partial class Editor : Form
    {
        Aes encAes;
        string filePath="";
        public Editor()
        {
            InitializeComponent();
            encAes = Aes.Create();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
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

        //Recebe um objecto do tipo HMACSHA256, que gere as chaves de integridade.
        //recebe tambem o texto introduzido pelo utilizador e o directorio destino.
        public static string AuthenticateFile(HMACSHA256 hMac, String text, String destFile)
        {
            //Converte o texto introduzido pelo utilizador num array de bytes, e depois computa a sua hash
            byte[] hashValue = hMac.ComputeHash(Encoding.UTF8.GetBytes(text));
            //retorna o valor do hash em forma de uma string base 64
            return Convert.ToBase64String(hashValue);
        }
        //Recebe a chave de integridade, o directorio do ficheiro origem, e o texto cifrado
        public static bool VerifyAuth(byte[] key, String filePath, string cypherText)
        {
            //Declara um novo objecto do tipo HMACSHA256, com o valor da chave recebida.
            HMACSHA256 hmac = new HMACSHA256(key);
            //Computa o hmac do texto cifrado
            string computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(cypherText)));
            //Recebe o hmac do ficheiro original guardado no computador do utilizador
            string storedHash = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\HMAC.txt");
            //compara os dois hmac (do ficheiro recebido e do ficheiro original)
            if (computedHash == storedHash)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
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
        private void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void abrirItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Encrypted/Authed/Signed Files|*.enc;*.auth;*.encAuth;*.sign";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filePath = dialog.FileName;
                if (Path.GetExtension(filePath) == ".auth")
                {
                    byte[] keyHMAC = Convert.FromBase64String(File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt"));
                    string authedText = File.ReadAllText(filePath);
                    if (VerifyAuth(keyHMAC, filePath, authedText))
                        MessageBox.Show("Os HMACs são iguais tanto no ficheiro Original, como no ficheiro recebido.");
                    else
                        MessageBox.Show("Os HMACs diferem! A Claire está a fazer das suas...");

                    textoBox.Text = authedText;
                }
                if (Path.GetExtension(filePath) == ".enc")
                {
                    string[] keysAndIVSplitted = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt").Split("|");
                    encAes.Key = Convert.FromBase64String(keysAndIVSplitted[0]);
                    encAes.IV = Convert.FromBase64String(keysAndIVSplitted[1]);
                    String decriptedText = DecryptAES(Convert.FromBase64String(File.ReadAllText(filePath)), encAes);
                    textoBox.Text = decriptedText;
                }
                if (Path.GetExtension(filePath) == ".encAuth")
                {
                    string[] keysAndIVSplitted = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt").Split("|");
                    encAes.Key = Convert.FromBase64String(keysAndIVSplitted[1]);
                    encAes.IV = Convert.FromBase64String(keysAndIVSplitted[2]);
                    byte[] keyHMAC = Convert.FromBase64String(keysAndIVSplitted[0]);
                    string decriptedText = DecryptAES(Convert.FromBase64String(File.ReadAllText(filePath)), encAes);
                    if (VerifyAuth(keyHMAC, filePath, decriptedText))
                        MessageBox.Show("Os HMACs são iguais tanto no ficheiro Original, como no ficheiro recebido.");
                    else
                        MessageBox.Show("Os HMACs diferem! A Claire está a fazer das suas...");
                    textoBox.Text = decriptedText;
                }
                if (Path.GetExtension(filePath) == ".sign")
                {
                    RSACryptoServiceProvider encRSA = new RSACryptoServiceProvider();
                    String PK = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\PK.pem");
                    string textolimpo = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\texto-limpo.txt");
                    encRSA.ImportRSAPublicKey(Convert.FromBase64String(PK), out _);
                    if (VerifySignedRSA(textolimpo, File.ReadAllText(filePath), encRSA))
                    {
                        MessageBox.Show("O texto foi verificado com sucesso!");
                    }
                    else
                    {
                        MessageBox.Show("O texto não condiz com a assinatura!");
                    }
                    textoBox.Text = textolimpo;
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
                    MessageBoxButtons alertaButtons = MessageBoxButtons.OK;
                    MessageBox.Show("É recomendado que elimine o ficheiro keys-and-IV.txt depois de efectuar esta operação!", "AVISO", alertaButtons, MessageBoxIcon.Warning);
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        filePath = sfd.FileName;
                        if (saveMode.Equals("Ambos"))
                        {
                            HMACSHA256 hmac = new HMACSHA256();
                            encAes = Aes.Create();
                            File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\HMAC.txt", AuthenticateFile(hmac, textoBox.Text, sfd.FileName));
                            File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\keys-and-IV.txt", Convert.ToBase64String(hmac.Key) + "|" + Convert.ToBase64String(encAes.Key) + "|" + Convert.ToBase64String(encAes.IV));
                            File.WriteAllText(sfd.FileName, Convert.ToBase64String(EncryptAES(textoBox.Text, encAes)));
                        }
                        else
                        {
                            if (saveMode.Equals("Autenticar"))
                            {
                                HMACSHA256 hmac = new HMACSHA256();
                                File.WriteAllText(sfd.FileName, textoBox.Text);
                                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\HMAC.txt", AuthenticateFile(hmac, textoBox.Text, sfd.FileName));
                                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\keys-and-IV.txt", Convert.ToBase64String(hmac.Key));

                            }
                            if (saveMode.Equals("Cifrar"))
                            {
                                File.WriteAllText(sfd.FileName, Convert.ToBase64String(EncryptAES(textoBox.Text, encAes)));
                                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\keys-and-IV.txt", Convert.ToBase64String(encAes.Key) + "|" + Convert.ToBase64String(encAes.IV));
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
                        HMACSHA256 hmac = new HMACSHA256();
                        File.WriteAllText(filePath, textoBox.Text);
                        File.WriteAllText(Path.GetDirectoryName(filePath) + "\\keys-and-IV.txt", Convert.ToBase64String(hmac.Key));
                    }
                    if (Path.GetExtension(filePath) == ".enc")
                    {
                        File.WriteAllText(filePath, Convert.ToBase64String(EncryptAES(textoBox.Text, encAes)));
                    }
                    if (Path.GetExtension(filePath) == ".encAuth")
                    {
                        string[] keysAndIVSplitted = File.ReadAllText(Path.GetDirectoryName(filePath) + "\\keys-and-iv.txt").Split("|");
                        Aes aes = Aes.Create();
                        aes.Key = Convert.FromBase64String(keysAndIVSplitted[1]);
                        aes.IV = Convert.FromBase64String(keysAndIVSplitted[2]);
                        byte[] encryptedAES = EncryptAES(textoBox.Text, aes);
                        File.WriteAllText(filePath, Convert.ToBase64String(encryptedAES));
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

        private void substituirItem_Click(object sender, EventArgs e)
        {
            Substitute subForm = new Substitute();
            subForm.ShowDialog();
            if (subForm.getWord2Substitute() != "")
            {
                if (textoBox.Text.Contains(subForm.getWord2Substitute()))
                    textoBox.Text = textoBox.Text.Replace(subForm.getWord2Substitute(), subForm.getSubstituteWord());
                else
                    MessageBox.Show("Não foi encontrada a palavra no texto!");
            }
        }

        private void Editor_Load(object sender, EventArgs e)
        {

        }

        private void procurarItem_Click(object sender, EventArgs e)
        {
            Procurar procurarForm = new Procurar();
            procurarForm.ShowDialog();
            if (procurarForm.getWord2Find()!="")
            {
                if (textoBox.Find(procurarForm.getWord2Find()) >= 0)
                {
                    MessageBox.Show("A palavra " + procurarForm.getWord2Find() + " foi encontrada no texto");
                }
                else
                {
                    MessageBox.Show("A palavra " + procurarForm.getWord2Find() + " não foi encontrada no texto");
                }
            }
        }

        private void assinarItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            RSACryptoServiceProvider encRSA = new RSACryptoServiceProvider();
            sfd.Filter = "SignedRSA|*.sign";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\texto-limpo.txt", textoBox.Text);
                File.WriteAllText(sfd.FileName, RSASign(textoBox.Text, encRSA));
                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\PK.pem", Convert.ToBase64String(encRSA.ExportRSAPublicKey()));
            }
        }


        private void acercaItem_Click_1(object sender, EventArgs e)
        {
            Acerca acerca = new Acerca();
            acerca.ShowDialog();
        }

        private void textoBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
