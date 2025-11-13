import express from "express";
import { createServer } from "node:http";
import { Server, Socket } from "socket.io";
import { v4 as uuidv4 } from "uuid";
import { PlayerManager } from "./managers/PlayerManager";
import { LobbyPlayer } from "./types/lobby";

interface PlayerData {
  x: number;
  y: number;
  socketId: string;
}

interface Position {
  x: number;
  y: number;
}

const app = express();
const server = createServer(app);
const io = new Server(server, {
  cors: {
    origin: "*",
    methods: ["GET", "POST"]
  }
});

const playerManager = new PlayerManager();
playerManager.addDummyPlayers(2);

io.on("connection", (socket: Socket) => {
  console.log("a client connected");

  socket.on("signin:guest", () => {
    const userId = uuidv4();
    const newPlayer = playerManager.addPlayer(userId, socket.id);

    socket.emit("signin:success", userId);
    console.log(`Guest signin: ${userId}`);

    const otherPlayers: LobbyPlayer[] = playerManager.getOtherPlayers(userId);
    socket.emit("player:other", otherPlayers);
    socket.broadcast.emit("player:join", {
      userId,
      x: newPlayer.position.x,
      y: newPlayer.position.y
    })
  });

  socket.on("player:move", (position: Position) => {
    const userId = playerManager.getUserIdBySocket(socket.id);
    if (!userId) return;

    console.log(`player ${userId} moved to (${position.x}, ${position.y})`);

    if (playerManager.updatePosition(userId, position)) {
      socket.broadcast.emit("player:moved", {
        userId,
        x: position.x,
        y: position.y
      })
    }
  });
});

const PORT = 3000;
server.listen(PORT, () => {
  console.log(`server running at http://localhost:${PORT}`);
});