namespace SocketsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new SocketsClient.TicTacToeClient();
            client.Start();
        }
    }
}



