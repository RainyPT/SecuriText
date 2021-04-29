
namespace SecuriText
{
    partial class Editor
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
            this.textHandle = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.cypherCheckBox = new System.Windows.Forms.CheckBox();
            this.authCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textHandle
            // 
            this.textHandle.Location = new System.Drawing.Point(12, 64);
            this.textHandle.Multiline = true;
            this.textHandle.Name = "textHandle";
            this.textHandle.Size = new System.Drawing.Size(776, 374);
            this.textHandle.TabIndex = 0;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(330, 12);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(110, 46);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Guardar";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cypherCheckBox
            // 
            this.cypherCheckBox.AutoSize = true;
            this.cypherCheckBox.Location = new System.Drawing.Point(446, 39);
            this.cypherCheckBox.Name = "cypherCheckBox";
            this.cypherCheckBox.Size = new System.Drawing.Size(55, 19);
            this.cypherCheckBox.TabIndex = 2;
            this.cypherCheckBox.Text = "Cifrar";
            this.cypherCheckBox.UseVisualStyleBackColor = true;
            // 
            // authCheckBox
            // 
            this.authCheckBox.AutoSize = true;
            this.authCheckBox.Location = new System.Drawing.Point(446, 12);
            this.authCheckBox.Name = "authCheckBox";
            this.authCheckBox.Size = new System.Drawing.Size(81, 19);
            this.authCheckBox.TabIndex = 3;
            this.authCheckBox.Text = "Autenticar";
            this.authCheckBox.UseVisualStyleBackColor = true;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.authCheckBox);
            this.Controls.Add(this.cypherCheckBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.textHandle);
            this.Name = "Editor";
            this.Text = "Editor";
            this.Load += new System.EventHandler(this.Editor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textHandle;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.CheckBox cypherCheckBox;
        private System.Windows.Forms.CheckBox authCheckBox;
    }
}