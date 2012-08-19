namespace PluggableApp
{
    partial class Form1
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
            this.buttonAddPlug = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colPlugin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPluginGuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRun = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colRemPlugin = new System.Windows.Forms.DataGridViewButtonColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAddPlug
            // 
            this.buttonAddPlug.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonAddPlug.Location = new System.Drawing.Point(165, 12);
            this.buttonAddPlug.Name = "buttonAddPlug";
            this.buttonAddPlug.Size = new System.Drawing.Size(119, 23);
            this.buttonAddPlug.TabIndex = 0;
            this.buttonAddPlug.Text = "Add Plugin From File";
            this.buttonAddPlug.UseVisualStyleBackColor = true;
            this.buttonAddPlug.Click += new System.EventHandler(this.buttonAddPlug_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPlugin,
            this.colPluginGuid,
            this.colRun,
            this.colRemPlugin});
            this.dataGridView1.Location = new System.Drawing.Point(12, 41);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(443, 168);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // colPlugin
            // 
            this.colPlugin.HeaderText = "Plugin";
            this.colPlugin.Name = "colPlugin";
            this.colPlugin.ReadOnly = true;
            // 
            // colPluginGuid
            // 
            this.colPluginGuid.HeaderText = "Guid";
            this.colPluginGuid.Name = "colPluginGuid";
            this.colPluginGuid.ReadOnly = true;
            // 
            // colRun
            // 
            this.colRun.HeaderText = "Run";
            this.colRun.Name = "colRun";
            this.colRun.ReadOnly = true;
            // 
            // colRemPlugin
            // 
            this.colRemPlugin.HeaderText = "Remove";
            this.colRemPlugin.Name = "colRemPlugin";
            this.colRemPlugin.ReadOnly = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 215);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(443, 122);
            this.textBox1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 349);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonAddPlug);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAddPlug;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPlugin;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPluginGuid;
        private System.Windows.Forms.DataGridViewButtonColumn colRun;
        private System.Windows.Forms.DataGridViewButtonColumn colRemPlugin;
        private System.Windows.Forms.TextBox textBox1;
    }
}

