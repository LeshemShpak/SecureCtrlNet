namespace fin_pro
{
    partial class Form2
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
            this.update_info_worker = new System.ComponentModel.BackgroundWorker();
            this.listen_to_command = new System.ComponentModel.BackgroundWorker();
            this.clientsListLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.loadingBar = new System.Windows.Forms.ProgressBar();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // update_info_worker
            // 
            this.update_info_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.update_info_worker_DoWork);
            // 
            // listen_to_command
            // 
            this.listen_to_command.DoWork += new System.ComponentModel.DoWorkEventHandler(this.listen_to_command_DoWork);
            // 
            // clientsListLayout
            // 
            this.clientsListLayout.AutoScroll = true;
            this.clientsListLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientsListLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.clientsListLayout.Location = new System.Drawing.Point(0, 0);
            this.clientsListLayout.Name = "clientsListLayout";
            this.clientsListLayout.Size = new System.Drawing.Size(430, 500);
            this.clientsListLayout.TabIndex = 0;
            this.clientsListLayout.Visible = false;
            this.clientsListLayout.WrapContents = false;
            // 
            // loadingBar
            // 
            this.loadingBar.Location = new System.Drawing.Point(92, 245);
            this.loadingBar.Name = "loadingBar";
            this.loadingBar.Size = new System.Drawing.Size(238, 23);
            this.loadingBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.loadingBar.TabIndex = 1;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoSize = true;
            this.loadingLabel.Location = new System.Drawing.Point(158, 218);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(114, 15);
            this.loadingLabel.TabIndex = 2;
            this.loadingLabel.Text = "Loading clients list...";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 500);
            this.Controls.Add(this.loadingLabel);
            this.Controls.Add(this.loadingBar);
            this.Controls.Add(this.clientsListLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RPC Clients";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker update_info_worker;
        private System.ComponentModel.BackgroundWorker listen_to_command;
        private FlowLayoutPanel clientsListLayout;
        private ProgressBar loadingBar;
        private Label loadingLabel;
    }
}