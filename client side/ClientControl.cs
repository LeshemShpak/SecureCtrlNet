namespace fin_pro
{
    public partial class ClientControl : UserControl
    {
        private string ip;
        private string id;
        private string server_ip;
        private bool alert = false;
        private string user_name;

        public ClientControl(string clientId, string ip, string server_ip, string user_name)
        {
            InitializeComponent();

            // Set the IP address label
            ipAddressLabel.Text = ip;

            // Store the IP and client ID
            this.ip = ip;
            this.id = clientId;
            this.server_ip = server_ip;
            this.user_name = user_name;
        }

        private void Disable_all_manage()
        {
            // Disable the manageButton
            manageButton.Enabled = false;

            // Raise the ButtonClickedEvent with the IP address
            ButtonClickedEventArgs args = new ButtonClickedEventArgs
            {
                Ip = this.ip,
            };
            if (ButtonClickedEvent != null)
            {
                ButtonClickedEvent(this, args);
            }
        }

        private void ChildForm_FormClosedEvent(object sender, EventArgs e)
        {
            // Enable the manageButton
            manageButton.Invoke(delegate
            {
                manageButton.Enabled = true;
            });

            // Raise the UnmanageEvent with the IP address
            UnmanageEvent?.Invoke(this, new ButtonClickedEventArgs { Ip = this.ip });
        }

        // Event declarations
        public event EventHandler<ButtonClickedEventArgs> ButtonClickedEvent;
        public event EventHandler<ButtonClickedEventArgs> UnmanageEvent;

        private void manageButton_Click(object sender, EventArgs e)
        {
            // Disable all manage operations
            Disable_all_manage();

            // Create and show Form3

            // Create an instance of Form3 and pass necessary data (server IP, id, user name)
            Form3 form = new Form3(this.server_ip, this.id, this.user_name);

            // Subscribe to the FormClosedEvent of Form3 and specify the event handler
            form.FormClosedEvent += ChildForm_FormClosedEvent;

            // Use Invoke to ensure the form is shown on the UI thread
            this.Invoke(new MethodInvoker(form.Show));
        }

        public void updateValues(bool online, string cpu, string ram, List<Tuple<string, bool>> detections, bool enable)
        {
            if (!online)
            {
                // Client is offline
                manageButton.Enabled = false;
                pictureBox1.Image = Properties.Resources.client_offline;
                toolTip1.SetToolTip(pictureBox1, "Client is offline");
                cpuBar.Value = 0;
                ramBar.Value = 0;
                return;
            }

            // Set CPU and RAM values
            cpuBar.Value = (int)float.Parse(cpu);
            ramBar.Value = (int)float.Parse(ram);

            // Enable or disable the manageButton
            manageButton.Enabled = enable;

            string toolTipText = "";
            bool alert = false;

            // Check if any detectors are activated
            foreach (var detection in detections)
            {
                if (detection.Item2)
                {
                    // Add the activated detector to the tooltip text
                    alert = true;
                    toolTipText += detection.Item1.ToString() + Environment.NewLine;
                }
            }

            if (alert)
            {
                // Set the client image to alert mode
                pictureBox1.Image = Properties.Resources.client_alert;

                // Update tooltip text with activated detectors
                toolTipText = "Activated detectors:" + Environment.NewLine + toolTipText;

                // Set the tooltip for pictureBox1 with the updated text
                toolTip1.SetToolTip(pictureBox1, toolTipText);
            }
            else
            {
                // Set the client image to normal mode
                pictureBox1.Image = Properties.Resources.client;

                // Set the tooltip for pictureBox1 indicating no detectors activated
                toolTip1.SetToolTip(pictureBox1, "No detectors activated");
            }
        }
    }

    public class ButtonClickedEventArgs : EventArgs
    {
        public string Ip { get; set; }
    }
}
