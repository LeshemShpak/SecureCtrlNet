using System.ComponentModel;
using Newtonsoft.Json;


namespace fin_pro
{
    public partial class Form3 : Form
    {
        private string ip;
        private string id;
        private Server_connactor socket;
        private Server_connactor outputSocket;
        private ProcessInfo[] pro_list;
        private bool updateScreenshot = false;
        private string user_name;

        public Form3(string ip, string id, string user_name)
        {
            InitializeComponent();
            CenterToScreen();
            this.ip = ip;
            this.id = id;
            this.user_name = user_name;
        }

        public event EventHandler FormClosedEvent;

        private void Form3_Load(object sender, EventArgs e)
        {
            this.Text = "Managing " + id;

            // Start the output listener worker thread.
            out_put_listener.RunWorkerAsync();

            // Initialize the secure sender.
            socket = new Server_connactor(this.ip, Config.port_1);

            // Set the list view to full row select mode.
            listView1.FullRowSelect = true;
        }
        // Checks if a folder exists at the specified path, and creates it if it doesn't.
        public static void CheckAndCreateFolder(string path)
        {
            // Check if the folder at the specified path exists
            if (!Directory.Exists(path))
            {
                // If the folder doesn't exist, create it
                Directory.CreateDirectory(path);
            }
        }
        // Event handler for the Send Command button click event.
        private void send_command_click(object sender, EventArgs e)
        {
            string mes = "";
            if (this.text_command.Text == "")
            {
                MessageBox.Show("indaliv command");
                return;
            }
            else
            {
                mes = this.text_command.Text;
            }
            button_send_command.Enabled = false;

            // Construct the message data.
            var data = new
            {
                command = "Run_command",
                username = "ron",
                id = this.id,
                cmd = mes,
            };
            var dataStr = JsonConvert.SerializeObject(data);

            // Send the message.
            socket.send_message_no_recv(dataStr);
        }

        // Method to update the my_files list with the names of all files in the specified directory.
        private void My_files_name()
        {
            CheckAndCreateFolder(Config.path);
            string[] my_files_names = Directory.GetFiles(Config.path);
            my_files.Invoke(delegate
            {
                my_files.DataSource = my_files_names;
            });
        }

