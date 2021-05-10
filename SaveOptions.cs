using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SecuriText
{
    public partial class SaveOptions : Form
    {

        string saveMode = "";
        public SaveOptions()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveMode = "Autenticar";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveMode = "Cifrar";
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveMode = "Ambos";
            this.Close();
        }
        public string getSaveMode()
        {
            return saveMode;
        }
    }
}
