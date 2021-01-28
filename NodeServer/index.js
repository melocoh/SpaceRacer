const crypto = require('crypto');
const express = require('express');
const {
    createServer
} = require('http');
const WebSocket = require('ws');

const app = express();

const server = createServer(app);
const wss = new WebSocket.Server({
    server
});
const port = process.env.PORT || 8085;

let wsArray = [];

var playerCount = 0;
var maxPlayers = 4;

/* On connection */
wss.on('connection', function (ws) {
    console.log("client joined.");
    wsArray.push(ws);

    /* Handle connected users */
    ws.on('message', function (data) {
        if (typeof (data) === "string") {
            // client sent a string
            console.log("string received from client -> '" + data + "'");

        } else {
            console.log("binary received from client -> " + Array.from(data).join(", ") + "");
            HandleMessage(data, ws);
        }
    });

    /* Handle disconnected users */
    ws.on('close', function () {
        console.log("client left.");
    });
});

/* Listen on port */
server.listen(port, function () {
    console.log("Server listening at port %d", port);
});

/* Handles socket messages */
function HandleMessage(mes, ws) {
    let mesArray = Array.from(mes)
    if (mesArray.length == 0) {
        return;
    }

    let key = mesArray[0];

    switch (key) {
        case 1:
            //new player
            if (playerCount < maxPlayers) {

                let buffer = new ArrayBuffer(2);
                let uint8View = new Uint8Array(buffer);
                uint8View[0] = 2;
                uint8View[1] = playerCount;
                playerCount++;

                // sends player position
                ws.send(uint8View);
                console.log("MESSAGE SENT!!");
            }
            break;
        case 2:
            // code block
            break;
        case 3:
            // onClick
            let playerID = mesArray[1];
            let playerClicks = mesArray[2];
            console.log("server-receieved: car update " + playerID.toString() + " : " + playerClicks.toString());

            //update the other clients
            let buffer = new ArrayBuffer(3);
            let uint8View = new Uint8Array(buffer);
            uint8View[0] = mesArray[0];
            uint8View[1] = mesArray[1];
            uint8View[2] = mesArray[2];

            // broadcasts movement / click to all clients
            wsArray.forEach(sendws => {
                sendws.send(uint8View);
            });
            break;
        default:
            // code block
            break;
    }
}
