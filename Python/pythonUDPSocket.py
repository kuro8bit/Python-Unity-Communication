import socket
import threading
import struct

class UdpServer():
    def __init__(self, ip="127.0.0.1", send_port=8000, recv_port=8001):
        self.ip = ip
        self.send_port = send_port
        self.recv_port = recv_port
        self.isDataReceived = False
        self.recv_bytes = None

        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.sock.bind((ip, recv_port))

        self.recvThread = threading.Thread(target=self.ReadThreadFunc, daemon=True)
        self.recvThread.start()

    def __del__(self):
        self.CloseSocket()

    def CloseSocket(self):
        self.sock.close()

    def SendBytes(self, arrayBytes):
        self.sock.sendto(arrayBytes, (self.ip, self.send_port))
        print(f'Send {len(arrayBytes)} bytes')

    def SendFloat(self, arrayFloat):
        arrayBytes = struct.pack(f'<{len(arrayFloat)}f', *arrayFloat)
        self.SendBytes(arrayBytes)

    def RecvBytes(self):
        try:
            arrayBytes, _ = self.sock.recvfrom(12544)
            print(f'Received {len(arrayBytes)} bytes')
        except socket.error as e:
            print(e)
        return arrayBytes

    def ReadThreadFunc(self):
        while True:
            arrayBytes = self.RecvBytes()
            if(len(arrayBytes) != 0):
                self.recv_bytes = arrayBytes
                self.isDataReceived = True

    def ReadReceivedFloatData(self, arrayLen):
        data = None
        if self.isDataReceived:
            self.isDataReceived = False
            data = struct.unpack(f'<{arrayLen}f', self.recv_bytes)
            self.recv_bytes = None
        return data
