namespace SocketsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SocketsServer.TicTacToeServer();
            server.Start();
        }
    }
}
