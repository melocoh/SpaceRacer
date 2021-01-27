const crypto = require('crypto');
const express = require('express');
const { createServer } = require('http');
const WebSocket = require('ws');

const app = express();

const server = createServer(app);
const wss = new WebSocket.Server({ server });

wss.on('connection', function(ws) {
  console.log("client joined.");

  // send "hello world" interval
  const textInterval = setInterval(() => ws.send("hello world!"), 100);

  // send random bytes interval
  const binaryInterval = setInterval(() => ws.send(crypto.randomBytes(8).buffer), 110);

  ws.on('message', function(data) {
    if (typeof(data) === "string") {
      // client sent a string
      console.log("string received from client -> '" + data + "'");

    } else {
      console.log("binary received from client -> " + Array.from(data).join(", ") + "");
    }
  });

  ws.on('close', function() {
    console.log("client left.");
    clearInterval(textInterval);
    clearInterval(binaryInterval);
  });
});

server.listen(8080, function() {
  console.log('Listening on http://localhost:8080');
});

function SendMessage() {
  // creating an array of bytes, with size 1024
  let arr = new Uint8Array(1024);

}



// needs to pass in a different parameter because byte array doesn't exist :'(
function HandleMessage(byte[] mes) {

  if (mes.length == 0) {
    return;
  }

  // needs to change byte
  byte key = mes[0];


  switch (key) {
    case 1:
      //new player
      if (playerCount < maxPlayers) {
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
