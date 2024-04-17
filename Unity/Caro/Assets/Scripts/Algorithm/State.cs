using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMax
{
    public class Point
    {
        public int x; 
        public int y;
        public Point Up { get { return new Point(x - 1, y); } }
        public Point Down { get { return new Point(x + 1, y); } }
        public Point Left { get { return new Point(x, y - 1); } }
        public Point Right { get { return new Point(x, y + 1); } }
        public Point UpLeft { get { return new Point(x - 1, y - 1); } }
        public Point UpRight { get { return new Point(x - 1, y + 1); } }
        public Point DownLeft { get { return new Point(x + 1, y - 1); } }
        public Point DownRight { get { return new Point(x + 1, y + 1); } }
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public class State
    {
        public string[,] board = { { " ", " ", " " }, { " ", " ", " " }, { " ", " ", " " } };
        public (State, Point, string)? pre = null;
        public State(string[,] board, (State, Point, string)? pre)
        {
            this.board = board;
            this.pre = pre;
        }
        public State() { }

        public void printState()
        {
            Console.WriteLine($"| {board[0, 0]} | {board[0, 1]} | {board[0, 2]} |");
            Console.WriteLine("-------------");
            Console.WriteLine($"| {board[1, 0]} | {board[1, 1]} | {board[1, 2]} |");
            Console.WriteLine("-------------");
            Console.WriteLine($"| {board[2, 0]} | {board[2, 1]} | {board[2, 2]} |");
            Console.Write("-------------");
        }
    }
}
