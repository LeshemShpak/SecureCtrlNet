
using System.Text;

using System.Net.Sockets;

using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;


namespace fin_pro
{
    // This class provides a secure connection to a server using the SSL/TLS protocol
    internal class Server_connactor
    {
        // Fields to store server IP address, port, socket object, and SSL/TLS stream object
        private string server_ip;
        private int port;
        private Socket sender;
        private SslStream secureSender;

        // Constructor to create and authenticate a secure connection to the server
        public Server_connactor(string server_ip, int port)
        {
            try
            {
                // Store server IP address and port
                this.server_ip = server_ip;
                this.port = port;

                // Create a new socket object and connect to the server
                this.sender = new Socket(SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(this.server_ip, this.port);

                // Create a new SSL/TLS stream object using a NetworkStream object wrapped around the socket
                // Server certificate validation is provided by the ValidateServerCertificate method
                secureSender = new SslStream(new NetworkStream(sender, true), false, ValidateServerCertificate);

                // Authenticate the client using the Tls12 protocol
                secureSender.AuthenticateAsClient(this.server_ip, new X509CertificateCollection(), SslProtocols.Tls12, false);
            }
            catch (Exception ex)
            {
                // Display error message and exit the program if there's an exception
                MessageBox.Show("The server has closed the connection");
                Environment.Exit(1);
            }
        }

        // Method to send a message to the server
        private void send_message(string message)
        {
            try
            {
                // Convert the message to bytes and prepend the message length in bytes
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);
                byte[] dataToSend = new byte[lengthBytes.Length + messageBytes.Length];
                lengthBytes.CopyTo(dataToSend, 0);
                messageBytes.CopyTo(dataToSend, lengthBytes.Length);

                // Send the data to the server using the SSL/TLS stream object
                secureSender.Write(dataToSend);
            }
            catch (Exception ex)
            {
                // Display error message and exit the program if there's an exception
                MessageBox.Show("The server has closed the connection");
                Environment.Exit(1);
            }
        }

        // This method receives a message from the secure sender and returns the decoded string.
        private string recv_message()
        {
            try
            {
                byte[] lengthBytes = new byte[4];
                try
                {
                    secureSender.Read(lengthBytes); // Read the first 4 bytes which represent the length of the message.
                }
                catch (Exception e)
                {
                    return "";
                }

                int messageLength = BitConverter.ToInt32(lengthBytes, 0); // Convert the byte array to an integer representing the length of the message.

                byte[] messageBytes = new byte[messageLength];
                int bytesRead = 0;
                while (bytesRead < messageLength) // Continue reading until the entire message has been received.
                {
                    int bytesReceived = secureSender.Read(messageBytes, bytesRead, messageLength - bytesRead); // Read the message in chunks.
                    if (bytesReceived == 0)
                    {
                        break; // If no bytes are received, break out of the loop.
                    }
                    bytesRead += bytesReceived;
                }

                return Encoding.UTF8.GetString(messageBytes); // Decode the message bytes and return the string.
            }
            catch (Exception ex)
            {
                MessageBox.Show("The sever has close the connaction");
                Environment.Exit(1); // Exit the application if there is an exception.
                return "";
            }
        }

        // This method sends a message and waits for a reply from the secure sender.
        public string send_message_and_recv(string message)
        {
            send_message(message); // Send the message.
            string reply = recv_message(); // Receive the reply.
            return reply; // Return the reply.
        }

        // This method sends a message but does not wait for a reply from the secure sender.
        public void send_message_no_recv(string message)
        {
            if (message != null)
            {
                send_message(message); // Send the message.
            }
        }

        // This method receives a message but does not send anything to the secure sender.
        public string recv_message_and_no_send()
        {
            string reply = recv_message(); // Receive the message.
            return reply; // Return the message.
        }

        // This method closes the secure sender and the sender.
        public void close()
        {
            try
            {
                secureSender.Close(); // Close the secure sender.
                sender.Close(); // Close the sender.
            }
            catch (Exception ex)
            {
                MessageBox.Show("The sever has close the connaction");
                Environment.Exit(1); // Exit the application if there is an exception.
            }
        }

        // This method is used to validate the server certificate.
        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // Always return true to indicate that the server certificate is valid.
        }

    }
}
