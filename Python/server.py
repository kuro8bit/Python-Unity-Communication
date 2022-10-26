import time
import numpy as np
import keras
import pythonUDPSocket

ip = "127.0.0.1"
send_port = 8000
recv_port = 8001

sock = UdpServer(ip, send_port, recv_port)
model = keras.models.load_model('handwriting.h5')

while True:
    arrayFloat = sock.ReadReceivedFloatData(784)

    if(arrayFloat != None):
        # convert tuple to np array of float and reshape
        arrayFloat = np.array(arrayFloat, dtype=np.float32)
        arrayFloat = arrayFloat.reshape((1, 784))

        # prediction
        prediction = model.predict(arrayFloat)

        # send data (append index of highest predition. Unity will display in red)
        prediction = np.append(prediction[0], [np.argmax(prediction[0])])
        sock.SendFloat(prediction)
    time.sleep(1)
sock.CloseSocket()
