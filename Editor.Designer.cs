
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
            this.textoBox = new System.Windows.Forms.RichTextBox();
            this.ficheiroMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.novoItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guardarItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editarMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.procurarItem = new System.Windows.Forms.ToolStripMenuItem();
            this.substituirItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ajudaItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verajudaItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.assinarItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // textoBox
            // 
            this.textoBox.Location = new System.Drawing.Point(0, 27);
            this.textoBox.Name = "textoBox";
            this.textoBox.Size = new System.Drawing.Size(782, 518);
            this.textoBox.TabIndex = 1;
            this.textoBox.Text = "";
            // 
            // ficheiroMenu
            // 
            this.ficheiroMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.novoItem,
            this.abrirItem,
            this.guardarItem,
            this.assinarItem});
            this.ficheiroMenu.Name = "ficheiroMenu";
            this.ficheiroMenu.Size = new System.Drawing.Size(61, 24);
            this.ficheiroMenu.Text = "Ficheiro";
            // 
            // novoItem
            // 
            this.novoItem.Name = "novoItem";
            this.novoItem.Size = new System.Drawing.Size(116, 22);
            this.novoItem.Text = "Novo";
            this.novoItem.Click += new System.EventHandler(this.novoItem_Click);
            // 
            // abrirItem
            // 
            this.abrirItem.Name = "abrirItem";
            this.abrirItem.Size = new System.Drawing.Size(116, 22);
            this.abrirItem.Text = "Abrir";
            this.abrirItem.Click += new System.EventHandler(this.abrirItem_Click);
            // 
            // guardarItem
            // 
            this.guardarItem.Name = "guardarItem";
            this.guardarItem.Size = new System.Drawing.Size(116, 22);
            this.guardarItem.Text = "Guardar";
            this.guardarItem.Click += new System.EventHandler(this.guardarItem_Click);
            // 
            // editarMenu
            // 
            this.editarMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.procurarItem,
            this.substituirItem});
            this.editarMenu.Name = "editarMenu";
            this.editarMenu.Size = new System.Drawing.Size(49, 24);
            this.editarMenu.Text = "Editar";
            // 
            // procurarItem
            // 
            this.procurarItem.Name = "procurarItem";
            this.procurarItem.Size = new System.Drawing.Size(124, 22);
            this.procurarItem.Text = "Procurar";
            this.procurarItem.Click += new System.EventHandler(this.procurarItem_Click);
            // 
            // substituirItem
            // 
            this.substituirItem.Name = "substituirItem";
            this.substituirItem.Size = new System.Drawing.Size(124, 22);
            this.substituirItem.Text = "Substituir";
            this.substituirItem.Click += new System.EventHandler(this.substituirItem_Click);
            // 
            // ajudaItem
            // 
            this.ajudaItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verajudaItem,
            this.acercaItem});
            this.ajudaItem.Name = "ajudaItem";
            this.ajudaItem.Size = new System.Drawing.Size(50, 24);
            this.ajudaItem.Text = "Ajuda";
            // 
            // verajudaItem
            // 
            this.verajudaItem.Name = "verajudaItem";
            this.verajudaItem.Size = new System.Drawing.Size(124, 22);
            this.verajudaItem.Text = "Ver Ajuda";
            this.verajudaItem.Click += new System.EventHandler(this.verajudaItem_Click_1);
            // 
            // acercaItem
            // 
            this.acercaItem.Name = "acercaItem";
            this.acercaItem.Size = new System.Drawing.Size(124, 22);
            this.acercaItem.Text = "Acerca";
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ficheiroMenu,
            this.editarMenu,
            this.ajudaItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Padding = new System.Windows.Forms.Padding(0);
            this.menu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menu.Size = new System.Drawing.Size(782, 24);
            this.menu.TabIndex = 0;
            this.menu.Text = "Menu";
            this.menu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menu_ItemClicked);
            // 
            // assinarItem
            // 
            this.assinarItem.Name = "assinarItem";
            this.assinarItem.Size = new System.Drawing.Size(116, 22);
            this.assinarItem.Text = "Assinar";
            this.assinarItem.Click += new System.EventHandler(this.assinarItem_Click);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 544);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.textoBox);
            this.Name = "Editor";
            this.Text = "SecuriText";
            this.Load += new System.EventHandler(this.Editor_Load);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox textoBox;
        private System.Windows.Forms.ToolStripMenuItem ficheiroMenu;
        private System.Windows.Forms.ToolStripMenuItem novoItem;
        private System.Windows.Forms.ToolStripMenuItem abrirItem;
        private System.Windows.Forms.ToolStripMenuItem guardarItem;
        private System.Windows.Forms.ToolStripMenuItem editarMenu;
        private System.Windows.Forms.ToolStripMenuItem procurarItem;
        private System.Windows.Forms.ToolStripMenuItem substituirItem;
        private System.Windows.Forms.ToolStripMenuItem ajudaItem;
        private System.Windows.Forms.ToolStripMenuItem verajudaItem;
        private System.Windows.Forms.ToolStripMenuItem acercaItem;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem assinarItem;
    }
}