using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SecuriText
{
    public partial class Editor : Form
    {
        private String filePath;
        public Editor(String path)
        {
            InitializeComponent();
            filePath = path;
        }

        private void Editor_Load(object sender, EventArgs e)
        {
            if (filePath != "")
            {
                textHandle.Text = File.ReadAllText(filePath);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (filePath == "")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "enc";
                sfd.Filter = "Encrypted File|*.enc";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, textHandle.Text);
                    MessageBox.Show("Ficheiro guardado com sucesso!");
                }
                else
                {
                    MessageBox.Show("Problema ao guardar o ficheiro!");
                }
            }
            else
            {
                File.WriteAllText(filePath, textHandle.Text);
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
