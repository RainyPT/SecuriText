using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SecuriText
{
    public partial class Substitute : Form
    {
        public Substitute()
        {
            InitializeComponent();
        }
        public string getWord2Substitute()
        {
            return textBoxWord2Sub.Text;
        }
        public string getSubstituteWord()
        {
            return textBoxSubWord.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Substitute_Load(object sender, EventArgs e)
        {

        }
    }
}
