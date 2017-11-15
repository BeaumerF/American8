using System;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

public class Program
{
    static int passCount = 0;
    static bool isFirstTurn = true;
    static int nbClients = 0;
    static Server.Player[] pl;
    static TcpListener tcpListener = new TcpListener(4242);
    static String toSay = "";
    static String lastPlay = "";

    static Boolean isValid(Server.Player pl, String str)
    {
        if (pl.cards.Contains(str))
            return (true);
        return (false);
    }

    static void writeForEverybody(String txt)
    {
        for (int a = 0; a < nbClients; a++)
        {
            if (pl[a].sock.Connected)
            {
                pl[a].streamWriter.WriteLine(txt);
                pl[a].streamWriter.Flush();
            }
        }
    }

    static bool isWinner(String str)
    {
        if (Regex.Matches(str, @"[a-zA-Z]").Count > 0)
            return (false);
        return (true);
    }

    static String getFirstArg(String str)
    {
        return (str.Split(".")[0]);
    }

    static String getSecondArg(String str)
    {
        return (str.Substring(str.LastIndexOf('.') + 1));
    }

    static void Listeners(Server.Player player)
    {
        player.sock = tcpListener.AcceptSocket();
        if (player.sock.Connected)
        {
            Console.WriteLine("Client:" + player.sock.RemoteEndPoint + " now connected to server.");
            player.networkStream = new NetworkStream(player.sock);
            player.streamWriter = new System.IO.StreamWriter(player.networkStream);
            player.streamReader = new System.IO.StreamReader(player.networkStream);
        }
    }

    static int play(Server.Player player)
    {
        player.streamWriter.WriteLine("Here are your cards: " + player.cards);
        player.streamWriter.Flush();
        string theString = player.streamReader.ReadLine();

        Console.WriteLine("Message recieved by client:" + theString);

        if (isValid(player, theString))
        {
            String first = getFirstArg(theString);
            String second = getSecondArg(theString);

            if (getFirstArg(lastPlay).Equals(first) ||
                (getSecondArg(lastPlay).Equals(second)) ||
                first.Equals("8") ||
                isFirstTurn == true)
                {
                isFirstTurn = false;
                passCount = 0;
                player.cards = player.cards.Replace(theString, " ");
                lastPlay = theString;
                if (first.Equals("V")) // si c'est un valet, passe le tour du joueur suivant
                    return (player.nb + 1);
                return (player.nb);
            }
            else
            {
                player.streamWriter.WriteLine("Play something you can.");
                player.streamWriter.Flush();
                play(player);
            }
        }
        else if (theString == "pass")
        {
            ++passCount;
            return (player.nb);
        }
        else if (theString == "exit") // attente reconnexion d'un joueur
        {
            player.streamReader.Close();
            player.networkStream.Close();

            Console.WriteLine("Waiting for an other player to take the place.");
            player.sock = tcpListener.AcceptSocket();
            if (player.sock.Connected)
            {
                Console.WriteLine("Client:" + player.sock.RemoteEndPoint + " now connected to server.");
                player.networkStream = new NetworkStream(player.sock);
                player.streamWriter = new System.IO.StreamWriter(player.networkStream);
                player.streamReader = new System.IO.StreamReader(player.networkStream);
                player.streamWriter.WriteLine("Your cards: " + player.cards);
                writeForEverybody("Player " + player.nb + " joined the game.");
                play(player);
            }
        }
        else
        {
            player.streamWriter.WriteLine("Invalid command");
            play(player);
        }
        return (player.nb);
    }

    static String replacing(String str)
    {
        str = str.Replace("A", ".Trefle");
        str = str.Replace("C", ".Coeur");
        str = str.Replace("B", ".Carreau");
        str = str.Replace("D", ".Pique");
        str = str.Replace("11", "V");
        str = str.Replace("12", "D");
        str = str.Replace("13", "R");
        str = str.Replace("14", "A");
        return (str);
    }

    public static void Main()
    {
        tcpListener.Start();
 
        Console.WriteLine("************Hello World************");
        while (!(nbClients <= 4 && nbClients >= 2))
        {
            Console.WriteLine("How many clients are going to play ? (between 2 to 4)");
            nbClients = int.Parse(Console.ReadLine());
        }

        Server.Card c = new Server.Card(nbClients);
        pl = new Server.Player[nbClients];
        for (int i = 0; i < nbClients; i++)
        {
            c.strtab[i] = c.distribution();
            pl[i] = new Server.Player(i, replacing(c.strtab[i]));
            Console.WriteLine(pl[i].nb + " = " + pl[i].cards);
            Listeners(pl[i]);
        }

        while (true)
        {
            for (int turn = 0; turn < nbClients; turn++)
            {
                turn = play(pl[turn]);
                if (turn == nbClients)
                {
                    writeForEverybody("player " + (turn - 1) + " played " + lastPlay);
                    if (isWinner(pl[turn - 1].cards))
                    {
                        writeForEverybody("player " + (turn - 1) + " win the game");
                        Thread.Sleep(1000);
                        Environment.Exit(1);
                    }
                    turn = 0;
                }
                else
                {
                    writeForEverybody("player " + turn + " played " + lastPlay);
                    if (isWinner(pl[turn].cards))
                    {
                        writeForEverybody("player " + (turn) + " win the game");
                        Thread.Sleep(1000);
                        Environment.Exit(1);
                    }
                }
                if (passCount == nbClients)
                {
                    writeForEverybody("Nobody can play, this is a new turn.");
                    isFirstTurn = true;
                }
            }
        }
    }
}
