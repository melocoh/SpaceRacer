using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class scrSeverControl : MonoBehaviour
{
    //public GameObject goCarControl;
    WebSocket websocket;

    private const int maxPlayers = 4;
    private int playerCount;

    public GameObject goCar0;
    public GameObject goCar1;
    public GameObject goCar2;
    public GameObject goCar3;
    int carClicks0;
    int carClicks1;
    int carClicks2;
    int carClicks3;

    private int yourCarNumber;


    // Update is called once per frame
    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
        #endif

        if (Input.GetMouseButtonDown(0))
        {
            carClicks0++;
            UpdateCar(yourCarNumber, carClicks0);
            updateOthersToMyCar((byte)carClicks0);
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
    //public void Start()
    //{
    //    yourCarNumber = 0;
    //    carClicks0 = 0;
    //    carClicks1 = 0;
    //    carClicks2 = 0;
    //    carClicks3 = 0;

    //    playerCount = 1;
    //    //send socket
        
    //    CreateNewListenningThread();
    //    Debug.Log("SERVER ONLINE");
    //}


    // Start is called before the first frame update
    async void Start()
    {
        yourCarNumber = 0;
        carClicks0 = 0;
        carClicks1 = 0;
        carClicks2 = 0;
        carClicks3 = 0;

        playerCount = 1;

        websocket = new WebSocket("ws://localhost:8080");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
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
        };

        // Keep sending messages at every 0.3s
        InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        await websocket.Connect();
    }





    public void CreateNewListenningThread()
    {
        Thread thread = new Thread(new ThreadStart(ListenningThread));
        thread.Start();
    }



    public void ListenningThread()
    {
        //recieve socket
        Socket recv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint recvipep = new IPEndPoint(IPAddress.Any, 4560);
        recv.Bind(recvipep);
        IPAddress ip = IPAddress.Parse("224.5.6.0");
        recv.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

        byte[] b = new byte[1024];
        while (true)
        {
            int size = recv.Receive(b);

            Debug.Log("[recieved] size: " + size + " key: " + b[0].ToString());

            HandleMessage(b);
        }
        
        recv.Close();
        CreateNewListenningThread();
        


        //send.Send(sendBytes, sendBytes.Length, SocketFlags.None);
        //send.Close();
    }


    public void HandleMessage(byte[] mes)
    {
        if (mes.Length == 0)
            return;

        byte key = mes[0];


        switch (key)
        {
            case 1:
                //new player
                if (playerCount < maxPlayers)
                {
                    byte[] sMes = new byte[2];
                    sMes[0] = 2;
                    sMes[1] = (byte)playerCount;
                    playerCount++;

                    SendMessage(sMes, 1);
                    Debug.Log("MESSAGE SENT!!");
                }

                break;
            case 2:
                // code block
                break;
            case 3:
                byte playerID = mes[1];
                byte playerClicks = mes[2];
                Debug.Log("server-receieved: car update " + playerID.ToString() + ":" + playerClicks.ToString());
                UpdateCar(playerID, playerClicks);

                //update the other clients
                byte[] copyMessage = new byte[3];
                copyMessage[0] = mes[0];
                copyMessage[1] = mes[1];
                copyMessage[2] = mes[2];
                if (playerID != 1)
                    SendMessage(copyMessage, 2);
                if (playerID != 2)
                    SendMessage(copyMessage, 3);
                if (playerID != 3)
                    SendMessage(copyMessage, 4);


                break;
            default:
                // code block
                break;
        }

    }



    //public void SendMessageThreaded(byte[] bSend)
    //{
    //    Thread thread = new Thread(new ParameterizedThreadStart(SendMessage));
    //    thread.Start(bSend);
    //}

    //public void SendMessage(byte[] bSend, int lastDigit)
    //{
    //    ////send socket
    //    Socket send = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    //    IPAddress sendip = IPAddress.Parse("224.5.6."+ lastDigit);
    //    send.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(sendip));
    //    send.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
    //    IPEndPoint sendipep = new IPEndPoint(sendip, 4560+ lastDigit);
    //    send.Connect(sendipep);

    //    send.Send(bSend, bSend.Length, SocketFlags.None);
    //    send.Close();

    //    if(bSend.Length==1)
    //    Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString());
    //    else if (bSend.Length == 2)
    //        Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString() + " : " + ((int)bSend[1]).ToString());
    //    else if (bSend.Length == 3)
    //        Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString() + " : " + ((int)bSend[1]).ToString() + " : " + ((int)bSend[2]).ToString());
    //    else
    //        Debug.Log("[sent] size: " + bSend.Length + " key: " + ((int)bSend[0]).ToString());
    //}


    async void SendMessage(byte[] bSend)
    {
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

    public void updateOthersToMyCar(byte clicks)
    {
        byte[] mes = new byte[3];
        mes[0] = 3;
        mes[1] = 0;
        mes[2] = clicks;

        SendMessage(mes, 2);
        SendMessage(mes, 3);
        SendMessage(mes, 4);
        //s.SendTo(sendMessage, sendMessage.Length, SocketFlags.None, new IPEndPoint(IPAddress.Parse("224.5.6.7"), 4567));

    }



}
