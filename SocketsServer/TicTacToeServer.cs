using System;
using System.Net;
using System.Net.Sockets;


namespace SocketsServer;

public class TicTacToeServer
{
    public int Port;
    public IPAddress HostIP;
    public TicTacToe game = new();
    public Socket? listenSocket;
    public TicTacToeServer()
    {
        Console.WriteLine("Enter host IP: ");
        string input = Console.ReadLine() ?? "";
        IPAddress? _serverIP = IPAddress.TryParse(input, out _serverIP) ? _serverIP : null;
        while (_serverIP == null)
        {
            Console.WriteLine("Invalid IP. Try again: ");
            input = Console.ReadLine() ?? "";
            _serverIP = IPAddress.TryParse(input, out _serverIP) ? _serverIP : null;
        }
        HostIP = _serverIP;

        Console.WriteLine("Enter server port: ");
        input = Console.ReadLine() ?? "";
        Port = int.Parse(input);
        while (Port < 1024 || Port > 65535)
        {
            Console.WriteLine("Invalid port. Try again: ");
            input = Console.ReadLine() ?? "";
            Port = int.Parse(input);
        }
    }

    public void Start()
    {
        listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ep = new IPEndPoint(HostIP, Port);

        try {
            listenSocket.Bind(ep);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            return;
        }

        Console.WriteLine("Listening for client...");
        listenSocket.Listen(1); // only 1 client for this game
        try
        {
            Socket clientSocket = listenSocket.Accept();
            Console.WriteLine("Client connected");

            // Game loop
            GameLoop(clientSocket);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            Console.WriteLine("Restarting server...");
            ResetConnection();
        }
    }

    public void ResetConnection()
    {
        // reset the game
        game = new TicTacToe();
        // reset the socket
        listenSocket?.Close();
        // start the server again
        Start();
    }

    public void GameLoop(Socket clientSocket)
    {
        Console.WriteLine("Starting game...");
        while (true)
        {
            // server is X, client is O
            if (game.currentPlayer == 'X')
            {
                ServerMove();
            }
            else
            {
                ClientMove(clientSocket);
            }

            // Check for winner
            char winner = game.CheckWinner();
            if (winner != ' ')
            {
                if (winner == 'X')
                {
                    Console.WriteLine("Winner : " + winner + " (server)");
                    SendWin(clientSocket, "Winner : " + winner + " (server)");
                }
                else
                {
                    Console.WriteLine("Winner : " + winner + " (client)");
                    SendWin(clientSocket, "Winner : " + winner + " (client)");
                }
                break;
            }

            // Check for draw
            if (game.IsDraw())
            {
                Console.WriteLine("Draw");
                SendWin(clientSocket, "Draw");
                break;
            }
        }

        // Play again?
        PlayAgain(clientSocket);
    }

    public void PlayAgain(Socket clientSocket)
    {
        // Check if the server wants to play again
        Console.WriteLine("Do you want to play again? (y/n)");
        string? input = Console.ReadLine();
        if (input != "y")
        {
            SendStop(clientSocket, "Game over");
            clientSocket.Close();
            return;
        }

        // Check if the client wants to play again
        SendQuestion(clientSocket, "Do you want to play again? (y/n)");
        byte[] buffer = new byte[1024];
        Console.WriteLine("Waiting for client...");
        clientSocket.Receive(buffer);
        string response = System.Text.Encoding.UTF8.GetString(buffer);
        Console.WriteLine("Client response: " + response);
        if (response[0] != 'y')
        {
            SendStop(clientSocket, "Game over");
            Console.WriteLine("Client does not want to play again. Closing connection...");
            clientSocket.Close();
            Console.WriteLine("Connection closed");
            return;
        }

        // If we get here, both server and client want to play again
        // so we reset the game
        Console.WriteLine("Restarting game...");
        game = new TicTacToe();
        // and start the game loop again
        GameLoop(clientSocket);
    }

    public int ServerMove()
    {
        Console.WriteLine("--- Current Board ---");
        Console.WriteLine(game.GetBoard());
        Console.WriteLine("---------------------");
        Console.WriteLine("Enter server move (0-8): ");
        string? input = Console.ReadLine();
        int position;
        position = int.TryParse(input, out position) ? position : -1;
        while (!game.IsMoveValid(position))
        {
            Console.WriteLine("Invalid move. Try again: ");
            input = Console.ReadLine();
            position = int.TryParse(input, out position) ? position : -1;
        }
        Console.WriteLine("Server move: " + position);
        game.MakeMove(position);
        return position;
    }

    public int ClientMove(Socket clientSocket)
    {
        // Send the board to the client
        SendBoard(clientSocket);


        // Get the move from the client
        byte[] buffer = new byte[1024];
        Console.WriteLine("Waiting for client...");
        int bytesRead = clientSocket.Receive(buffer);
        string move = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
        int position = int.Parse(move);
        if (!game.IsMoveValid(position))
        {
            Console.WriteLine("Invalid move from client. no move made.  Try again");
        }
        else
        {
            Console.WriteLine("Client move: " + position);
            game.MakeMove(position);
        }
        return position;
    }

    public void SendBoard(Socket clientSocket)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("BOARD:" + game.GetBoard());
        clientSocket.Send(buffer);
    }

    public void SendWin(Socket clientSocket, string message)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("WIN:" + message);
        clientSocket.Send(buffer);
    }

    public void SendQuestion(Socket clientSocket, string message)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("ASK:" + message);
        clientSocket.Send(buffer);
    }

    public void SendStop(Socket clientSocket, string message)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("STOP:" + message);
        clientSocket.Send(buffer);
    }
}
