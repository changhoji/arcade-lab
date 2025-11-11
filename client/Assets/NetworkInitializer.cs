using SocketIOClient;
using UnityEngine;

public class NetworkInit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var socket = new SocketIO("http://localhost:3000");

        socket.ConnectAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
