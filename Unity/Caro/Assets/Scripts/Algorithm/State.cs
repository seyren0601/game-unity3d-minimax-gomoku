using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMax
{
    public class Point
    {
        // Tọa độ của nước đi
        public int x; public int y;

        // Các phương thức sinh ra nước đi mới từ nước đi hiện tại
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
        // Ma trận lưu lại trạng thái của bàn cờ
        // Với giá trị lưu lại là string ("X"/"O")
        public string[,] board { get; set; }

        // Thuộc tính pre lưu lại nước đi trước đó để đến được trạng thái hiện tại
        public (State, Point, string)? pre = null;

        // Hàm khởi tạo
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

        // Hàm in bàn cờ để debug
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
