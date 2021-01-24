using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class scrTestClient : MonoBehaviour
{
    Socket s;
    // Start is called before the first frame update
    void Start()
    {
        s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
        s.Bind(ipep);
        IPAddress ip = IPAddress.Parse("224.5.6.7");

        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

        MakeANewRecieveThread();

        //while()
        //Console.ReadLine();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakeANewRecieveThread()
    {
        Thread thread = new Thread(new ThreadStart(ThreadRecieveUDP));
        thread.Start();
    }

    void ThreadRecieveUDP()
    {
        byte[] b = new byte[1024];

        s.Receive(b);
        string str = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);
        Debug.Log(str.Trim());

        MakeANewRecieveThread();
    }
}
