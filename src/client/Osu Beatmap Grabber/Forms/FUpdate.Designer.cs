namespace Osu_Beatmap_Grabber
{
    partial class FUpdate
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FUpdate));
            this.PBUpdate = new System.Windows.Forms.ProgressBar();
            this.LBUpdate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PBUpdate
            // 
            this.PBUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PBUpdate.Location = new System.Drawing.Point(0, 0);
            this.PBUpdate.Name = "PBUpdate";
            this.PBUpdate.Size = new System.Drawing.Size(256, 32);
            this.PBUpdate.TabIndex = 0;
            // 
            // LBUpdate
            // 
            this.LBUpdate.BackColor = System.Drawing.SystemColors.Control;
            this.LBUpdate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LBUpdate.Location = new System.Drawing.Point(0, 16);
            this.LBUpdate.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.LBUpdate.Name = "LBUpdate";
            this.LBUpdate.Padding = new System.Windows.Forms.Padding(1);
            this.LBUpdate.Size = new System.Drawing.Size(256, 16);
            this.LBUpdate.TabIndex = 1;
            this.LBUpdate.Text = "label1";
            this.LBUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(256, 32);
            this.Controls.Add(this.LBUpdate);
            this.Controls.Add(this.PBUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FUpdate";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar PBUpdate;
        private System.Windows.Forms.Label LBUpdate;
    }
}

