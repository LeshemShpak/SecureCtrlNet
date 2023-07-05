
using System.ComponentModel;

using System.Diagnostics;

using Newtonsoft.Json;


namespace fin_pro
{

    // Define the Form2 class that inherits from the Form class
    public partial class Form2 : Form
    {
        // Define class-level variables
        private string ip;
        private string user_name;
        private string password;
        private Dictionary<string, ClientControl> clientControls = new Dictionary<string, ClientControl>();
        private Server_connactor updaterSocket;
        private List<string> ip_in_control = new List<string>();

        // Define the Form2 constructor that takes an ip parameter
        public Form2(string ip, string user_name, string password)
        {
            // Call the parent constructor
            InitializeComponent();
            
            // Store the passed ip in the class-level ip variable
            this.ip = ip;
            this.user_name = user_name;
            this.password = password;
        }

        // This function retrieves the current CPU and RAM usage and returns a JSON-formatted string of the gathered data.
        private string give_coputer_data_and_get_data(PerformanceCounter counter, PerformanceCounter ramCounter, bool ans_pro, bool ans_dis)
        {
            // Get the current CPU usage as a percentage
            float cpu_usage = counter.NextValue();

            // Get the current RAM usage as a percentage
            float ram_counter = ramCounter.NextValue();

            // Create an anonymous object containing the gathered data

            var data = new
            {
                command = "Rec_information",
                username = this.user_name,
                password = this.password,
                cpu = cpu_usage.ToString("n2"), // Format the CPU usage value as a string with 2 decimal places
                ram = ram_counter.ToString("n2"), // Format the RAM usage value as a string with 2 decimal places
                ips_in_control = string.Join(",", this.ip_in_control),
                epic_games_process = ans_pro,
                diskonkey = ans_dis
            };

            // Serialize the anonymous object to a JSON-formatted string
            var dataStr = JsonConvert.SerializeObject(data);

            // Send the JSON-formatted string to the server and receive a response
            return updaterSocket.send_message_and_recv(dataStr);
        }

        // This function is used to hide the loading bar and show the clients list layout
        private void hide_loading_bar()
        {
            // Invoke the action on the main thread to avoid cross-threading exceptions
            clientsListLayout.Invoke(delegate {
                // Set the loading bar and label visibility to false
                loadingBar.Visible = false;
                loadingLabel.Visible = false;
                // Set the clients list layout visibility to true
                clientsListLayout.Visible = true;
            });
        }
        // This method updates the values (CPU and RAM usage) of a client's control in the center panel
        private void Update_center_control(string clientId, bool online, string cpu, string ram, List<Tuple<string, bool>> detections, bool enable)
        {
            ClientControl client = this.clientControls[clientId];
            client.Invoke(delegate
            {
                client.updateValues(online, cpu, ram, detections, enable);
            });
        }

        // This method handles the button click event of a child form to add it to the list of IPs in control
        private void ChildForm_ButtonClickedEvent(string ip)
        {
            this.ip_in_control.Add(ip);
        }
        // This method creates a new client control in the center panel and adds it to the list of client controls
        private void Create_center_control(string clientId, string ip)
        {
            // Create a new instance of the client control and set its properties
            ClientControl clientControl = new ClientControl(clientId, ip, this.ip, this.user_name);
            clientControl.ButtonClickedEvent += (sender, args) =>
            {
                ChildForm_ButtonClickedEvent(ip);
            };
            clientControl.UnmanageEvent += (sender, args) =>
            {
                string ip = args.Ip;
                this.ip_in_control.Remove(ip);
            };
            // Add the client control to the layout
            clientsListLayout.Invoke(delegate
            {
                // Center the control horizontally in the layout
                clientControl.Anchor = AnchorStyles.None;
                clientControl.Margin = new Padding((this.clientsListLayout.Width - clientControl.Width) / 2, 0, 0, 0);
                clientsListLayout.Controls.Add(clientControl);
            });

            // Add the client control to the dictionary of client controls
            this.clientControls.Add(clientId, clientControl);
        }
        // This method handles the data received from the server for the center panel controls
        private void Handel_center_controls(KeyValuePair<string, dynamic> pair)
        {
            // Extract the necessary data from the dictionary
            string clientId = pair.Key;
            dynamic obj = pair.Value;
            string cpu = obj.cpu.ToString();
            string ram = obj.ram.ToString();
            string ip = obj.ip.ToString();
            bool online = obj.online;
            string manager = obj.manager.ToString();
            bool enable = (manager == "");
            bool epic_games_process = obj.detector_1;
            bool diskonkey = obj.detector_2;
            List<Tuple<string, bool>> detections = new List<Tuple<string, bool>>
            {
                Tuple.Create("Epic games process", epic_games_process),
                Tuple.Create("Disk on key", diskonkey)
            };
            // Check if the client control already exists, and update its values if ites
            if (this.clientControls.ContainsKey(clientId))
            {
                Update_center_control(clientId, online, cpu, ram, detections, enable);
            }
            // If the client control does not exist, create a new one and add it to the layout
            else
            {
                Create_center_control(clientId, ip);
            }
        }
        // This method runs on a separate background thread to continuously update information about client machines
        private void update_info_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Initialize the server connection
            updaterSocket = new Server_connactor(this.ip, Config.port_1);
            // Create a PerformanceCounter to measure the CPU usage
            PerformanceCounter counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");            
            while (true)
            {
                // Get computer data and send it to the server
                bool ans_pro = check_if_epic_games_process_is_open();
                bool ans_dis = check_if_disk_on_key();
                string infoStr = give_coputer_data_and_get_data(counter, ramCounter, ans_pro, ans_dis);

                // Deserialize the received data
                Dictionary<string, dynamic> dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(infoStr);
                if (dict != null)
                {
                    // Iterate through the deserialized objects
                    foreach (KeyValuePair<string, dynamic> pair in dict)
                    {
                        Handel_center_controls(pair);
                    }
                }

                // If the loading bar is still visible, hide it and show the client list layout
                if (loadingBar.Visible)
                {
                    hide_loading_bar();
                }

                // Wait for 1 second before checking for updates again
                Thread.Sleep(1000);
            }
        }

