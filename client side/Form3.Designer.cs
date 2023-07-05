namespace fin_pro
{
    partial class Form3
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
            this.text_command = new System.Windows.Forms.TextBox();
            this.button_send_command = new System.Windows.Forms.Button();
            this.textboxCmdResults = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Upload_file = new System.Windows.Forms.Button();
            this.Delete_file = new System.Windows.Forms.Button();
            this.Get_file = new System.Windows.Forms.Button();
            this.client_files = new System.Windows.Forms.ListBox();
            this.my_files = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.ProcessName = new System.Windows.Forms.ColumnHeader();
            this.ProcessID = new System.Windows.Forms.ColumnHeader();
            this.search_bar = new System.Windows.Forms.TextBox();
            this.kill_procces_button = new System.Windows.Forms.Button();
            this.procces_count = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.out_put_listener = new System.ComponentModel.BackgroundWorker();
            this.screenshotWorker = new System.ComponentModel.BackgroundWorker();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // text_command
            // 
            this.text_command.Location = new System.Drawing.Point(11, 33);
            this.text_command.Name = "text_command";
            this.text_command.PlaceholderText = "Command...";
            this.text_command.Size = new System.Drawing.Size(601, 31);
            this.text_command.TabIndex = 0;
            // 
            // button_send_command
            // 
            this.button_send_command.Location = new System.Drawing.Point(620, 33);
            this.button_send_command.Name = "button_send_command";
            this.button_send_command.Size = new System.Drawing.Size(189, 38);
            this.button_send_command.TabIndex = 1;
            this.button_send_command.Text = "Send command";
            this.button_send_command.UseVisualStyleBackColor = true;
            this.button_send_command.Click += new System.EventHandler(this.send_command_click);
            // 
            // textboxCmdResults
            // 
            this.textboxCmdResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxCmdResults.Location = new System.Drawing.Point(11, 85);
            this.textboxCmdResults.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textboxCmdResults.Name = "textboxCmdResults";
            this.textboxCmdResults.ReadOnly = true;
            this.textboxCmdResults.Size = new System.Drawing.Size(795, 672);
            this.textboxCmdResults.TabIndex = 2;
            this.textboxCmdResults.Text = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(846, 820);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textboxCmdResults);
            this.tabPage1.Controls.Add(this.text_command);
            this.tabPage1.Controls.Add(this.button_send_command);
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(838, 782);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Commands";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.Upload_file);
            this.tabPage2.Controls.Add(this.Delete_file);
            this.tabPage2.Controls.Add(this.Get_file);
            this.tabPage2.Controls.Add(this.client_files);
            this.tabPage2.Controls.Add(this.my_files);
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(838, 782);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Files";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(510, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 25);
            this.label2.TabIndex = 6;
            this.label2.Text = "Client files";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 25);
            this.label1.TabIndex = 5;
            this.label1.Text = "Local files";
            // 
            // Upload_file
            // 
            this.Upload_file.Location = new System.Drawing.Point(350, 372);
            this.Upload_file.Name = "Upload_file";
            this.Upload_file.Size = new System.Drawing.Size(123, 42);
            this.Upload_file.TabIndex = 4;
            this.Upload_file.Text = "----->";
            this.Upload_file.UseVisualStyleBackColor = true;
            this.Upload_file.Click += new System.EventHandler(this.Upload_file_Click);
            // 
            // Delete_file
            // 
            this.Delete_file.Location = new System.Drawing.Point(707, 578);
            this.Delete_file.Name = "Delete_file";
            this.Delete_file.Size = new System.Drawing.Size(117, 42);
            this.Delete_file.TabIndex = 3;
            this.Delete_file.Text = "Delete";
            this.Delete_file.UseVisualStyleBackColor = true;
            this.Delete_file.Click += new System.EventHandler(this.Delete_file_Click);
            // 
            // Get_file
            // 
            this.Get_file.Location = new System.Drawing.Point(350, 308);
            this.Get_file.Name = "Get_file";
            this.Get_file.Size = new System.Drawing.Size(123, 42);
            this.Get_file.TabIndex = 2;
            this.Get_file.Text = "<-----";
            this.Get_file.UseVisualStyleBackColor = true;
            this.Get_file.Click += new System.EventHandler(this.Get_file_Click);
            // 
            // client_files
            // 
            this.client_files.FormattingEnabled = true;
            this.client_files.ItemHeight = 25;
            this.client_files.Location = new System.Drawing.Point(510, 65);
            this.client_files.Name = "client_files";
            this.client_files.Size = new System.Drawing.Size(313, 504);
            this.client_files.TabIndex = 1;
            // 
            // my_files
            // 
            this.my_files.FormattingEnabled = true;
            this.my_files.ItemHeight = 25;
            this.my_files.Location = new System.Drawing.Point(4, 65);
            this.my_files.Name = "my_files";
            this.my_files.Size = new System.Drawing.Size(313, 504);
            this.my_files.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listView1);
            this.tabPage3.Controls.Add(this.search_bar);
            this.tabPage3.Controls.Add(this.kill_procces_button);
            this.tabPage3.Controls.Add(this.procces_count);
            this.tabPage3.Location = new System.Drawing.Point(4, 34);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(838, 782);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Task manager";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ProcessName,
            this.ProcessID});
            this.listView1.Location = new System.Drawing.Point(4, 63);
            this.listView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(803, 651);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // ProcessName
            // 
            this.ProcessName.Text = "Process Name";
            this.ProcessName.Width = 350;
            // 
            // ProcessID
            // 
            this.ProcessID.Text = "Process ID";
            this.ProcessID.Width = 100;
            // 
            // search_bar
            // 
            this.search_bar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.search_bar.Location = new System.Drawing.Point(484, 22);
            this.search_bar.Name = "search_bar";
            this.search_bar.PlaceholderText = "Search...";
            this.search_bar.Size = new System.Drawing.Size(323, 31);
            this.search_bar.TabIndex = 3;
            this.search_bar.Click += new System.EventHandler(this.search_bar_TextChanged);
            // 
            // kill_procces_button
            // 
            this.kill_procces_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.kill_procces_button.Location = new System.Drawing.Point(3, 725);
            this.kill_procces_button.Name = "kill_procces_button";
            this.kill_procces_button.Size = new System.Drawing.Size(129, 45);
            this.kill_procces_button.TabIndex = 2;
            this.kill_procces_button.Text = "Kill process";
            this.kill_procces_button.UseVisualStyleBackColor = true;
            this.kill_procces_button.Click += new System.EventHandler(this.kill_procces_Click);
            // 
            // procces_count
            // 
            this.procces_count.AutoSize = true;
            this.procces_count.Location = new System.Drawing.Point(10, 27);
            this.procces_count.Name = "procces_count";
            this.procces_count.Size = new System.Drawing.Size(59, 25);
            this.procces_count.TabIndex = 1;
            this.procces_count.Text = "label1";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.pictureBox1);
            this.tabPage4.Location = new System.Drawing.Point(4, 34);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(838, 782);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "ScreenShot";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(832, 776);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // out_put_listener
            // 
            this.out_put_listener.DoWork += new System.ComponentModel.DoWorkEventHandler(this.out_put_listener_DoWork);
            // 
            // screenshotWorker
            // 
            this.screenshotWorker.WorkerSupportsCancellation = true;
            this.screenshotWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.screenshotWorker_DoWork);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 820);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChildForm_FormClosed);
            this.Load += new System.EventHandler(this.Form3_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TextBox text_command;
        private Button button_send_command;
        private RichTextBox textboxCmdResults;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private ListBox my_files;
        private System.ComponentModel.BackgroundWorker out_put_listener;
        private Label procces_count;
        private Button kill_procces_button;
        private TextBox search_bar;
        private ListView listView1;
        private ColumnHeader ProcessName;
        private ColumnHeader ProcessID;
        private ListBox client_files;
        private Button Get_file;
        private TabPage tabPage4;
        private PictureBox pictureBox1;
        private Button Delete_file;
        private Button Upload_file;
        private System.ComponentModel.BackgroundWorker screenshotWorker;
        private Label label2;
        private Label label1;
    }
}