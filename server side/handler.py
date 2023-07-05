import threading
import datetime
import socket
import sqlite3
import json
import ssl
import struct
import time
from user import User


def send_by_chunks(conn, msg):
    try:
        # This method sends a message in chunks to a given connection.
        # Convert the message string to bytes using UTF-8 encoding.
        message_bytes = msg.encode('utf-8')
        # Pack the length of the message in bytes using struct.pack()
        # function with 'I' as the format string, where 'I' stands for unsigned int (4 bytes).
        length_bytes = struct.pack('I', len(message_bytes))
        # Concatenate the length and message bytes to create the final data to be sent.
        data_to_send = length_bytes + message_bytes
        # Send the data to the connection using conn.sendall() method,
        # which ensures that all data is sent in chunks.
        conn.sendall(data_to_send)
    except (OSError):
        return


# This class represents a handler object that holds the necessary information to handle incoming client requests
class Handler:

    # Initialize the handler object with the provided parameters
    def __init__(self, command, message, address, cur, conn, client_sock):
        self.command = command  # The command requested by the client
        self.message = message  # The message sent by the client
        self.address = address  # The IP address and port number of the client
        self.cur = cur  # The cursor object to execute SQL statements
        self.conn = conn  # The connection object to communicate with the database
        self.client_sock = client_sock  # The client socket object for communication with the client

    def handle_reg_client(self, users_info):
        # get the user's name and password from the message
        user_name, password = self.message['username'], self.message['password']

        # query the database for the user's id based on their name
        self.cur.execute("select id from users where name=?", [user_name, ])
        user_id = self.cur.fetchone()

        # insert the client's IP address and user id into the Clients table
        self.cur.execute("INSERT INTO Clients (ip, user) Values (?,?)", [self.address[0], user_id[0]])
        self.conn.commit()
        self.cur.execute("select id, password from users where name=?", [user_name, ])
        user_id = self.cur.fetchone()
        # query the database for the client's id based on their IP address
        self.cur.execute("select id from Clients where ip=? and user=?", [self.address[0], user_id[0]])
        client_id = self.cur.fetchone()

        # create a new User object if the user id doesn't exist in the users_info dictionary
        if user_id[0] not in users_info:
            users_info[user_id[0]] = User(user_id[0])

        # add the client to the User's list of clients
        users_info[user_id[0]].add_client(client_id[0], self.address[0])

    def handle_reg_user(self, users_info):
        # extract the username and password from the message
        user_name, password = self.message['username'], self.message['password']
        # check if user with the given username already exists in the database
        self.cur.execute("select id from users where name=?", [user_name, ])
        user_id = self.cur.fetchone()
        if user_id is not None:
            # if user already exists, send "exist" message to client
            print("Client: "+self.address[0] +" is tring to reg with the User Name: "+user_name+" but this user name is already exists")
            send_by_chunks(self.client_sock, "exist")
        else:
            # if user does not exist, insert new user into the database
            self.cur.execute("INSERT INTO USERS (name, password) Values (?,?)", [user_name, password])
            # call handle_reg_client() to register the client to the user
            self.handle_reg_client(users_info)
            # send "OK" message to the client
            send_by_chunks(self.client_sock, "OK")
            print("New user register: " + user_name)

    def handle_log_in(self, users_info):
        # Retrieve the username and password from the message
        user_name, password = self.message['username'], self.message['password']

        # Check if the user with the given username exists and retrieve their ID and password
        self.cur.execute("select id, password from users where name=?", [user_name, ])
        user_id = self.cur.fetchone()

        # If the user exists and the password is correct
        if user_id is not None and user_id[1] == password:
            print("Client: " + self.address[0] + " log in with the user name: " +user_name)

            # Check if the client with the current IP address already exists in the database
            self.cur.execute("select id from Clients where ip=? AND user=?", [self.address[0], user_id[0]])
            client = self.cur.fetchone()

            # Send an "OK" message to the client to indicate successful login
            send_by_chunks(self.client_sock, "OK")
            # If the client does not exist in the database, register it
            if client == None:
                self.handle_reg_client(users_info)
        else:
            # Send a message to the client indicating that the username or password is incorrect
            send_by_chunks(self.client_sock, "bad password or user_name")

    def update_clients_in_control(self, users_info, user_id):
        self.cur.execute("select ip from Clients where user=?", [user_id])
        all_ips = self.cur.fetchall()
        managed_ips = self.message["ips_in_control"].split(",")
        print(
            "list of ips in control by - " + self.message["username"] + " from computer - " + self.address[0] + ":   ",
            managed_ips)
        the_user = users_info[user_id]
        for ip in all_ips:
            self.cur.execute("select id from Clients where ip=? AND user=?", [ip[0], user_id])
            client_id = self.cur.fetchone()[0]
            # ip is being managed by this adress
            if ip[0] in managed_ips:
                the_user.clients[client_id].manager = self.address[0]

            # ip is not managed by this address. only reset it if it was managed by this address,
            # otherwise don't touch it since it has another manager
            elif the_user.clients[client_id].manager == self.address[0]:
                the_user.clients[client_id].manager = ""

            elif the_user.clients[client_id].online == False:
                the_user.clients[client_id].manager = ""


    def update_client_info(self, user_id, client_id, users_info):
        # extract CPU and RAM usage from message
        cpu_usage = self.message['cpu']
        ram_usage = self.message['ram']

        epic_games_process = self.message['epic_games_process']
        diskonkey = self.message['diskonkey']

        # get the corresponding User object
        the_user = users_info[user_id]
        # update the CPU and RAM usage for this client

        the_user.clients[client_id].cpu = cpu_usage
        the_user.clients[client_id].ram = ram_usage

        the_user.clients[client_id].detector_1 = epic_games_process
        the_user.clients[client_id].detector_2 = diskonkey

        the_user.clients[client_id].last_update = datetime.datetime.now().isoformat()


        # get dictionary representation of client information for this user
        data = {}
        current_time = datetime.datetime.now().isoformat()
        for id, client in the_user.clients.items():
            time_difference = datetime.datetime.fromisoformat(current_time) - datetime.datetime.fromisoformat(client.last_update)
            if time_difference.total_seconds() < 5:
                client.online = True
            else:
                client.online = False
            data[id] = client.__dict__
        #data = {k: v.__dict__ for k, v in the_user.clients.items()}

        # serialize the updated client information as a JSON string
        data_str = json.dumps(data)

        # send the updated client information back to the client
        send_by_chunks(self.client_sock, data_str)

    def handle_rec_information(self, users_info):
        # Get the username and password from the message
        user_name, password = self.message['username'], self.message['password']
        # Check if the user with the given username exists and get the user ID and password
        self.cur.execute("select id, password from users where name=?", [user_name, ])
        user_id = self.cur.fetchone()
        # Check if the client with the given IP address exists and get the client ID
        self.cur.execute("select id from Clients where ip=? AND user=?", [self.address[0], user_id[0]])
        client_id = self.cur.fetchone()
        self.update_clients_in_control(users_info, user_id[0])
        # Call the update_client_info method to update the CPU and RAM usage of the client
        self.update_client_info(user_id[0], client_id[0], users_info)

    def handle_run_command(self, client_sockets):
        # get IP address of the client sending the command
        ip_of_asking_client = self.address[0]

        # get ID of the client that should run the command
        client_id = self.message['id']

        # query the IP address of the client with the given ID
        self.cur.execute("select ip from clients where id=?", [client_id, ])
        ip_client_running_the_command = self.cur.fetchone()[0]

        # get socket associated with the IP address of the client that should run the command
        sock = client_sockets.get(ip_client_running_the_command)

        # create a dictionary with information about the command to be run
        data = {
            "command": self.message['command'],
            "asking_ip": ip_of_asking_client,
            "cmd_to_execute": self.message['cmd']
        }
        data_str = json.dumps(data)

        # send the command information to the client that should run the command
        send_by_chunks(sock, data_str)

    def handle_run_command_result(self, out_put_sockets):
        # Get the cmd result and addressed client from the incoming message
        the_cmd_result = self.message["out_put"]
        asking_client = self.message["asking_ip"]
        # Get the socket associated with the addressed client from the dictionary
        sock = out_put_sockets[asking_client]
        # Prepare the data to be sent back to the addressed client
        command = self.message['command']
        data = {
            "out_put": the_cmd_result,
            "command": command
        }
        # Convert the data dictionary to a JSON string and send it to the addressed client socket
        data_str = json.dumps(data)
        send_by_chunks(sock, data_str)

    def handle_get_all_processes(self, client_sockets):
        # Get the IP address of the client that sent the request
        ip_of_asking_client = self.address[0]
        self.cur.execute("select id from users where name=?", [self.message['username'],])
        user_id = self.cur.fetchone()[0]
        # Get the ID of the client that sent the request
        self.cur.execute("select id from clients where ip=? AND user =?", [ip_of_asking_client, user_id])
        asking_client_id = self.cur.fetchone()[0]

        # Get the ID of the client that will execute the command
        client_id = self.message['id']
        # Get the IP address of the client that will execute the command
        self.cur.execute("select ip from clients where id=?", [client_id, ])
        ip_client_running_the_command = self.cur.fetchone()[0]

        # Get the socket associated with the IP address of the client that will execute the command
        sock = client_sockets.get(ip_client_running_the_command)
        # Create the message that will be sent to the client that will execute the command
        data = {
            "command": self.message['command'],
            "asking_ip": ip_of_asking_client
        }
        data_str = json.dumps(data)
        # Send the message to the client that will execute the command
        send_by_chunks(sock, data_str)

    def handle_get_all_processes_result(self, out_put_sockets):
        # Get names and the id of the processes from the message
        the_processes = self.message["the_processes"]
        # Get the client who asked for the process names
        asking_client = self.message["asking_ip"]
        # Get the socket of the client who asked for the process names
        sock = out_put_sockets[asking_client]
        # Get the command that was executed to get the process names
        command = self.message["command"]
        # Create a dictionary containing the process names and the command executed
        data = {
            "out_put":  the_processes,
            "command": command
        }
        # Convert the dictionary to a JSON string
        data_str = json.dumps(data)
        # Send the JSON string back to the client who asked for the process names
        send_by_chunks(sock, data_str)

    def handle_kill_process(self, client_sockets):
        # Get the IP of the client making the request
        ip_of_asking_client = self.address[0]

        # Get the ID of the client making the request
        client_id = self.message["id"]

        # Get the IP of the client running the command
        self.cur.execute("select ip from clients where id=?", [client_id, ])
        ip_client_running_the_command = self.cur.fetchone()[0]

        # Get the socket for the client running the command
        sock = client_sockets.get(ip_client_running_the_command)

        # Construct the message to send to the client running the command
        data = {
            "command": self.message["command"],
            "asking_ip": ip_of_asking_client,
            "process_id": self.message["process_id"]
        }

        # Convert the message to a JSON string
        data_str = json.dumps(data)

        # Send the message to the client running the command
        send_by_chunks(sock, data_str)

    def handle_kill_process_result(self, out_put_sockets):
        # Get the client that asked for the process to be killed
        asking_client = self.message["addressed_client"]
        # Get the socket of the client that asked for the process to be killed
        sock = out_put_sockets[asking_client]
        # Create the response message with the killed process ID
        data = {
            "process_id": self.message["process_id"],
            "command": self.message["command"],
            "success": self.message["success"]
        }
        # Convert the message to a JSON string and send it to the client
        data_str = json.dumps(data)
        send_by_chunks(sock, data_str)

    def handle_get_all_files_name(self, client_sockets):
        # Get the IP address of the client that sent the request
        ip_of_asking_client = self.address[0]
        self.cur.execute("select id from users where name=?", [self.message['username'],])
        user_id = self.cur.fetchone()[0]
        # Get the ID of the client that sent the request
        self.cur.execute("select id from clients where ip=? AND user=?", [ip_of_asking_client,user_id])
        asking_client_id = self.cur.fetchone()[0]

        # Get the ID of the client that will execute the command
        client_id = self.message['id']
        # Get the IP address of the client that will execute the command
        self.cur.execute("select ip from clients where id=?", [client_id, ])
        ip_client_running_the_command = self.cur.fetchone()[0]
        # Get the socket associated with the IP address of the client that will execute the command
        sock = client_sockets.get(ip_client_running_the_command)
        # Create the message that will be sent to the client that will execute the command
        data = {
            "command": self.message['command'],
            "asking_ip": ip_of_asking_client
        }
        data_str = json.dumps(data)
        # Send the message to the client that will execute the command
        send_by_chunks(sock, data_str)

    def handle_get_all_files_name_result(self, out_put_sockets):
        # Get names and the id of the files from the message
        files_names = self.message["files_names"]
        # Get the client who asked for the process names
        asking_client = self.message["addressed_client"]
        # Get the socket of the client who asked for the process names
        sock = out_put_sockets[asking_client]
        # Get the command that was executed to get the process names
        command = self.message["command"]
        # Create a dictionary containing the process names and the command executed
        data = {
            "out_put": files_names,
            "command": command
        }
        # Convert the dictionary to a JSON string
        data_str = json.dumps(data)
        # Send the JSON string back to the client who asked for the process names
        send_by_chunks(sock, data_str)

    def handle_get_file(self, client_sockets):
        # Get the name of the file
        file_name = self.message["file_name"]
        # Get the IP address of the client that sent the request
        ip_of_asking_client = self.address[0]
        # Get the ID of the client that will execute the command
        client_id = self.message['id']
        # Get the IP address of the client that will execute the command
        self.cur.execute("select ip from clients where id=?", [client_id, ])
        ip_client_running_the_command = self.cur.fetchone()[0]
        # Get the socket associated with the IP address of the client that will execute the command
        sock = client_sockets.get(ip_client_running_the_command)
        # Create the message that will be sent to the client that will execute the command
        data = {
            "command": self.message['command'],
            "file_name": file_name,
            "asking_ip": ip_of_asking_client
        }
        data_str = json.dumps(data)
        # Send the message to the client that will execute the command
        send_by_chunks(sock, data_str)

    def handle_get_file_result(self, out_put_sockets):
        # Get the data of the requested file,from the message
        file_data = self.message["file_data"]
        # Get the client who asked for the process names
        asking_client = self.message["addressed_client"]
        file_name = self.message["file_name"]
        # Get the socket of the client who asked for the process names
        sock = out_put_sockets[asking_client]
        # Get the command that was executed to get the process names
        command = self.message["command"]
        # Create a dictionary containing the process names and the command executed
        data = {
            "out_put": file_data,
            "command": command,
            "file_name": file_name
        }
        # Convert the dictionary to a JSON string
        data_str = json.dumps(data)
        # Send the JSON string back to the client who asked for the process names
        send_by_chunks(sock, data_str)

    def handle_upload_file(self, client_sockets):
        # Get the name of the file
        file_name = self.message["file_name"]
        # Get the data of the file
        file_data = self.message["file_data"]
        # Get the IP address of the client that sent the request
        ip_of_asking_client = self.address[0]
        # Get the ID of the client that will execute the command
        client_id = self.message['id']
        # Get the IP address of the client that will execute the command
        self.cur.execute("select ip from clients where id=?", [client_id, ])
        ip_client_running_the_command = self.cur.fetchone()[0]
        # Get the socket associated with the IP address of the client that will execute the command
        sock = client_sockets.get(ip_client_running_the_command)
        # Create the message that will be sent to the client that will execute the command
        data = {
            "command": self.message['command'],
            "file_name": file_name,
            "file_data": file_data,
            "asking_ip": ip_of_asking_client
        }
        data_str = json.dumps(data)
        # Send the message to the client that will execute the command
        send_by_chunks(sock, data_str)

    def handle_upload_file_result(self, out_put_sockets):
        # Get the client that asked for the process to be killed
        asking_client = self.message["addressed_client"]
        # Get the socket of the client that asked for the process to be killed
        sock = out_put_sockets[asking_client]
        # Create the response message with the killed process ID
        data = {
            "command": self.message["command"],
            "success": self.message["success"]
        }
        # Convert the message to a JSON string and send it to the client
        data_str = json.dumps(data)
        send_by_chunks(sock, data_str)

    def handle_delete_file(self, client_sockets):
        # Get the name of the file
        file_name = self.message["file_name"]
        # Get the IP address of the client that sent the request
        ip_of_asking_client = self.address[0]
        # Get the ID of the client that will execute the command
        client_id = self.message['id']
        # Get the IP address of the client that will execute the command
        self.cur.execute("select ip from clients where id=?", [client_id, ])
        ip_client_running_the_command = self.cur.fetchone()[0]
        # Get the socket associated with the IP address of the client that will execute the command
        sock = client_sockets.get(ip_client_running_the_command)
        # Create the message that will be sent to the client that will execute the command
        data = {
            "command": self.message['command'],
            "file_name": file_name,
            "asking_ip": ip_of_asking_client
        }
        data_str = json.dumps(data)
        # Send the message to the client that will execute the command
        send_by_chunks(sock, data_str)

    def handle_delete_file_result(self, out_put_sockets):
        # Get the client that asked for the process to be killed
        asking_client = self.message["addressed_client"]
        # Get the socket of the client that asked for the process to be killed
        sock = out_put_sockets[asking_client]
        # Create the response message with the killed process ID
        data = {
            "command": self.message["command"],
            "success": self.message["success"]
        }
        # Convert the message to a JSON string and send it to the client
        data_str = json.dumps(data)
        send_by_chunks(sock, data_str)

    def handle_get_screenshot(self, client_sockets):
        # Get the IP address of the client that sent the request
        ip_of_asking_client = self.address[0]
        # Get the ID of the client that will execute the command
        client_id = self.message['id']
        # Get the IP address of the client that will execute the command
        self.cur.execute("select ip from clients where id=?", [client_id, ])
        ip_client_running_the_command = self.cur.fetchone()[0]

        # Get the socket associated with the IP address of the client that will execute the command
        sock = client_sockets.get(ip_client_running_the_command)
        # Create the message that will be sent to the client that will execute the command
        data = {
            "command": self.message['command'],
            "asking_ip": ip_of_asking_client
        }
        data_str = json.dumps(data)
        # Send the message to the client that will execute the command
        send_by_chunks(sock, data_str)

    def handle_get_screenshot_result(self, out_put_sockets):
        # Get the data of the requested file,from the message
        screenshot_data = self.message["screenshot_data"]
        # Get the client who asked for the process names
        asking_client = self.message["addressed_client"]
        # Get the socket of the client who asked for the process names
        sock = out_put_sockets[asking_client]
        # Get the command that was executed to get the process names
        command = self.message["command"]
        # Create a dictionary containing the process names and the command executed
        data = {
            "out_put": screenshot_data,
            "command": command
        }
        # Convert the dictionary to a JSON string
        data_str = json.dumps(data)
        # Send the JSON string back to the client who asked for the process names
        send_by_chunks(sock, data_str)