        // This method runs when Form2 is loaded and starts the update_info_worker and listen_to_command workers
        private void Form2_Load(object sender, EventArgs e)
        {
            update_info_worker.RunWorkerAsync();
            listen_to_command.RunWorkerAsync();
        }

        // This method runs when Form2 is closed and closes the updaterSocket and the entire application
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            updaterSocket.close();
            Application.Exit();
        }
        // This method checks if the Epic Games Launcher process is currently running
        private bool check_if_epic_games_process_is_open()
        {
            // Get an array of the names of all running processes
            string[] process_names = get_my_process_names();
            // Check if any process name contains "epicgameslauncher"
            foreach (string process_name in process_names)
            {
                if (process_name.ToLower().Contains("notepad"))
                {
                    return true;
                }
            }
            return false;
        }
        // Check if any process name contains "epicgameslauncher"
        private string[] get_my_process_names()
        {
            // Get an array of all running processes
            Process[] processes = Process.GetProcesses();
            // Create a string array to hold the names of the processes
            string[] process_names = new string[processes.Length];
            int i = 0;

            // Loop through each process and add its name to the string array
            foreach (Process process in processes)
            {
                process_names[i++] = process.ProcessName;
            }
            return process_names;
        }
        // This method checks if a USB flash drive is currently connected to the system
        private bool check_if_disk_on_key()
        {
            // Get an array of all drives on the system
            DriveInfo[] drives = DriveInfo.GetDrives();
            // Check if any drive is a removable disk that is ready to be accessed
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Removable && drive.IsReady)
                {
                    return true;
                }
            }
            return false;

        }
        // This method runs the listen_to_command worker and listens for incoming command requests from the server
        private void listen_to_command_DoWork(object sender, DoWorkEventArgs e)
        {
            // Create a socket to listen for incoming commands from the server
            Server_connactor socket = new Server_connactor(this.ip, Config.port_2);
            Incoming_commands executioner = new Incoming_commands();
            while (true)
            {
                // Receive the incoming message from the server and deserialize it into CommandInfo object
                string message = socket.recv_message_and_no_send();
                CommandInfo info = JsonConvert.DeserializeObject<CommandInfo>(message);

                // Create a new  to send the response back to the server
                Server_connactor new_socket = new Server_connactor(this.ip, Config.port_1);

                if (info == null)
                {
                    MessageBox.Show("The connection was closed");
                    // Close the program
                    Environment.Exit(0);
                }

                Handel_command(info, executioner, new_socket);

                // Close the socket
                new_socket.close();
            }
        }
        private void Handel_command(CommandInfo info, Incoming_commands executioner, Server_connactor new_socket)
        {

            // Check the type of command received and execute the corresponding function
            if (info.command == "Run_command")
            {
                executioner.Run_command(info.cmd_to_execute, info.asking_ip, new_socket);
            }
            else if (info.command == "Get_all_processes")
            {
                executioner.Get_all_process(info.asking_ip, new_socket, this.user_name);
            }
            else if (info.command == "Kill_process")
            {
                executioner.Kill_process(info.process_id, info.asking_ip, new_socket);
            }
            else if (info.command == "Get_all_files_name")
            {
                executioner.Get_all_files_name(info.asking_ip, new_socket, this.user_name);
            }
            else if (info.command == "Get_file")
            {
                executioner.Get_file(info.asking_ip, new_socket, info.file_name);
            }
            else if (info.command == "Upload_file")
            {
                executioner.Upload_file(info.asking_ip, new_socket, info.file_name, info.file_data);
            }
            else if (info.command == "Delete_file")
            {
                executioner.Delete_file(info.asking_ip, new_socket, info.file_name);
            }
            else if (info.command == "Get_screenshot")
            {
                executioner.Get_screenshot(info.asking_ip, new_socket);
            }
        }
    }
}
