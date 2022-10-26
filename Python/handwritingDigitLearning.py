import keras
from keras.datasets import mnist
from keras.models import Sequential
from keras.layers import Dense, Dropout
import matplotlib.pyplot as plt

# get MNIST dataset
(x_train, y_train), (x_test, y_test)  = mnist.load_data()

# reshape dataset with GPU feature
rows, cols = x_train.shape[1:]
num_classes = 10 # 0,1,...9
epochs = 15
batch_size = 128

x_train = x_train.reshape(x_train.shape[0], rows * cols)
x_test = x_test.reshape(x_test.shape[0], rows * cols)
x_train = x_train.astype('float32')
x_test = x_test.astype('float32')
x_train /= 255 # set pixels from [0,255] to [0,1]
x_test /= 255 # set pixels from [0,255] to [0,1]

y_train = keras.utils.to_categorical(y_train, num_classes)
y_test = keras.utils.to_categorical(y_test, num_classes)

# create model
model = Sequential()
model.add(Dense(512, activation='relu', input_shape=(784,)))
model.add(Dropout(0.2))
model.add(Dense(512, activation='relu'))
model.add(Dropout(0.2))
model.add(Dense(num_classes, activation='softmax'))
model.compile(loss=keras.losses.categorical_crossentropy, optimizer=keras.optimizers.RMSprop(), metrics=['accuracy'])

model.summary()

# learning
history = model.fit(x_train, y_train, batch_size=batch_size, epochs=epochs, verbose=1, validation_data=(x_test, y_test))

score = model.evaluate(x_test, y_test, verbose=0)
print(f'Test loss: {score[0]} | Test accuracy: {score[1]}')

fig = plt.figure(figsize=(10, 5))
a = fig.add_subplot(1,2,1)
plt.plot(history.history['loss'])
plt.plot(history.history['val_loss'])
a.set_title('model loss')
plt.ylabel('loss')
plt.xlabel('epoch')
plt.legend(['Train loss', 'Validation loss'], loc='upper left')
a = fig.add_subplot(1,2,2)
plt.plot(history.history['acc'])
plt.plot(history.history['val_acc'])
a.set_title('model accuracy')
plt.ylabel('accuracy')
plt.xlabel('epoch')
plt.legend(['Train accuracy', 'Validation accuracy'], loc='upper left')
plt.show()

# save model
model.save('handwriting.h5')
