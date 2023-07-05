import threading
import socket
import sqlite3
import json
import ssl
import struct
import time
from user import User
import datetime
from handler import Handler

PORT = 6060
PORT_1 = 6066
PORT_2 = 6666
PORT_3 = 6006

IP = "0.0.0.0"
INT_BYTE_SIZE = 4
NAME_OF_SQL_DATABASE = "my_database.db"

users_info = {}
client_sockets = {}
out_put_sockets = {}

# punish_ips ----  {    [ip : [time,1]],    [ip : []],  [ip : [time,time,2]],   [ip : [time,time,time,3]]   }
punish_ips = {}
band_ips = {}


# main function
def main():
    # create locks for the ddos dictionaries, for
    lock_punish = threading.Lock()
    lock_band = threading.Lock()
    # load data base
    load_db_info()
    # observe band client list
    ddos_observer(lock_band, lock_punish)
    try:
        # Open three encrypted sockets
        encrypted_socked, encrypted_socked1, encrypted_socked2 = open_sockets()

        # Register client sockets using threads
        # Register client socket to client_sockets
        t1 = threading.Thread(target=register_client_socket, args=(encrypted_socked1, client_sockets, lock_band))
        t1.start()

        # Register client socket to out_put_sockets
        t2 = threading.Thread(target=register_client_socket, args=(encrypted_socked2, out_put_sockets, lock_band))
        t2.start()

        # Continue to accept incoming client connections and handle them with threads
        while True:
            # Accept incoming client connections
            client_sock, address = encrypted_socked.accept()
            with lock_band:
                ans = ip_is_not_banned(address[0])
            # Check if the client's IP address is not banned
            if ans:
                # Handle client requests using threads
                t3 = threading.Thread(target=handle_client, args=(client_sock, address, lock_band, lock_punish))
                t3.start()
            else:
                # Close the client connection if the IP address is banned
                client_sock.close()

        # Catch socket and connection errors
    except (ConnectionRefusedError, ConnectionResetError, socket.error):
        print("Failed to open communication, socket error, can't run the program")
        return


# This function creates SSL certificates and opens three sockets
def open_sockets():
    # create a certificate for wrapped SSL communication
    context = ssl.create_default_context(ssl.Purpose.CLIENT_AUTH)
    context.load_cert_chain(certfile='server.crt', keyfile='server.key')

    # Open the first socket and wrap it with SSL
    server_sock_1 = socket.socket()
    server_sock_1.bind((IP, PORT_1))
    server_sock_1.listen()
    encrypted_socked1 = context.wrap_socket(server_sock_1, server_side=True)

    # Open the second socket and wrap it with SSL
    server_sock_2 = socket.socket()
    server_sock_2.bind((IP, PORT_2))
    server_sock_2.listen()
    encrypted_socked2 = context.wrap_socket(server_sock_2, server_side=True)


    # Open the main socket and wrap it with SSL
    server_sock = socket.socket()
    server_sock.bind((IP, PORT))
    server_sock.listen()
    encrypted_socked = context.wrap_socket(server_sock, server_side=True)

    print("The server successfully opened all three sockets and now ready for use")
    # Return the three encrypted sockets
    return encrypted_socked, encrypted_socked1, encrypted_socked2


# This function registers client sockets and stores them in a dictionary
def register_client_socket(server_socket, dick, lock_band):
    while True:
        # accept incoming client socket connections
        client_sock, address = server_socket.accept()
        # check if the client IP address is not banned
        with lock_band:
            ans = ip_is_not_banned(address[0])
        if ans:
            # store the client socket in the dictionary
            dick[address[0]] = client_sock
            print(dick)
        else:
            # close the client socket if its IP is banned
            client_sock.close()


# This function loads information from a SQLite database and stores it in a dictionary
def load_db_info():
    # connect to the database and retrieve the desired information
    conn = sqlite3.connect(NAME_OF_SQL_DATABASE)
    cur = conn.cursor()
    cur.execute(
        "SELECT USERS.id, Clients.id, Clients.ip FROM Clients INNER JOIN USERS on Users.id = Clients.user")
    info = cur.fetchall()
    print(info)
    # iterate over the information and store it in a dictionary
    for inf in info:
        # check if the user is already in the dictionary
        if inf[0] in users_info:
            user = users_info.get(inf[0])
        else:
            # create a new user object if the user is not in the dictionary
            user = User(inf[0])
            users_info[inf[0]] = user

        # add the client to the user's client list
        user.add_client(inf[1], inf[2])

    print("Successfully loaded data-base information")



