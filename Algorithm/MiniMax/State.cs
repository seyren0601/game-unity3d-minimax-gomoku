using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMax
{
    internal class Point
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
    internal class State
    {
        public string[,] board { get; set; }
        public (State, Point, string)? pre = null;
        public State(string[,] board, (State, Point, string)? pre)
        {
            this.board = board;
            this.pre = pre;
        }
        public State()
        {
            board = new string[MiniMax.BOARD_SIZE, MiniMax.BOARD_SIZE];
            for(int i = 0; i < MiniMax.BOARD_SIZE; i++)
            {
                for(int j = 0; j < MiniMax.BOARD_SIZE; j++)
                {
                    board[i, j] = " ";
                }
            }
        }

        public void printState()
        {
            for(int i = 0; i < MiniMax.BOARD_SIZE; i++)
            {
                for(int j=0;j< MiniMax.BOARD_SIZE; j++)
                {
                    Console.Write($"| {board[i, j]} ");
                }
                Console.WriteLine("|");
                for(int k = 0;k< MiniMax.BOARD_SIZE; k++)
                {
                    Console.Write("----");
                }
                Console.WriteLine();
            }
        }
    }
}
