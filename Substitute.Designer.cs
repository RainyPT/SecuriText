
namespace SecuriText
{
    partial class Substitute
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Substitute));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSubWord = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxWord2Sub = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Palavra a substituir:";
            // 
            // textBoxSubWord
            // 
            this.textBoxSubWord.Location = new System.Drawing.Point(143, 57);
            this.textBoxSubWord.Name = "textBoxSubWord";
            this.textBoxSubWord.Size = new System.Drawing.Size(160, 23);
            this.textBoxSubWord.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Palavra Substituição:";
            // 
            // textBoxWord2Sub
            // 
            this.textBoxWord2Sub.Location = new System.Drawing.Point(143, 28);
            this.textBoxWord2Sub.MaximumSize = new System.Drawing.Size(160, 23);
            this.textBoxWord2Sub.MinimumSize = new System.Drawing.Size(160, 23);
            this.textBoxWord2Sub.Name = "textBoxWord2Sub";
            this.textBoxWord2Sub.Size = new System.Drawing.Size(160, 23);
            this.textBoxWord2Sub.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(229, 103);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Substituir";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Substitute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 138);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxWord2Sub);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSubWord);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(325, 177);
            this.MinimumSize = new System.Drawing.Size(325, 177);
            this.Name = "Substitute";
            this.Text = "Substitute";
            this.Load += new System.EventHandler(this.Substitute_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSubWord;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxWord2Sub;
        private System.Windows.Forms.Button button1;
    }
}