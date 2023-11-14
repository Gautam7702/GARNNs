using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;

public class CameraImageProcessor : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;

    private string ipAddr = "127.0.0.1";
    private int port = 12345;
    public int percentage;

    private void Start()
    {
        ConnectToServer(ipAddr, port);
    }

    void ConnectToServer(string ipAddress, int port)
    {
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            Debug.Log("Connected to server");
        }
        catch (Exception e)
        {
            Debug.Log("Error connecting to server: " + e.Message);
        }
    }

    public void ReceivePercentageCoroutine()
    {
            try
            {
                byte[] receiveBuffer = new byte[1024];

                // Receive data from the network stream
                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);

                // Convert the received bytes to a string
                string response = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                print(response);

                // Convert the string to an integer
                if (int.TryParse(response, out int receivedPercentage))
                {
                    percentage = receivedPercentage;
                }
                else
                {
                    Debug.LogWarning("Received data is not a valid integer: " + response);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving response: " + e.Message);
            }
    }
}
