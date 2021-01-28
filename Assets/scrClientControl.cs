using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

using NativeWebSocket;

public class scrClientControl : MonoBehaviour
{
    //public GameObject goCarControl;

    //UdpClient udpClient;
    //IPAddress multicastIPaddress;
    //IPAddress localIPaddress;
    //IPEndPoint localEndPoint;
    //IPEndPoint remoteEndPoint;
    //private byte playerNumber;

    WebSocket websocket;


    public GameObject goCar0;
    public GameObject goCar1;
    public GameObject goCar2;
    public GameObject goCar3;
    public GameObject winText;
    int carClicks0;
    int carClicks1;
    int carClicks2;
    int carClicks3;
    bool gameover = false;
    //public GameObject goClientControl;


    private int yourCarNumber;


    public void UpdateCarNumber(int carNum)
    {
        Debug.Log("UpdateCarNumber");
        if (yourCarNumber == -1)
        {
            yourCarNumber = carNum;
            CreateNewListenningThread(yourCarNumber);
        }


    }

    //// Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif

        if (!gameover && Input.GetKeyDown("space"))
        {
            int currentDistance = 0;

            if (yourCarNumber == 0)
            {
                carClicks0++;
                currentDistance = carClicks0;
            }
            else if (yourCarNumber == 1)
            {
                carClicks1++;
                currentDistance = carClicks1;
            }
            else if (yourCarNumber == 2)
            {
                carClicks2++;
                currentDistance = carClicks2;
            }
            else if (yourCarNumber == 3)
            {
                carClicks3++;
                currentDistance = carClicks3;
            }

            if(currentDistance > 65)
            {
                winText.SetActive(true);
                gameover = true;
                Time.timeScale = 0;
            }

            UpdateCar(yourCarNumber, currentDistance);
            updateOthersToMyCar((byte)currentDistance);
        }


