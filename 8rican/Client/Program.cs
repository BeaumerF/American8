using System;
using System.Net.Sockets;
using System.Threading;



public class Worker
{
    public void ReadServer()
    {
        string strRead;
        try {
            while (!_shouldStop)
            {
                strRead = streamReader.ReadLine();
                if (strRead != "")
                    Console.WriteLine("Message Recieved by server:" + strRead);
            }
        }
        catch (System.IO.IOException)
        {
            _shouldStop = true;
            return;
        }
    }

    public void setStreamReader(System.IO.StreamReader _streamReader)
    {
        this.streamReader = _streamReader;   
    }

    public void RequestStop()
    {
        _shouldStop = true;
    }

    private volatile bool _shouldStop;
    private System.IO.StreamReader streamReader;
}

public class Client
{
    static public void Main(string[] Args)
    {
        TcpClient socketForServer;
        try
        {
            socketForServer = new TcpClient("localHost", 4242);
        }
        catch
        {
            Console.WriteLine(
            "Failed to connect to server at {0}:4242", "localhost");
            return;
        }

        NetworkStream networkStream = socketForServer.GetStream();
        System.IO.StreamReader streamReader =
                  new System.IO.StreamReader(networkStream);
        System.IO.StreamWriter streamWriter =
                  new System.IO.StreamWriter(networkStream);
        try
        {
            Worker workerObject = new Worker();
            workerObject.setStreamReader(streamReader);
            Thread thread = new Thread(workerObject.ReadServer);
            thread.Start();
            string str = Console.ReadLine();
            while (str != "exit")
            {
                streamWriter.WriteLine(str);
                streamWriter.Flush();
                str = Console.ReadLine();
            }
            if (str == "exit")
            {
                streamWriter.WriteLine(str);
                streamWriter.Flush();
                Thread.Sleep(1);
                workerObject.RequestStop();
                thread.Join();
            }
        }
        catch
        {
            Console.WriteLine("Exception reading from Server");
        }
        // tidy up
        networkStream.Close();
        Console.ReadKey();
    }
}
