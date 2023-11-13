using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class CameraImageProcessor
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

    private string ipAddr = "127.0.0.1";
    private int port = 12345;
    public CameraImageProcessor(){
        ConnectToServer(ipAddr,port);
        
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

    public string sendImagePath(string imagePath)
        {
            try
            {
                // Convert the string to bytes
                byte[] data = Encoding.UTF8.GetBytes(imagePath);

                // Send the data through the network stream
                stream.Write(data, 0, data.Length);

                Debug.Log("String sent: " + imagePath);
                return ReceiveResponse();
            }
            catch (Exception e)
            {
                Debug.LogError("Error sending string: " + e.Message);
                return "0";
            }
        }
    private string ReceiveResponse(){
            try
            {
                // Clear the receive buffer
                Array.Clear(receiveBuffer, 0, receiveBuffer.Length);

                // Receive data from the network stream
                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);

                // Convert the received bytes to a string
                string response = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);

                return response;
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving response: " + e.Message);
                return null;
            }
        }
}
