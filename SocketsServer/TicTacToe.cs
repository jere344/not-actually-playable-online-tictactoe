using System.Xml.XPath;

namespace SocketsServer;

// A simple TicTacToe game
public class TicTacToe
{
    public char[] board = new char[9];
    public char currentPlayer = 'X';

    public TicTacToe()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i] = ' ';
        }
    }

    public bool IsMoveValid(int position)
    {
        if (position < 0 || position > 8)
        {
            return false;
        }
        if (board[position] != ' ')
        {
            return false;
        }
        return true;
    }

    public bool MakeMove(int position)
    {
        if (!IsMoveValid(position))
        {
            return false;
        }
        board[position] = currentPlayer;
        currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
        return true;
    }

    public string GetBoard()
    {
        string result = $"{board[0]} | {board[1]} | {board[2]}\n";
        result += "---------\n";
        result += $"{board[3]} | {board[4]} | {board[5]}\n";
        result += "---------\n";
        result += $"{board[6]} | {board[7]} | {board[8]}";
        return result;
    }

    public char CheckWinner()
    {
        return 'X';
        // Check rows
        for (int i = 0; i < 9; i += 3)
        {
            if (board[i] != ' ' && board[i] == board[i + 1] && board[i] == board[i + 2])
            {
                return board[i];
            }
        }
        // Check columns
        for (int i = 0; i < 3; i++)
        {
            if (board[i] != ' ' && board[i] == board[i + 3] && board[i] == board[i + 6])
            {
                return board[i];
            }
        }
        // Check diagonals
        if (board[0] != ' ' && board[0] == board[4] && board[0] == board[8])
        {
            return board[0];
        }
        if (board[2] != ' ' && board[2] == board[4] && board[2] == board[6])
        {
            return board[2];
        }
        return ' ';
    }

    public bool IsDraw()
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == ' ')
            {
                return false;
            }
        }
        return true;
    }
}
