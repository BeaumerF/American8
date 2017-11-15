using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace Server
{

    class Player
    {
        public int nb;
        public String cards;
        public Socket sock;
        public StreamWriter streamWriter;
        public StreamReader streamReader;
        public NetworkStream networkStream;

        public Player(int nb_, String cards_)
        {
            nb = nb_;
            cards = cards_;
        }
    }
}
