using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SecuriText
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "enc";
            dialog.Filter = "Encrypted File|*.enc";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Editor editor = new Editor(dialog.FileName);
                editor.Show();
            }
        }

        private void newFileButton_Click(object sender, EventArgs e)
        {
            Editor editor = new Editor("");
            editor.Show();
        }
    }
}
