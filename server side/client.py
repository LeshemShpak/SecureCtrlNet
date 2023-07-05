class Client:
    def __init__(self, ip):
        self.cpu = 0
        self.ram = 0
        self.ip = ip
        self.last_update = "2000-01-01T12:30:00.000000"
        self.online = False
        self.manager = ""
        self.detector_1 = 0
        self.detector_2 = 0

