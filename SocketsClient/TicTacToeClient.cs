using System;
using System.Net;
using System.Net.Sockets;

namespace SocketsClient;

public class TicTacToeClient
{
    public IPAddress serverIP = IPAddress.Parse("127.0.0.1");
    public int Port = 12345;

    public TicTacToeClient()
    {
    }

    public void Start()
    {
        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ep = new IPEndPoint(serverIP, Port);

        Console.WriteLine("Connecting to server...");
        clientSocket.Connect(ep);
        Console.WriteLine("Connected to server");

        // Game loop
        GameLoop(clientSocket);
    }

    public void GameLoop(Socket clientSocket)
    {
        byte[] buffer;
        while (true)
        {
            // Receive the message from the server
            buffer = new byte[1024];

            Console.WriteLine("Waiting for server...");
            clientSocket.Receive(buffer);
            string response = System.Text.Encoding.UTF8.GetString(buffer);

            // Call the appropriate method based on the response
            if (response.StartsWith("BOARD:"))
            {
                BoardCommand(clientSocket, response[6..]);
                continue;
            }
            else if (response.StartsWith("WIN:"))
            {
                WinCommand(response[4..]);
                continue;
            }
            else if (response.StartsWith("ASK:"))
            {
                AskCommand(clientSocket, response[4..]);
                continue;
            }
            else if (response.StartsWith("STOP:"))
            {
                StopCommand(response[5..]);
                break;
            }
            else
            {
                Console.WriteLine("Unknown command: " + response);
                break;
            }
        }
    }

    public static void BoardCommand(Socket clientSocket, string response)
    {
        Console.WriteLine("--- Current Board ---");
        Console.WriteLine(response);
        Console.WriteLine("---------------------");
        // Send the move to the server
        // If the move is invalid, the server will just not make the move and send the board again
        int position = -1;
        while (position < 0 || position > 8)
        {
            Console.WriteLine("Enter a position (0-8): ");
            position = int.TryParse(Console.ReadLine(), out position) ? position : -1;
        }
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(position.ToString());
        clientSocket.Send(buffer);
    }

    public static void WinCommand(string response)
    {
        Console.WriteLine(response);
    }

    public static void AskCommand(Socket clientSocket, string response)
    {
        // Show server message
        Console.WriteLine("Server is asking : " + response);

        // Send the answer to the server
        Console.WriteLine("Your answer: ");
        string? answer = Console.ReadLine();
        while (string.IsNullOrEmpty(answer))
        {
            Console.WriteLine("Answer cannot be empty. Try again: ");
            answer = Console.ReadLine();
        }
        byte[]  buffer = System.Text.Encoding.UTF8.GetBytes(answer);
        clientSocket.Send(buffer);
    }

    public static void StopCommand(string response)
    {
        Console.WriteLine(response);
    }
}
