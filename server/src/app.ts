import express from 'express';
import { createServer } from 'node:http';
import { Server } from 'socket.io';
import { setupNameSpaces } from './namespaces';

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
    origin: '*',
    methods: ['GET', 'POST'],
  },
});

setupNameSpaces(io);

const PORT = 3000;
server.listen(PORT, () => {
  console.log(`server running at http://localhost:${PORT}`);
});