        // Event handler for the tab control's SelectedIndexChanged event.
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Name != "tabPage4")
            {
                screenshotWorker.CancelAsync();
            }
            if (tabControl1.SelectedTab.Name == "tabPage2")
            {
                // Construct the message data to get the names of all files in the specified directory.
                var data = new
                {
                    command = "Get_all_files_name",
                    id = this.id,
                    username = this.user_name,
                };
                var dataStr = JsonConvert.SerializeObject(data);

                // Send the message.
                socket.send_message_no_recv(dataStr);

                // Update the my_files list.
                My_files_name();
            }
            else if (tabControl1.SelectedTab.Name == "tabPage3")
            {
                // Construct the message data to get the list of all processes.
                var data = new
                {
                    command = "Get_all_processes",
                    id = this.id,
                    username = this.user_name,
                };
                var dataStr = JsonConvert.SerializeObject(data);

                // Send the message.
                socket.send_message_no_recv(dataStr);
            }
            else if (tabControl1.SelectedTab.Name == "tabPage4")
            {
                if (!screenshotWorker.IsBusy)
                {
                    // Start the screenshot worker thread.
                    screenshotWorker.RunWorkerAsync();
                }
            }
        }
        // This method updates the UI with the result of a command
        public void Run_command_result_func(dynamic command_info)
        {
            string out_put = command_info.out_put;
            textboxCmdResults.Invoke(delegate
            {
                // Set the output of the command to the textbox
                textboxCmdResults.Text = out_put.Trim();
                // Re-enable the button for sending commands
                button_send_command.Enabled = true;
                // Set focus to the command textbox
                text_command.Focus();
            });
        }

        // This method updates the UI with a list of running processes
        public void Get_all_processes_result_func(dynamic command_info)
        {
            // Deserialize the output from the server into an array of ProcessInfo objects
            this.pro_list = command_info.out_put.ToObject<ProcessInfo[]>();
            listView1.Invoke(delegate
            {
                // Clear the list view and search bar
                listView1.Items.Clear();
                search_bar.TextChanged -= search_bar_TextChanged;
                search_bar.Text = "";
                search_bar.TextChanged += search_bar_TextChanged;
                // Add each process to the list view
                foreach (ProcessInfo p in pro_list)
                {
                    ListViewItem item = new ListViewItem(p.name);
                    item.SubItems.Add(p.id.ToString());
                    listView1.Items.Add(item);
                }
                // Display the number of processes
                procces_count.Text = "Total Processes:  " + this.pro_list.Length.ToString();
            });
        }

        // This method is called after attempting to kill a process
        public void Kill_process_result_func(dynamic command_info)
        {
            if ((bool)command_info.success)
            {
                // Get updated processes list
                var data = new
                {
                    command = "Get_all_processes",
                    id = this.id,
                    username = this.user_name

                };
                var dataStr = JsonConvert.SerializeObject(data);
                socket.send_message_no_recv(dataStr);
            }
            else
            {
                MessageBox.Show("Failed to kill process");
            }
            // Re-enable the button for killing processes
            kill_procces_button.Invoke(delegate
            {
                kill_procces_button.Enabled = true;
            });
        }

        // This method updates the UI with a list of files on the client machine
        private void Get_all_files_names_result_func(dynamic command_info)
        {
            string[] files_names_client = command_info.out_put.ToObject<string[]>();
            client_files.Invoke(delegate
            {
                // Set the list of files to the data source of the client_files list box
                client_files.DataSource = files_names_client;
            });
        }

        // This method is called after successfully downloading a file from the client machine
        private void Get_file_result_func(dynamic command_info)
        {

            CheckAndCreateFolder(Config.path);
            string path = command_info.file_name;
            string filePath = Config.path + "\\" + path;
            MessageBox.Show(filePath);
            byte[] fileBytes = command_info.out_put;

            // Open a new file stream with write access
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                // Write the byte array data to the file stream
                fileStream.Write(fileBytes, 0, fileBytes.Length);
            }
            // Update the list of files
            My_files_name();
            // Re-enable the button for downloading files
            Get_file.Invoke(delegate
            {
                Get_file.Enabled = true;
            });
        }
        // This function handles the result of the "Upload_file" command.
        // It checks whether the upload was successful or not, and then sends a new command to update the list of files.
        // It also enables the "Upload_file" button.
        private void Upload_file_result_func(dynamic command_info)
        {
            if ((bool)command_info.success)
            {
                // Get updated files list
                var data = new
                {
                    command = "Get_all_files_name",
                    id = this.id,
                    username= this.user_name,
                };
                var dataStr = JsonConvert.SerializeObject(data);
                socket.send_message_no_recv(dataStr);
            }
            else
            {
                MessageBox.Show("Failed to upload file");
            }
            Upload_file.Invoke(delegate
            {
                Upload_file.Enabled = true;
            });
        }

        // This function handles the result of the "Delete_file" command.
        // It checks whether the deletion was successful or not, and then sends a new command to update the list of files.
        // It also enables the "Delete_file" button and calls the function to update the list of files.
        private void Delete_file_result_func(dynamic command_info)
        {
            if ((bool)command_info.success)
            {
                // Get updated files list
                var data = new
                {
                    command = "Get_all_files_name",
                    id = this.id,
                    username= this.user_name,
                };
                var dataStr = JsonConvert.SerializeObject(data);
                socket.send_message_no_recv(dataStr);
            }
            else
            {
                MessageBox.Show("Failed to delete file");
            }
            Delete_file.Invoke(delegate
            {
                Delete_file.Enabled = true;
            });
            My_files_name(); // call function to update list of files
        }

        // This function handles the result of the "Get_screenshot" command.
        // It converts the base64 string received in the command_info to an Image and displays it in a PictureBox.
        private void Get_screenshot_result_func(dynamic command_info)
        {
            // in the command_info.out_put there is the data for the screenshot
            string base64String = command_info.out_put;
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes);
            Image image = Image.FromStream(ms);
            pictureBox1.Image = image;
        }

        // This function continuously listens for incoming messages on the third socket connection to receive results from commands sent to the server.
        // It parses the incoming messages and calls the appropriate function to handle the result based on the command.
        private void out_put_listener_DoWork(object sender, DoWorkEventArgs e)
        {
            Server_connactor outputSocket = new Server_connactor(this.ip, Config.port_3);
            while (true)
            {
                string message = outputSocket.recv_message_and_no_send();
                CommandInfo_Ans info = JsonConvert.DeserializeObject<CommandInfo_Ans>(message);
                if (info == null) { break; }
                string command = info.command;
                dynamic command_info = JsonConvert.DeserializeObject<dynamic>(message);
                if (command == "Run_command_result")
                {
                    Run_command_result_func(command_info);
                }
                else if (command == "Get_all_processes_result")
                {
                    Get_all_processes_result_func(command_info);
                }
                else if (command == "Kill_process_result")
                {
                    Kill_process_result_func(command_info);
                }
                else if (command == "Get_all_files_names_result")
                {
                    Get_all_files_names_result_func(command_info);
                }
                else if (command == "Get_file_result")
                {
                    Get_file_result_func(command_info);
                }
                else if (command == "Upload_file_result")
                {
                    Upload_file_result_func(command_info);
                }
                else if (command == "Delete_file_result")
                {
                    Delete_file_result_func(command_info);
                }
                else if (command == "Get_screenshot_result")
                {
                    Get_screenshot_result_func(command_info);
                }
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the socket connections and cancel the screenshot worker if it's busy
            if (socket != null)
            {
                socket.close();
            }
            if (outputSocket != null)
            {
                outputSocket.close();
            }
            if (screenshotWorker.IsBusy)
            {
                screenshotWorker.CancelAsync();
            }
        }

        private void ChildForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Trigger the FormClosedEvent if it's not null
            if (FormClosedEvent != null)
            {
                FormClosedEvent(this, EventArgs.Empty);
            }
        }

        private void AddChildNodesFromFolderStructure(dynamic folderStructure, TreeNode parentNode)
        {
            // Recursively add child nodes to the parent node for the given folder structure
            if (folderStructure.isFolder)
            {
                foreach (var child in folderStructure.children)
                {
                    // Create a new node for the child folder or file
                    var childNode = new TreeNode(child.name);
                    childNode.Tag = child;

                    // Recursively add child nodes for the child folder or file
                    AddChildNodesFromFolderStructure(child, childNode);

                    // Add the child node to the parent node
                    parentNode.Nodes.Add(childNode);
                }
            }
        }

        private void kill_procces_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the selected item from the list view and extract the process ID
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string processIdText = selectedItem.SubItems[1].Text;
                int processId = int.Parse(processIdText);

                // Send a command to kill the selected process to the server
                var data = new
                {
                    command = "Kill_process",
                    process_id = processId,
                    id = this.id,
                };
                var dataStr = JsonConvert.SerializeObject(data);
                socket.send_message_no_recv(dataStr);

                // Disable the kill process button to prevent multiple clicks
                kill_procces_button.Enabled = false;
            }
            catch (Exception ex)
            {
                // Show an error message if there is no selected item or the process ID cannot be parsed
                MessageBox.Show("Please select a valid processor");
            }
        }

        // This method filters the processes in the list view based on the text in the search bar
        private void search_bar_TextChanged(object sender, EventArgs e)
        {
            string searchText = search_bar.Text.ToLower();
            // Clear the current items in the list view and add new items that match the search text
            listView1.Invoke(delegate
            {
                listView1.Items.Clear();
                foreach (ProcessInfo p in pro_list)
                {
                    if (p.name.ToLower().Contains(searchText))
                    {
                        ListViewItem item = new ListViewItem(p.name);
                        item.SubItems.Add(p.id.ToString());
                        listView1.Items.Add(item);
                    }
                }
            });
        }

        // This method sends a command to the server to retrieve a specific file from the client
        private void Get_file_Click(object sender, EventArgs e)
        {
            string fullPath = "";
            if (client_files.SelectedItem != null)
            {
                fullPath = client_files.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select an existing file");
                return;
            }
            string name_of_the_file = Path.GetFileName(fullPath);
            var data = new
            {
                command = "Get_file",
                file_name = name_of_the_file,
                id = this.id,
            };
            var dataStr = JsonConvert.SerializeObject(data);
            socket.send_message_no_recv(dataStr);
            Get_file.Enabled = false;
        }

        // This method sends a command to the server to delete a specific file from the client
        private void Delete_file_Click(object sender, EventArgs e)
        {
            string fullPath = "";
            if (client_files.SelectedItem != null)
            {
                fullPath = client_files.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select an existing file");
                return;
            }
            string name_of_the_file = Path.GetFileName(fullPath);
            var data = new
            {
                command = "Delete_file",
                file_name = name_of_the_file,
                id = this.id,
            };
            var dataStr = JsonConvert.SerializeObject(data);
            socket.send_message_no_recv(dataStr);
            Delete_file.Enabled = false;
        }
        // This method gets the binary data of a specific file
        private byte[] Get_file_data(string file_name)
        {
            CheckAndCreateFolder(Config.path);
            string filePath = Config.path + "\\" + file_name;
            byte[] buffer;
            // Open the file as a stream
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                // Create a buffer to hold the binary data
                buffer = new byte[fileStream.Length];

                // Read the binary data into the buffer
                fileStream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }
        // This method sends a command to the server to upload a specific file to the client
        private void Upload_file_Click(object sender, EventArgs e)
        {
            string fullPath = "";
            if (my_files.SelectedItem != null)
            {
                fullPath = my_files.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select an existing file");
                return;
            }
            try
            {
                string name_of_the_file = Path.GetFileName(fullPath);

                // Create a new object that contains the necessary data for the "Upload_file" command
                var data = new
                {
                    command = "Upload_file",
                    file_name = name_of_the_file,
                    file_data = Get_file_data(name_of_the_file),
                    id = this.id,
                };

                // Convert the object to a JSON string and send it to the server
                var dataStr = JsonConvert.SerializeObject(data);
                socket.send_message_no_recv(dataStr);
                Upload_file.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("OS problem, can't extract file data");
                return;
            }
        }

        private void screenshotWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Continuously send the "Get_screenshot" command to the server while the worker is running
            while (!screenshotWorker.CancellationPending)
            {
                // Create a new object that contains the necessary data for the "Get_screenshot" command
                var data = new
                {
                    command = "Get_screenshot",
                    id = this.id,
                };

                // Convert the object to a JSON string and send it to the server
                var dataStr = JsonConvert.SerializeObject(data);
                socket.send_message_no_recv(dataStr);

                // Wait for 2 seconds before sending the next command
                Thread.Sleep(2000);
            }
        }

    }
}
