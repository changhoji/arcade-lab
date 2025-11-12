import express from "express";
import { createServer } from "node:http";
import { Server, Socket } from "socket.io";
import { v4 as uuidv4 } from "uuid";

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

const players = new Map<string, PlayerData>();
const socketToUserId = new Map<string, string>();

io.on("connection", (socket: Socket) => {
  console.log("a client connected");

  socket.on("signin:guest", () => {
    const userId = uuidv4();

    players.set(userId, {
      x: 0,
      y: 0,
      socketId: socket.id
    });

    socketToUserId.set(socket.id, userId);

    socket.emit("signin:success", userId);
    console.log(`Guest signin: ${userId}`);

    const existingPlayers = Array.from(players.entries())
      .filter(([id]) => id !== userId)
      .map(([id, data]) => ({ userId: id, x: data.x, y: data.y }));

    socket.emit("players:existing", existingPlayers);

    socket.broadcast.emit("player:joined", {
      userId,
      x: 0,
      y: 0
    });
  });

  socket.on("player:move", (position: Position) => {
    const currentUserId = socketToUserId.get(socket.id);
    if (!currentUserId) return;

    const player = players.get(currentUserId);
    if (player) {
      player.x = position.x;
      player.y = position.y;

      socket.broadcast.emit("player:moved", {
        userId: currentUserId,
        x: position.x,
        y: position.y
      });
    }
  });

  socket.on("disconnect", () => {
    const userId = socketToUserId.get(socket.id);
    if (userId) {
      players.delete(userId);
      socketToUserId.delete(socket.id);
      socket.broadcast.emit("player:left", { userId });
      console.log(`Player disconnected: ${userId}`);
    }
  });
});

const PORT = 3000;
server.listen(PORT, () => {
  console.log(`server running at http://localhost:${PORT}`);
});