        goCar0.transform.position = new Vector3(-7f + 0.2f * carClicks0, 2.27f - 1.48f * 0, 0f);
        goCar1.transform.position = new Vector3(-7f + 0.2f * carClicks1, 2.27f - 1.48f * 1, 0f);
        goCar2.transform.position = new Vector3(-7f + 0.2f * carClicks2, 2.27f - 1.48f * 2, 0f);
        goCar3.transform.position = new Vector3(-7f + 0.2f * carClicks3, 2.27f - 1.48f * 3, 0f);
    }

    public void UpdateCar(int num, int clicks)
    {
        if (num == 0)
            carClicks0 = clicks;
        else if (num == 1)
            carClicks1 = clicks;
        else if (num == 2)
            carClicks2 = clicks;
        else if (num == 3)
            carClicks3 = clicks;
        else
            return;

        //moveCar.transform.position = new Vector3(-7f + 0.2f * clicks, 2.27f - 1.48f * num, 0f);
        //updateOthersToMyCar((byte)clicks);
    }








    // Start is called before the first frame update
    public async void Start()
    {
        yourCarNumber = -1;
        carClicks0 = 0;
        carClicks1 = 0;
        carClicks2 = 0;
        carClicks3 = 0;



        // Store params

        //CreateConnectionListenningThread();

        // websocket = new WebSocket("ws://localhost:5000");

        websocket = new WebSocket("wss://space-racerz.herokuapp.com/");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            Debug.Log("CLIENT ONLINE");
            byte[] sendBytes = new byte[1];
            sendBytes[0] = 1;
            SendMessage(sendBytes);
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
            HandleMessage(bytes);
        };

        // Keep sending messages at every 0.3s
        // InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        await websocket.Connect();

    }



    public void CreateConnectionListenningThread()
    {
        Thread thread = new Thread(new ThreadStart(ConnectionListenningThread));
        thread.Start();
    }


    public void ConnectionListenningThread()
    {
        //recieve socket
        Socket recv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint recvipep = new IPEndPoint(IPAddress.Any, 4561);
        recv.Bind(recvipep);
        IPAddress ip = IPAddress.Parse("224.5.6.1");
        recv.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));


        byte[] b = new byte[1024];

        int size = recv.Receive(b);
        recv.Close();

        Debug.Log("[recieved] size: " + size + " key: " + b[0].ToString());

        HandleMessage(b);


    }




    public void CreateNewListenningThread(int playerNumber)
    {
        Debug.Log("CreateNewListenningThread " + playerNumber);
        Thread thread;

        if (playerNumber == 1)
            thread = new Thread(new ThreadStart(Client1));
        else if (playerNumber == 2)
            thread = new Thread(new ThreadStart(Client2));
        else if (playerNumber == 3)
            thread = new Thread(new ThreadStart(Client3));
        else
            return;


        thread.Start();
    }


    public void Client1()
    {
        ListenningThread(2);
    }

    public void Client2()
    {
        ListenningThread(3);
    }

    public void Client3()
    {
        ListenningThread(4);
    }


    public void ListenningThread(int lastDigit)
    {
        Debug.Log(lastDigit.ToString());
        //recieve socket
        Socket recv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint recvipep = new IPEndPoint(IPAddress.Any, 4560 + lastDigit);
        recv.Bind(recvipep);
        IPAddress ip = IPAddress.Parse("224.5.6." + lastDigit);
        recv.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));


        byte[] b = new byte[1024];
        while (true)
        {
            int size = recv.Receive(b);

            Debug.Log("[recieved] size: " + size + " key: " + b[0].ToString());

            HandleMessage(b);
        }

        HandleMessage(b);
    }




    public void HandleMessage(byte[] mes)
    {
        if (mes.Length == 0)
            return;

        byte key = mes[0];


        switch (key)
        {
            case 1:

                break;
            case 2:
                //recieved their player id
                byte newPlayerID = mes[1];
                UpdateCarNumber(newPlayerID);

                break;
            case 3:
                byte playerID = mes[1];
                byte playerClicks = mes[2];
                Debug.Log("client-receieved: car update " + playerID.ToString() + ":" + playerClicks.ToString());
                UpdateCar(playerID, playerClicks);
                break;
            default:
                // code block
                break;
        }

    }



    public async void SendMessage(byte[] bSend)
    {
        ////send socket
        //Socket send = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //IPAddress sendip = IPAddress.Parse("224.5.6.0");
        //send.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(sendip));
        //send.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
        //IPEndPoint sendipep = new IPEndPoint(sendip, 4560);
        //send.Connect(sendipep);

        //send.Send(bSend, bSend.Length, SocketFlags.None);
        //send.Close();

        //Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString());

        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(bSend);

            if (bSend.Length == 1)
                Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString());
            else if (bSend.Length == 2)
                Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString() + " : " + ((int)bSend[1]).ToString());
            else if (bSend.Length == 3)
                Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString() + " : " + ((int)bSend[1]).ToString() + " : " + ((int)bSend[2]).ToString());
            else
                Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString());
        }
    }


    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }


    ///// Callback which is called when UDP packet is received
    ///// </summary>
    ///// <param name="ar"></param>
    //private void ReceivedCallback(IAsyncResult ar)
    //{
    //    // Restart listening for udp data packages
    //    udpClient.BeginReceive(new AsyncCallback(ReceivedCallback), null);

    //    // Get received data
    //    IPEndPoint sender = new IPEndPoint(0, 0);
    //    Byte[] receivedBytes = udpClient.EndReceive(ar, ref sender);
    //    Debug.Log("REC " + receivedBytes.Length + " " + receivedBytes[0]);
    //    if (receivedBytes.Length > 0)
    //    {
    //        byte key = receivedBytes[0];

    //        Debug.Log("REC");
    //        switch (key)
    //        {
    //            case 1:

    //                break;
    //            //we are allowed to play
    //            case 2:
    //                Debug.Log("hi");
    //                byte newPlayerID = receivedBytes[1];
    //                playerNumber = newPlayerID;
    //                UpdateCarNumber(newPlayerID);
    //                Debug.Log("client-receieved: their player number " + newPlayerID.ToString());

    //                break;
    //            //update a car position
    //            case 3:
    //                byte playerID = receivedBytes[1];
    //                byte playerClicks = receivedBytes[2];
    //                Debug.Log("client-receieved: car update " + playerID.ToString() + ":" + playerClicks.ToString());
    //                UpdateCar(playerID, playerClicks);
    //                break;
    //            default:

    //                break;
    //        }

    //    }


    //}



    public void updateOthersToMyCar(byte clicks)
    {
        byte[] mes = new byte[3];
        mes[0] = 3;
        mes[1] = (byte)yourCarNumber;
        mes[2] = clicks;

        SendMessage(mes);
        //s.SendTo(sendMessage, sendMessage.Length, SocketFlags.None, new IPEndPoint(IPAddress.Parse("224.5.6.7"), 4567));
        Debug.Log("client-sent: update my car to others " + mes[1] + ":" + mes[2]);
    }

}
