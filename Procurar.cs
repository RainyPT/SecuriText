using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SecuriText
{
    public partial class Procurar : Form
    {
        public Procurar()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
        public string getWord2Find()
        {
            return word2find.Text;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Procurar_Load(object sender, EventArgs e)
        {

        }
    }
}
