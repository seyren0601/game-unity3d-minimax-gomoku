//REFERENCES
//https://cs.stackexchange.com/questions/70563/the-fastest-way-to-check-if-a-move-is-a-winning-move-in-tic-tac-toe Check winning move (generalized)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MiniMax.Game;

namespace MiniMax
{
    internal class MiniMax
    {
        public enum Result
        {
            OWin,
            XWin,
            Draw,
            Pending
        }
        public static int alpha;
        public static int beta;
        public const int BOARD_SIZE = 3;
        const int LINE_SIZE = 3;
        const int DEPTH = 3;

        public static Point AutoPlay_GetMove(State startState, string player)
        {
            List<State> Moves = new List<State>();
            foreach(State state in GetAllMoves(startState))
            {
                alpha = int.MinValue;
                beta = int.MaxValue;
                int point = MiniMax_Run(state, player);
                if (point >= 0)
                {
                    return state.pre.Value.Item2;
                }
                Moves.Add(state);
            }
            return Moves[0].pre.Value.Item2;
        }

        public static int MiniMax_Run(State state, string player)
        {
            Result result = CurrentState(state);
            if(result != Result.Pending)
            {
                if (result == Result.OWin) return 1; // "O" win
                else if (result == Result.XWin) return -1; // "X" win
                else return 0; // Draw
            }
            if(player == "O")
            {
                foreach(State new_state in GetAllMoves(state))
                {
                    alpha = int.Max(alpha, MiniMax_Run(new_state, "X"));
                }
                return alpha;
            }
            else
            {
                foreach (State new_state in GetAllMoves(state))
                {
                    beta = int.Min(beta, MiniMax_Run(new_state, "O"));
                }
                return beta;
            }
        }

        public static List<State> GetAllMoves(State state)
        {
            List<State> Moves = new List<State>();
            string current_type = state.pre.Value.Item3 == "X" ? "O" : "X";
            string[,] board = state.board;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (board[i, j] == " ")
                    {
                        Point point = new Point(i, j);
                        string[,] newboard = (string[,])board.Clone();
                        newboard[i, j] = current_type;
                        State newState = new State(newboard, (state, new Point(i, j), current_type));
                        Moves.Add(newState);
                    }
                }
            }
            return Moves;
        }

        public static Result CurrentState(State state)
        {
            (bool, string?) result = isWinMove(state);
            if (result.Item1)
            {
                string winner = result.Item2!;
                if (winner == "X") return Result.XWin;
                else return Result.OWin;
            }
            else if (isFull(state.board))
            {
                return Result.Draw;
            }
            return Result.Pending;
        }

        public static bool isFull(string[,] state)
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    if (state[i, j] == " ") return false;
                }
            }
            return true;
        }

        public static (bool, string?) isWinMove(State state)
        {
            if (state.pre is null) return (false, null);
            if (countLine(state, state.pre.Value.Item2) == LINE_SIZE)
                return (true, state.pre.Value.Item3);
            return (false, null);
        }

        /*public static int countLine(string[,] board, Point point, string orientation)
        {
            Console.WriteLine(point.x + " " + point.y);
            int sum = 1;
            switch (orientation)
            {
                case "vertical":
                    Point Up = point.Up;
                    Point Down = point.Down;
                    if (isValid(Up) && board[Up.x, Up.y] == board[point.x, point.y]) sum += countLine(board, Up, "vertical");
                    if (isValid(Down) && board[Down.x, Down.y] == board[point.x, point.y]) sum += countLine(board, Down, "vertical");
                    break;
                case "horizontal":
                    Point Left = point.Left;
                    Point Right = point.Right;
                    if (isValid(Left) && board[Left.x, Left.y] == board[point.x, point.y]) sum += countLine(board, Left, "horizontal");
                    if (isValid(Right) && board[Right.x, Right.y] == board[point.x, point.y]) sum += countLine(board, Right, "horizontal");
                    break;
                case "diagonal_topleft_bottomright":
                    Point UpLeft = point.UpLeft;
                    Point DownRight = point.DownRight;
                    if (isValid(UpLeft) && board[UpLeft.x, UpLeft.y] == board[point.x, point.y]) sum += countLine(board, UpLeft, "diagonal_topleft_bottomright");
                    if (isValid(DownRight) && board[DownRight.x, DownRight.y] == board[point.x, point.y]) sum += countLine(board, DownRight, "diagonal_topleft_bottomright");
                    break;
                case "diagonal_topright_bottomleft":
                    Point UpRight = point.UpRight;
                    Point DownLeft = point.DownLeft;
                    if (isValid(UpRight) && board[UpRight.x, UpRight.y] == board[point.x, point.y]) sum += countLine(board, UpRight, "diagonal_bottomleft_topright");
                    if (isValid(DownLeft) && board[DownLeft.x, DownLeft.y] == board[point.x, point.y]) sum += countLine(board, DownLeft, "diagonal_bottomleft_topright");
                    break;
            }
            return sum;
        }*/

        public static int countLine(State state, Point point) // Đếm số quân cùng loại ở các hàng dọc, ngang, chéo bắt đầu từ ô cho trước
        {
            int count = 0;
            int max = int.MinValue;

            //Đếm số quân cùng hàng dọc
            for(int i = 0; i < BOARD_SIZE; i++)
            {
                if (state.board[i, point.y] == state.pre.Value.Item3) count += 1;
            }
            if (count > max) max = count;


            //Đếm số quân cùng hàng ngang
            count = 0;
            for(int i = 0; i < BOARD_SIZE; i++)
            {
                if (state.board[point.x, i] == state.pre.Value.Item3) count += 1;
            }
            if (count > max) max = count;

            // Đếm số quân cùng hàng chéo trên trái -> dưới phải
            count = 0;
            for(int i=0;i< BOARD_SIZE; i++)
            {
                for(int j=0;j< BOARD_SIZE; j++)
                {
                    if (i + j == point.x + point.y && state.board[i, j] == state.pre.Value.Item3) count += 1;
                }
            }
            if (count > max) max = count;

            // Đếm số quan cùng hàng chéo trên phải -> dưới trái
            count = 0;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (j - i == point.y - point.x && state.board[i, j] == state.pre.Value.Item3) count += 1;
                }
            }
            if (count > max) max = count;
            return max;
        }

        public static bool isValid(Point point)
        {
            if (point.x < 0 || point.y < 0 || point.x == BOARD_SIZE || point.y == BOARD_SIZE) return false;
            return true;
        }
    }
}