# This function receives data from a socket connection in chunks and decodes it
def recv_by_chunks(conn):
    try:
        # receive the message length in bytes
        msg_length = conn.recv(INT_BYTE_SIZE)
    except Exception as e:
        return None

    if not msg_length:
        return

    # convert the message length bytes to an integer
    msg_length_int = struct.unpack('I', msg_length)[0]

    # receive the message in chunks until the full message is received
    msg = b''
    while len(msg) < msg_length_int:
        chunk = conn.recv(msg_length_int - len(msg))
        if not chunk:
            break
        msg += chunk

    # decode the message from bytes to utf-8
    decoded_msg = msg.decode('utf-8')
    return decoded_msg


# This function handles communication with a single client connection
def handle_client(client_sock, address,  lock_band, lock_punish):
    # connect to the SQLite database
    conn = sqlite3.connect(NAME_OF_SQL_DATABASE)
    cur = conn.cursor()

    while True:
        # receive a message from the client
        message_str = recv_by_chunks(client_sock)
        if not message_str:
            with lock_punish:
                punish_ip(address[0], client_sock, lock_band)
            # the last message is empty also,
            # the client can send empty message only at the beginning of the communication and in the end
            # otherwise he will be banned
            break

        # parse the message as a JSON object
        message = json.loads(message_str)

        try:
            # extract the command from the message
            command = message['command']
        except Exception as e:
            # if the message is not formatted correctly, punish the client
            with lock_band:
                client_into_banned(address[0], lock_punish, client_sock)

        # handle the command with the appropriate function
        handle_all_commands(command, message, address, cur, conn, client_sock, lock_band, lock_punish)


# This function handles all incoming commands from clients
def handle_all_commands(command, message, address, cur, conn, client_sock, lock_band, lock_punish):
    # Create a new handler object to handle the command
    handler_1 = Handler(command, message, address, cur, conn, client_sock)
    # Check the command and call the appropriate handler method
    if command == "Reg_user":
        handler_1.handle_reg_user(users_info)
    elif command == "Log_in":
        handler_1.handle_log_in(users_info)
    elif command == "Rec_information":
        handler_1.handle_rec_information(users_info)
    elif command == "Run_command":
        handler_1.handle_run_command(client_sockets)
    elif command == "Run_command_result":
        handler_1.handle_run_command_result(out_put_sockets)
    elif command == "Get_all_processes":
        handler_1.handle_get_all_processes(client_sockets)
    elif command == "Get_all_processes_result":
        handler_1.handle_get_all_processes_result(out_put_sockets)
    elif command == "Kill_process":
        handler_1.handle_kill_process(client_sockets)
    elif command == "Kill_process_result":
        handler_1.handle_kill_process_result(out_put_sockets)
    elif command == "Get_all_files_name":
        handler_1.handle_get_all_files_name(client_sockets)
    elif command == "Get_all_files_names_result":
        handler_1.handle_get_all_files_name_result(out_put_sockets)
    elif command == "Get_file":
        handler_1.handle_get_file(client_sockets)
    elif command == "Get_file_result":
        handler_1.handle_get_file_result(out_put_sockets)
    elif command == "Upload_file":
        handler_1.handle_upload_file(client_sockets)
    elif command == "Upload_file_result":
        handler_1.handle_upload_file_result(out_put_sockets)
    elif command == "Delete_file":
        handler_1.handle_delete_file(client_sockets)
    elif command == "Delete_file_result":
        handler_1.handle_delete_file_result(out_put_sockets)
    elif command == "Change_directory":
        handler_1.handle_change_directory()
    elif command == "Change_directory_result":
        handler_1.handle_change_directory_result()
    elif command == "Update_for_file_being_moved_or_deleted":
        handler_1.handle_update_for_file_being_moved_or_deleted()
    elif command == "Update_for_file_being_moved_or_deleted_result":
        handler_1.handle_update_for_file_being_moved_or_deleted_result()
    elif command == "Get_screenshot":
        handler_1.handle_get_screenshot(client_sockets)
    elif command == "Get_screenshot_result":
        handler_1.handle_get_screenshot_result(out_put_sockets)
    elif command == "":
        with lock_band:
            client_into_banned(address[0], lock_punish, client_sock)


def client_into_banned(ip, lock_punish, client_sock):
    now = datetime.datetime.now()
    band_ips[ip] = now
    with lock_punish:
        if ip in punish_ips:
            del punish_ips[ip]
    print(address[0] + " is now band from use, closing all connections with that client for 24 hours")
    client_sock.close()
    close_all_sockets_of_given_ip(ip)

