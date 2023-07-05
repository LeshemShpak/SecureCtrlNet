namespace fin_pro
{
    partial class ClientControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            ramBar = new ProgressBar();
            cpuBar = new ProgressBar();
            ipLabel = new Label();
            cpuLabel = new Label();
            label2 = new Label();
            ipAddressLabel = new Label();
            manageButton = new Button();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.client;
            pictureBox1.Location = new Point(18, 16);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(63, 44);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // ramBar
            // 
            ramBar.Location = new Point(167, 74);
            ramBar.Name = "ramBar";
            ramBar.Size = new Size(173, 15);
            ramBar.TabIndex = 1;
            // 
            // cpuBar
            // 
            cpuBar.Location = new Point(167, 45);
            cpuBar.Name = "cpuBar";
            cpuBar.Size = new Size(173, 15);
            cpuBar.TabIndex = 2;
            // 
            // ipLabel
            // 
            ipLabel.AutoSize = true;
            ipLabel.Location = new Point(117, 16);
            ipLabel.Name = "ipLabel";
            ipLabel.Size = new Size(20, 15);
            ipLabel.TabIndex = 3;
            ipLabel.Text = "IP:";
            // 
            // cpuLabel
            // 
            cpuLabel.AutoSize = true;
            cpuLabel.Location = new Point(117, 45);
            cpuLabel.Name = "cpuLabel";
            cpuLabel.Size = new Size(33, 15);
            cpuLabel.TabIndex = 4;
            cpuLabel.Text = "CPU:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(117, 74);
            label2.Name = "label2";
            label2.Size = new Size(36, 15);
            label2.TabIndex = 5;
            label2.Text = "RAM:";
            // 
            // ipAddressLabel
            // 
            ipAddressLabel.AutoSize = true;
            ipAddressLabel.Location = new Point(167, 16);
            ipAddressLabel.Name = "ipAddressLabel";
            ipAddressLabel.Size = new Size(38, 15);
            ipAddressLabel.TabIndex = 6;
            ipAddressLabel.Text = "label1";
            // 
            // manageButton
            // 
            manageButton.Location = new Point(3, 66);
            manageButton.Name = "manageButton";
            manageButton.Size = new Size(93, 23);
            manageButton.TabIndex = 7;
            manageButton.Text = "Manage Client";
            manageButton.UseVisualStyleBackColor = true;
            manageButton.Click += manageButton_Click;
            // 
            // ClientControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(manageButton);
            Controls.Add(ipAddressLabel);
            Controls.Add(label2);
            Controls.Add(cpuLabel);
            Controls.Add(ipLabel);
            Controls.Add(cpuBar);
            Controls.Add(ramBar);
            Controls.Add(pictureBox1);
            Name = "ClientControl";
            Size = new Size(347, 98);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private ProgressBar ramBar;
        private ProgressBar cpuBar;
        private Label ipLabel;
        private Label cpuLabel;
        private Label label2;
        private Label ipAddressLabel;
        private Button manageButton;
        private ToolTip toolTip1;
    }
}
