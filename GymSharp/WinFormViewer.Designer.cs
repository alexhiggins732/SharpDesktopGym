
namespace GymSharp
{
    partial class WinFormViewer
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
            this.frame = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.frame)).BeginInit();
            this.SuspendLayout();
            // 
            // frame
            // 
            this.frame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frame.Location = new System.Drawing.Point(0, 0);
            this.frame.Name = "frame";
            this.frame.Size = new System.Drawing.Size(647, 481);
            this.frame.TabIndex = 0;
            this.frame.TabStop = false;
            // 
            // WinFormViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 481);
            this.Controls.Add(this.frame);
            this.Name = "WinFormViewer";
            this.Text = "WinFormViewer";
            ((System.ComponentModel.ISupportInitialize)(this.frame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox frame;
    }
}