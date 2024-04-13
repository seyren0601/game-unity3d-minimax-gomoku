//REFERENCES
//https://cs.stackexchange.com/questions/70563/the-fastest-way-to-check-if-a-move-is-a-winning-move-in-tic-tac-toe Check winning move (generalized)

using System;
using System.Collections.Generic;
using System.Linq;
using static MiniMax.Game;
using UnityEngine;

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
        //public static int alpha;
        //public static int beta;
        public const int BOARD_SIZE = 5;
        const int LINE_SIZE = 4;
        const int DEPTH = 4;

        public static Point AutoPlay_GetMove(State startState, string player)
        {
            List<(State, int)> Moves = new List<(State, int)>();
            foreach(State state in GetAllMoves(startState))
            {
                int point = MiniMax_Run(state, int.MinValue, int.MaxValue, "X", 0);
                Moves.Add((state, point));
            }
            int max_point = Moves.Max(x => x.Item2);
            State bestState = Moves.FirstOrDefault(x => x.Item2 == max_point).Item1;
            // Debug writes
            /*Console.WriteLine($"Number of possible moves: {Moves.Count}");
            foreach(var move in Moves)
            {
                Console.WriteLine("###########Move state#############");
                State state_move = move.Item1;
                state_move.printState(); Console.WriteLine();
				foreach (var state in GetAllMoves(state_move))
				{
					int point = MiniMax_Run(state, player, 0);
					state.printState();
					Console.Write($"heuristic value = {point}\n\n");
				}
			}*/
			//Console.WriteLine($"Best state with h = {Heuristic("O", bestState.board)}: ");
            Debug.Log(max_point);
			return bestState.pre.Value.Item2;
        }

        public static int MiniMax_Run(State state, int alpha, int beta, string player, int depth)
        {
            Result result = CurrentState(state);
            if(result != Result.Pending)
            {
                if (result == Result.OWin) return int.MaxValue; // "O" win
                else if (result == Result.XWin) return int.MinValue; // "X" win
                else return 0; // Draw
            }
            if (depth == DEPTH)
            {
                int heuristic = Heuristic(state, "O");
                // Debug writes
                //state.printState();
                //Console.Write($"heuristic value = {heuristic}, depth={depth}\n\n");
                return heuristic;
            }
            if (player == "O")
            {
                foreach(State new_state in GetAllMoves(state))
                {
                    alpha = Math.Max(alpha, MiniMax_Run(new_state, alpha, beta, "X", depth + 1));
                    if (beta <= alpha) break;
                }
                return alpha;
            }
            else
            {
                foreach (State new_state in GetAllMoves(state))
                {
                    beta = Math.Min(beta, MiniMax_Run(new_state, alpha, beta, "O", depth + 1));
                    if (beta <= alpha) break;
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
            for(int i = 0; i < BOARD_SIZE; i++)
            {
                for(int j = 0; j < BOARD_SIZE; j++)
                {
                    if (state[i, j] == " ") return false;
                }
            }
            return true;
        }

        public static (bool, string?) isWinMove(State state)
        {
            if (state.pre is null) return (false, null);
            if (countLine(state, state.pre.Value.Item2) >= LINE_SIZE)
                return (true, state.pre.Value.Item3);
            return (false, null);
        }

        public static int countLine(State state, Point point) 
        {
            int count = 0;
            int max = int.MinValue;
            int x_original = point.x;
            int y_original = point.y;
            string player = state.pre.Value.Item3;

            // Check vertical
            count = CountVertical(point, player, state.board).Item2;
            if (count > max) max = count;

            // Check horizontal
            count = CountHorizontal(point, player, state.board).Item2;
            if (count > max) max = count;

            // Check top left->down right diagonal
            count = CountDiagonal1(point, player, state.board).Item2;
            if (count > max) max = count;

            // Check top right->down left diagonal
            count = CountDiagonal2(point, player, state.board).Item2;
            if (count > max) max = count;
            return max;
        }

        public static bool isValid(Point point)
        {
            if (point.x < 0 || point.y < 0 || point.x == BOARD_SIZE || point.y == BOARD_SIZE) return false;
            return true;
        }

        public static int Heuristic(State state, string player)
        {
            string enemy = player == "O" ? "X" : "O";
            int player_point = 0;

            for(int i = 0; i < BOARD_SIZE; i++)
            {
                for(int j=0;j< BOARD_SIZE; j++)
                {
                    Point currentPoint = new Point(i, j);
                    if (state.board[i,j] == player)
                    {
                        (int, int) horizontal = CountHorizontal(currentPoint, player, state.board);
                        (int, int) vertical = CountVertical(currentPoint, player, state.board);
                        (int, int) diagonal1 = CountDiagonal1(currentPoint, player, state.board);
                        (int, int) diagonal2 = CountDiagonal2(currentPoint, player, state.board);

                        // Đánh giá trạng thái bằng heuristic cho từng quân đã đặt lên bàn cờ
                        // Nếu có LINE_SIZE - 1 quân liền nhau và hở 2 hoặc 1 đầu
                        /*if (horizontal.Item2 == LINE_SIZE - 1 && horizontal.Item1 > 0) player_point = int.MaxValue;
                        if (vertical.Item2 == LINE_SIZE - 1 && vertical.Item1 > 0) player_point = int.MaxValue;
                        if (diagonal1.Item2 == LINE_SIZE - 1 && vertical.Item1 > 0) player_point = int.MaxValue;
                        if (diagonal1.Item2 == LINE_SIZE - 1 && vertical.Item1 > 0) player_point = int.MaxValue;*/

                        // Nếu có LINE_SIZE - 2 quân liền nhau và hở 2 hoặc 1 đầu
                        if (horizontal.Item1 == 2 && horizontal.Item2 == LINE_SIZE - 2) player_point += int.MaxValue;
                        else if (horizontal.Item1 == 1) player_point += 5;
                        if (vertical.Item1 > 0 && vertical.Item2 == LINE_SIZE - 2) player_point += int.MaxValue;
                        else if (vertical.Item1 == 1) player_point += 5;
                        if (diagonal1.Item1 > 0 && diagonal1.Item2 == LINE_SIZE - 2) player_point += int.MaxValue;
                        else if (diagonal1.Item1 == 1) player_point += 5;
                        if (diagonal2.Item1 > 0 && diagonal2.Item2 == LINE_SIZE - 2) player_point += int.MaxValue;
                        else if (diagonal2.Item1 == 1) player_point += 5;

                        // Nếu có LINE_SIZE - 3 quân liền nhau và hở 2 hoặc 1 đầu
                        if (LINE_SIZE - 3 > 0)
                        {
                            if (horizontal.Item1 > 0 && horizontal.Item2 == LINE_SIZE - 3) player_point += 3;
                            if (vertical.Item1 > 0 && vertical.Item2 == LINE_SIZE - 3) player_point += 3;
                            if (diagonal1.Item1 > 0 && diagonal1.Item2 == LINE_SIZE - 3) player_point += 3;
                            if (diagonal2.Item1 > 0 && diagonal2.Item2 == LINE_SIZE - 3) player_point += 3;
                        }
                    }
                }
            }
            return player_point;
        }

        // item1 = Số ô trống ở 2 đầu
        // item2 = Số quân liền nhau
        public static (int, int) CountHorizontal(Point point, string player, string[,] board)
        {
            int count = 0;
            int count_blank = 0;
            int max = int.MinValue;
            int x_original = point.x;
            int y_original = point.y;

            // Check vertical
            while (isValid(point))
            {
                if (board[point.x, point.y] == player)
                    count += 1;
                else
                {
                    if (board[point.x, point.y] == " ") count_blank += 1;
                    break;
                }
                point = point.Up;
            }
            point.x = x_original;
            point.y = y_original;
            while (isValid(point))
            {
                if (board[point.x, point.y] == player)
                    count += 1;
                else
                {
                    if (board[point.x, point.y] == " ") count_blank += 1;
                    break;
                }
                point = point.Down;
            }
            if (count - 1 > max) max = count - 1; //- 1 because the original point is counted twice
            return (count_blank, max);
        }

        public static (int, int) CountVertical(Point point, string player, string[,] board)
        {
            int count = 0;
            int count_blank = 0;
            int max = int.MinValue;
            int x_original = point.x;
            int y_original = point.y;

            // Check vertical
            while (isValid(point))
            {
                if (board[point.x, point.y] == player)
                    count += 1;
                else
                {
                    if (board[point.x, point.y] == " ") count_blank += 1;
                    break;
                }
                point = point.Left;
            }
            point.x = x_original;
            point.y = y_original;
            while (isValid(point))
            {
                if (board[point.x, point.y] == player)
                    count += 1;
                else
                {
                    if (board[point.x, point.y] == " ") count_blank += 1;
                    break;
                }
                point = point.Right;
            }
            if (count - 1 > max) max = count - 1; //- 1 because the original point is counted twice
            return (count_blank, max);
        }

        // Topright->BottomLeft
        public static (int, int) CountDiagonal1(Point point, string player, string[,] board)
        {
            int count = 0;
            int count_blank = 0;
            int max = int.MinValue;
            int x_original = point.x;
            int y_original = point.y;

            // Check vertical
            while (isValid(point))
            {
                if (board[point.x, point.y] == player)
                    count += 1;
                else
                {
                    if (board[point.x, point.y] == " ") count_blank += 1;
                    break;
                }
                point = point.UpRight;
            }
            point.x = x_original;
            point.y = y_original;
            while (isValid(point))
            {
                if (board[point.x, point.y] == player)
                    count += 1;
                else
                {
                    if (board[point.x, point.y] == " ") count_blank += 1;
                    break;
                }
                point = point.DownLeft;
            }
            if (count - 1 > max) max = count - 1; //- 1 because the original point is counted twice
            return (count_blank, max);
        }

        // Topleft->BottomRight
        public static (int, int) CountDiagonal2(Point point, string player, string[,] board)
        {
            int count = 0;
            int count_blank = 0;
            int max = int.MinValue;
            int x_original = point.x;
            int y_original = point.y;

            // Check vertical
            while (isValid(point))
            {
                if (board[point.x, point.y] == player)
                    count += 1;
                else
                {
                    if (board[point.x, point.y] == " ") count_blank += 1;
                    break;
                }
                point = point.UpLeft;
            }
            point.x = x_original;
            point.y = y_original;
            while (isValid(point))
            {
                if (board[point.x, point.y] == player)
                    count += 1;
                else
                {
                    if (board[point.x, point.y] == " ") count_blank += 1;
                    break;
                }
                point = point.DownRight;
            }
            if (count - 1 > max) max = count - 1; //- 1 because the original point is counted twice
            return (count_blank, max);
        }
    }
}
