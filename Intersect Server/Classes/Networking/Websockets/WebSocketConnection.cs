﻿using System;
using Intersect.Network;
using Intersect.Network.Packets.Reflectable;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace Intersect.Server.Classes.Networking.Websockets
{
    public class WebSocketConnection : AbstractConnection
    {
        private readonly WebSocketBehavior behavior;
        private readonly WebSocketContext context;
        private ByteBuffer buffer = new ByteBuffer();
        private object bufferLock = new Object();
        private bool clientRemoved;
        private PacketHandler packetHandler = new PacketHandler();

        public WebSocketConnection(WebSocketContext context, WebSocketBehavior behavior)
        {
            this.context = context;
            context.WebSocket.OnMessage += WebSocket_OnMessage;
            context.WebSocket.OnClose += WebSocket_OnClose;
            context.WebSocket.OnError += WebSocket_OnError;
            Client.CreateBeta4Client(this);
        }

        public override string Ip
        {
            get { return this.context.UserEndPoint.Address.ToString(); }
        }

        public override int Port
        {
            get { return this.context.UserEndPoint.Port; }
        }

        private void WebSocket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            if (!clientRemoved)
            {
                clientRemoved = true;
                Client.RemoveBeta4Client(this);
            }
        }

        private void WebSocket_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            if (!clientRemoved)
            {
                clientRemoved = true;
                Client.RemoveBeta4Client(this);
            }
        }

        private void WebSocket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (e.IsBinary && !clientRemoved)
            {
                lock (bufferLock)
                {
                    buffer.WriteBytes(e.RawData);
                    ParseData();
                }
            }
        }

        private void ParseData()
        {
            int packetLen;
            if (clientRemoved) return;
            lock (bufferLock)
            {
                while (buffer.Length() >= 4)
                {
                    packetLen = buffer.ReadInteger(false);
                    if (packetLen == 0)
                    {
                        break;
                    }
                    if (buffer.Length() >= packetLen + 4)
                    {
                        buffer.ReadInteger();
                        var data = buffer.ReadBytes(packetLen);
                        var bf = new ByteBuffer();
                        bf.WriteBytes(data);
                        var packet = new BinaryPacket(this) {Buffer = bf};
                        packetHandler.HandlePacket(packet);
                    }
                    else
                    {
                        break;
                    }
                }
                if (buffer.Length() == 0)
                {
                    buffer.Clear();
                }
            }
        }

        public override bool Send(IPacket packet)
        {
            try
            {
                if (packet.GetType() == typeof(BinaryPacket))
                {
                    BinaryPacket bpacket = (BinaryPacket) packet;
                    var bf = new ByteBuffer();
                    bf.WriteInteger(bpacket.Buffer.ToArray().Length);
                    bf.WriteBytes(bpacket.Buffer.ToArray());
                    context.WebSocket.SendAsync(bf.ToArray(), null);
                    return true;
                }
                else
                {
                    throw new Exception("Websockets cannot send non-binary packets yet!");
                }
            }
            catch (InvalidOperationException ex)
            {
                //Do Nothing.. the socket is just disconnected.
            }
            return false;
        }
    }
}