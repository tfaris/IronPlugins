namespace ErrorLogging
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
            this.textBoxInfo = new System.Windows.Forms.TextBox();
            this.buttonTrace = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxInfo
            // 
            this.textBoxInfo.Location = new System.Drawing.Point(12, 12);
            this.textBoxInfo.Name = "textBoxInfo";
            this.textBoxInfo.Size = new System.Drawing.Size(260, 20);
            this.textBoxInfo.TabIndex = 0;
            // 
            // buttonTrace
            // 
            this.buttonTrace.Location = new System.Drawing.Point(105, 38);
            this.buttonTrace.Name = "buttonTrace";
            this.buttonTrace.Size = new System.Drawing.Size(75, 23);
            this.buttonTrace.TabIndex = 1;
            this.buttonTrace.Text = "Trace Info";
            this.buttonTrace.UseVisualStyleBackColor = true;
            this.buttonTrace.Click += new System.EventHandler(this.buttonTrace_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 120);
            this.Controls.Add(this.buttonTrace);
            this.Controls.Add(this.textBoxInfo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxInfo;
        private System.Windows.Forms.Button buttonTrace;
    }
}