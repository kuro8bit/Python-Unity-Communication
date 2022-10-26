using UnityEngine;
using System;
using System.Threading;
using System.Collections;

public class Client : MonoBehaviour
{
    [SerializeField] private string IP = "127.0.0.1";
    [SerializeField] private int recvPort = 8000;
    [SerializeField] private int sendPort = 8001;

    private Thread recvThread;

    public static float[] sendData = new float[784];
    public static float[] recvData;

    private void Awake()
    {
        UDPSocket.Connect(IP, sendPort, recvPort);

        recvThread = new Thread(new ThreadStart(ReceiveData));
        recvThread.IsBackground = true;
        recvThread.Start();

        StartCoroutine(SendDataCoroutine());
    }

    IEnumerator SendDataCoroutine()
    {
        while (true)
        {
            SendData();
            yield return new WaitForSeconds(1f);
        }
    }

    void SendData()
    {
        UDPSocket.SendFloat(sendData);
    }

    private void ReceiveData()
    {
        while (true)
        {
            float[] arrayFloat = UDPSocket.RecvFloat();

            if (arrayFloat != null)
            {
                recvData = arrayFloat;
            }
        }
    }

    private void OnDisable()
    {
        if (recvThread != null) recvThread.Abort();
        UDPSocket.Close();
    }
}