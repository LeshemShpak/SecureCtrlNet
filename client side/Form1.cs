
using Newtonsoft.Json;

namespace fin_pro
{
    //The following script is a partial class for the Form1 of a GUI application. This script is responsible for handling the user registration, login and IP validation.
    public partial class Form1 : Form
    {
        // Constructor for Form1
        public Form1()
        {
            InitializeComponent();
        }

        // Event handler for the register button
        private void register_click(object sender, EventArgs e)
        {
            // Check if the entered IP address is valid
            if (check_valid_ip())
            {
                //Construct the registration data object and serialize it to JSON
                var data = new
                {
                    command = "Reg_user",
                    username = this.user_name.Text,
                    password = this.password.Text,
                };
                var dataStr = JsonConvert.SerializeObject(data);
                try
                {
                    //Start the background worker to connect to the server and log in the user
                    login_or_register_worker.RunWorkerAsync(dataStr);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                MessageBox.Show("invalid ip");
            }
        }

        // Event handler for the login button
        private void login_click(object sender, EventArgs e)
        {
            // Check if the entered IP address is valid
            if (check_valid_ip())
            {
                //Construct the login data object and serialize it to JSON
                var data = new
                {
                    command = "Log_in",
                    username = this.user_name.Text,
                    password = this.password.Text,
                };
                var dataStr = JsonConvert.SerializeObject(data);
                try
                {
                    //Start the background worker to connect to the server and log in the user
                    login_or_register_worker.RunWorkerAsync(dataStr);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                MessageBox.Show("invalid ip");
            }
        }

        // This method checks whether the IP address entered by the user is valid
        private bool check_valid_ip()
        {
            // Split the IP address string into an array of strings separated by dots
            string[] a = this.text_ip.Text.Split(".");
            if (a.Length < 4) // If there are less than 4 segments in the IP address
            {
                return false; // The IP address is invalid
            }
            int num;
            for (int i = 0; i < a.Length; i++) // Loop through each segment of the IP address
            {
                num = Convert.ToInt32(a[i]); // Convert the segment to an integer
                if ((num >= 255) || (num < 0)) // If the integer is greater than or equal to 255 or less than 0
                {
                    return false; // The IP address is invalid
                }
            }
            return true; // The IP address is valid
        }

        // This method executes in a background worker thread to send a message to the server and receive a response
        private void login_or_register_worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try 
            {
                // Create a new instance of the Server_connactor class with the IP address and port number
                Server_connactor socket = new Server_connactor(this.text_ip.Text, Config.port_1);
                string message;
                if ((e.Argument.ToString()) == null) // If the argument passed to the method is null
                {
                    message = "@"; // Set the message to a default value
                }
                else
                {
                    message = e.Argument.ToString(); // Set the message to the value passed in the argument
                }
                string server_mes = socket.send_message_and_recv(message); // Send the message to the server and receive the response
                if (server_mes == "OK") // If the response from the server is "OK"
                {
                    socket.close(); // Close the connection to the server
                    Form2 form = new Form2(this.text_ip.Text, this.user_name.Text, this.password.Text); // Create a new instance of the Form2 class with the IP address
                    this.Invoke(new MethodInvoker(this.Hide)); // Hide the current form
                    this.Invoke(new MethodInvoker(form.Show)); // Show the new form
                }
                else if (server_mes == "exist") // If the response from the server is "exist"
                {
                    MessageBox.Show("username taken"); // Display an error message
                }
                else // If the response from the server is anything else
                {
                    MessageBox.Show("wrong username or password"); // Display an error message
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show("Faild to open communicatin with the server");
            }
        }

        // This method executes when the form is loaded
        private void Form1_Load(object sender, EventArgs e)
        {
            //this.text_ip.Text = Config.SERVER_IP; // Set the IP address text box to the default value from the Config class
        }
    }
}