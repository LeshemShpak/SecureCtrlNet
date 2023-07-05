import client


class User:
    def __init__(self, id):
        self.id = id
        self.clients = {}

    def add_client(self, client_id, client_ip):
        self.clients[client_id] = client.Client(client_ip)
