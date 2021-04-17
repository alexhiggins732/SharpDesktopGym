
namespace MouseSimulatorGui
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.cbMiddlePressed = new System.Windows.Forms.CheckBox();
            this.cbRightPressed = new System.Windows.Forms.CheckBox();
            this.cbLeftPressed = new System.Windows.Forms.CheckBox();
            this.grpMouseControls = new System.Windows.Forms.GroupBox();
            this.btnRightClick = new System.Windows.Forms.Button();
            this.btnMiddleClick = new System.Windows.Forms.Button();
            this.btnLeftClick = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnTouch = new System.Windows.Forms.Button();
            this.cbTouchPressed = new System.Windows.Forms.CheckBox();
            this.pbScreen = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.grpMouseControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbTouchPressed);
            this.groupBox1.Controls.Add(this.lblY);
            this.groupBox1.Controls.Add(this.lblX);
            this.groupBox1.Controls.Add(this.cbMiddlePressed);
            this.groupBox1.Controls.Add(this.cbRightPressed);
            this.groupBox1.Controls.Add(this.cbLeftPressed);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 155);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mousepad State";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(115, 135);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(26, 13);
            this.lblY.TabIndex = 8;
            this.lblY.Text = "Y: 0";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(6, 135);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(26, 13);
            this.lblX.TabIndex = 7;
            this.lblX.Text = "X: 0";
            // 
            // cbMiddlePressed
            // 
            this.cbMiddlePressed.AutoSize = true;
            this.cbMiddlePressed.Location = new System.Drawing.Point(9, 65);
            this.cbMiddlePressed.Name = "cbMiddlePressed";
            this.cbMiddlePressed.Size = new System.Drawing.Size(98, 17);
            this.cbMiddlePressed.TabIndex = 6;
            this.cbMiddlePressed.Text = "Middle Pressed";
            this.cbMiddlePressed.UseVisualStyleBackColor = true;
            // 
            // cbRightPressed
            // 
            this.cbRightPressed.AutoSize = true;
            this.cbRightPressed.Location = new System.Drawing.Point(9, 42);
            this.cbRightPressed.Name = "cbRightPressed";
            this.cbRightPressed.Size = new System.Drawing.Size(92, 17);
            this.cbRightPressed.TabIndex = 5;
            this.cbRightPressed.Text = "Right Pressed";
            this.cbRightPressed.UseVisualStyleBackColor = true;
            // 
            // cbLeftPressed
            // 
            this.cbLeftPressed.AutoSize = true;
            this.cbLeftPressed.Location = new System.Drawing.Point(9, 19);
            this.cbLeftPressed.Name = "cbLeftPressed";
            this.cbLeftPressed.Size = new System.Drawing.Size(85, 17);
            this.cbLeftPressed.TabIndex = 4;
            this.cbLeftPressed.Text = "Left Pressed";
            this.cbLeftPressed.UseVisualStyleBackColor = true;
            // 
            // grpMouseControls
            // 
            this.grpMouseControls.Controls.Add(this.btnTouch);
            this.grpMouseControls.Controls.Add(this.btnRightClick);
            this.grpMouseControls.Controls.Add(this.btnMiddleClick);
            this.grpMouseControls.Controls.Add(this.btnLeftClick);
            this.grpMouseControls.Controls.Add(this.btnLeft);
            this.grpMouseControls.Controls.Add(this.btnRight);
            this.grpMouseControls.Controls.Add(this.btnDown);
            this.grpMouseControls.Controls.Add(this.btnUp);
            this.grpMouseControls.Location = new System.Drawing.Point(282, 12);
            this.grpMouseControls.Name = "grpMouseControls";
            this.grpMouseControls.Size = new System.Drawing.Size(174, 155);
            this.grpMouseControls.TabIndex = 1;
            this.grpMouseControls.TabStop = false;
            this.grpMouseControls.Text = "Mousepad Controls";
            // 
            // btnRightClick
            // 
            this.btnRightClick.Location = new System.Drawing.Point(116, 125);
            this.btnRightClick.Name = "btnRightClick";
            this.btnRightClick.Size = new System.Drawing.Size(48, 23);
            this.btnRightClick.TabIndex = 7;
            this.btnRightClick.Text = "R";
            this.btnRightClick.UseVisualStyleBackColor = true;
            // 
            // btnMiddleClick
            // 
            this.btnMiddleClick.Location = new System.Drawing.Point(66, 125);
            this.btnMiddleClick.Name = "btnMiddleClick";
            this.btnMiddleClick.Size = new System.Drawing.Size(44, 23);
            this.btnMiddleClick.TabIndex = 6;
            this.btnMiddleClick.Text = "M";
            this.btnMiddleClick.UseVisualStyleBackColor = true;
            // 
            // btnLeftClick
            // 
            this.btnLeftClick.Location = new System.Drawing.Point(8, 125);
            this.btnLeftClick.Name = "btnLeftClick";
            this.btnLeftClick.Size = new System.Drawing.Size(52, 23);
            this.btnLeftClick.TabIndex = 4;
            this.btnLeftClick.Text = "L";
            this.btnLeftClick.UseVisualStyleBackColor = true;
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(8, 67);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 23);
            this.btnLeft.TabIndex = 3;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(89, 67);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 2;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(55, 96);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 1;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(55, 38);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 23);
            this.btnUp.TabIndex = 0;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // btnTouch
            // 
            this.btnTouch.Location = new System.Drawing.Point(55, 13);
            this.btnTouch.Name = "btnTouch";
            this.btnTouch.Size = new System.Drawing.Size(75, 23);
            this.btnTouch.TabIndex = 8;
            this.btnTouch.Text = "Touch";
            this.btnTouch.UseVisualStyleBackColor = true;
            // 
            // cbTouchPressed
            // 
            this.cbTouchPressed.AutoSize = true;
            this.cbTouchPressed.Location = new System.Drawing.Point(9, 88);
            this.cbTouchPressed.Name = "cbTouchPressed";
            this.cbTouchPressed.Size = new System.Drawing.Size(98, 17);
            this.cbTouchPressed.TabIndex = 9;
            this.cbTouchPressed.Text = "Touch Pressed";
            this.cbTouchPressed.UseVisualStyleBackColor = true;
            // 
            // pbScreen
            // 
            this.pbScreen.Location = new System.Drawing.Point(12, 187);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(444, 251);
            this.pbScreen.TabIndex = 2;
            this.pbScreen.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pbScreen);
            this.Controls.Add(this.grpMouseControls);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpMouseControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpMouseControls;
        private System.Windows.Forms.Button btnRightClick;
        private System.Windows.Forms.Button btnMiddleClick;
        private System.Windows.Forms.Button btnLeftClick;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.CheckBox cbMiddlePressed;
        private System.Windows.Forms.CheckBox cbRightPressed;
        private System.Windows.Forms.CheckBox cbLeftPressed;
        private System.Windows.Forms.Button btnTouch;
        private System.Windows.Forms.CheckBox cbTouchPressed;
        private System.Windows.Forms.PictureBox pbScreen;
    }
}