def ip_is_not_banned(ip):
    """
    Check if an IP address is not currently banned.

    Args:
        ip (str): The IP address to check.

    Returns:
        bool: True if the IP address is not banned, False otherwise.
    """
    if ip in band_ips:
        return False
    else:
        return True


def ddos_observer(lock_band, lock_punish):
    # This function sets up a thread to continuously monitor and update a list of banned IPs
    # The list of banned IPs is created and updated by the function "go_over_band_ip_list"
    # create a new thread that runs the function "go_over_band_ip_list" with the lock as an argument
    t1 = threading.Thread(target=go_over_band_ip_list, args=(lock_band,))
    # start the thread
    t1.start()

    t2 = threading.Thread(target=go_over_punish_ip_list, args=(lock_punish,))
    t2.start()


# this function handle clients who are suspected of taking part in a ddos attack
def punish_ip(ip, client_sock, lock_band):
    # print a message to indicate that punishment is being applied to the given IP address
    print()
    print("punishment for:  " + ip)
    print()
    # if the IP is already on the punishment list, increment the count of how many times they've attempted sabotage
    if ip in punish_ips:
        how_many_times_tried_to_sabotage = punish_ips[ip]
        # if this is the third time, ban the IP from server access for 6 hours
        now = datetime.datetime.now()
        if how_many_times_tried_to_sabotage == 5:
            with lock_band:
                band_ips[ip] = now
            print(ip + " is now band from use, closing all connections with that client for 24 hours")
            del punish_ips[ip]
            client_sock.close()
            close_all_sockets_of_given_ip(ip)
        else:
            punish_ips[ip] += 1
    else:
        # if this is the first time this IP has been caught trying to sabotage the server,
        # add them to the punishment list with a count of 1
        punish_ips[ip] = 1


# this function closes all connections with the given ip
def close_all_sockets_of_given_ip(ip):
    try:
        # Get the output socket associated with the given IP address
        sock = out_put_sockets[ip]
        # Close the output socket
        sock.close()
        # Get the client socket associated with the given IP address
        sock = client_sockets[ip]
        # Close the client socket
        sock.close()
    except (KeyError,):
        pass
        # the client closed the program first


# This function continuously checks the list of punished IPs and updates the
# punishment duration for each one every second.
def go_over_punish_ip_list(lock_punish):
    while True:
        # Wait for 1 second before checking again
        time.sleep(1)
        # Lock the punishment dictionary to avoid concurrent modification
        with lock_punish:
            try:
                # Iterate over each IP in the punishment dictionary
                for ip in list(punish_ips.keys()):
                    # If the remaining punishment duration is 1 second, remove the IP from the punishment dictionary
                    if punish_ips[ip] == 2:
                        del punish_ips[ip]
                    # Otherwise, subtract 2 from the remaining punishment duration
                    else:
                        punish_ips[ip] = punish_ips[ip] - 1
            except RuntimeError:
                print("the size of the punish_ips changed")


# This function is used to iterate over the band_ips dictionary, which stores the IPs that have been banned
# by the server. The function runs in a loop and checks the length of the dictionary. If the dictionary is too long
# (more than 2000 items), it calls the clean_100_from_dictionary() function to remove 100 items that have been banned
# for the longest time.
def go_over_band_ip_list(lock_band):
    while True:
        time.sleep(5)
        with lock_band:
            # handle the length of the dictionary
            # if the dictionary is too long, remove 100 items that have been banned for the longest time
            if len(band_ips) >= 2000:
                clean_100_from_dictionary()
            else:
                # if the dictionary is not too long, check each banned IP
                for key, value in list(band_ips.items()):
                    # calculate the time difference between the current time and the time the IP was banned
                    current_time = datetime.datetime.now()
                    time_difference = current_time - value
                    # if the IP has been banned for more than 24 hours, remove it from the dictionary
                    if time_difference.total_seconds() >= 24 * 60 * 60:
                        print(str(key) + " can connect again")
                        del band_ips[key]


def clean_100_from_dictionary():
    print("this sever is under havey attack, must clean 100 ips from the band list")
    # Sort the dictionary items by their values (which are datetime objects)
    sorted_items = sorted(band_ips.items(), key=lambda x: x[1], reverse=True)
    # Select the top 100 items with the latest timestamps and create a new dictionary
    top_100 = dict(sorted_items[:100])
    # Delete the selected items from the original dictionary
    for key in list(top_100.keys()):
        del band_ips[key]




main()
