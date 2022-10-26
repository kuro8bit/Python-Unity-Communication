using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;

public static class UDPSocket
{
    static UdpClient socket;
    static IPEndPoint remoteEndPoint;

    public static void Connect(string IP, int sendPort, int recvPort)
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), sendPort);
        socket = new UdpClient(recvPort);
    }

    static void SendBytes(byte[] bytes)
    {
        try
        {
            int byteCount = socket.Send(bytes, bytes.Length, remoteEndPoint);
            Debug.Log("Send " + byteCount + " bytes");
        }
        catch (Exception err)
        {
            Debug.LogError(err.ToString());
        }
    }


    static byte[] RecvBytes()
    {
        try
        {
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
            byte[] recv_bytes = socket.Receive(ref anyIP);
            Debug.Log("Received " + recv_bytes.Length + " bytes");
            return recv_bytes;
        }
        catch (Exception err)
        {
            Debug.LogError(err.ToString());
            return null;
        }
    }

    public static void SendFloat(float[] arrayFloat)
    {
        byte[] bytes = new byte[arrayFloat.Length * sizeof(float)];
        Buffer.BlockCopy(arrayFloat, 0, bytes, 0, bytes.Length);
        SendBytes(bytes);
    }

    public static float[] RecvFloat()
    {
        byte[] bytes = RecvBytes();

        if(bytes != null)
        {
            float[] arrayFloat = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, arrayFloat, 0, bytes.Length);

            return arrayFloat;
        }
        return null;
    }

    public static void Close()
    {
        socket.Close();
    }
}
