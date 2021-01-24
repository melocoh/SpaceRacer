using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class scrTestServer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        IPAddress ip = IPAddress.Parse("224.5.6.7");

        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));

        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);

        IPEndPoint ipep = new IPEndPoint(ip, 4567);
        s.Connect(ipep);

        // This creates the letters ABCDEFGHIJ
        byte[] b = new byte[10];
        for (int x = 0; x < b.Length; x++) b[x] = (byte)(x + 65);

        s.Send(b, b.Length, SocketFlags.None);

        s.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
