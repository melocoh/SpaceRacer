using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;

namespace websocketchat
{
    public class TestWebSocketHandler : WebSocketHandler
    {
        private static WebSocketCollection clients = new WebSocketCollection();

        private static int playerCount = 0;
        private static int maxPlayers = 4;

        public override void OnOpen()

        {
            clients.Add(this);
        }

        public override void OnMessage(byte[] message)

        {
            HandleMessage(message);
        }

        public override void OnClose()

        {
            clients.Remove(this);
            
            if(clients.Count == 0)
            {
                playerCount = 0;
            }
        }

        /* Handles socket messages */
        public void HandleMessage(byte[] mesArray)
        {
            if (mesArray.Length == 0)
            {
                return;
            }

            byte key = mesArray[0];

            switch (key)
            {
                case 1:
                    //new player
                    if (playerCount < maxPlayers)
                    {
                        Byte[] byteArray = new Byte[2];
                        byteArray[0] = 2;
                        byteArray[1] = (Byte)playerCount;
                        playerCount++;

                        // sends player position
                        clients.Broadcast(byteArray);
                    }
                    break;
                case 2:
                    // code block
                    break;
                case 3:
                    // onClick
                    byte playerID = mesArray[1];
                    byte playerClicks = mesArray[2];

                    //update the other clients
                    Byte[] byteArray3 = new Byte[3];
                    byteArray3[0] = mesArray[0];
                    byteArray3[1] = mesArray[1]; // player number
                    byteArray3[2] = mesArray[2]; // player position

                    // broadcasts movement / click to all clients
                    clients.Broadcast(byteArray3);

                    if (playerClicks > 65)
                    {
                        clients.Clear();
                        playerCount = 0;
                    }
                    break;
                default:
                    // code block
                    break;
            }
        }
    }
}