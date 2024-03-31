//REFERENCES
//https://cs.stackexchange.com/questions/70563/the-fastest-way-to-check-if-a-move-is-a-winning-move-in-tic-tac-toe Check winning move (generalized)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security;
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
        //public static int alpha;
        //public static int beta;
        public const int BOARD_SIZE = 3;
        const int LINE_SIZE = 3;
        const int DEPTH = 3;

        public static Point AutoPlay_GetMove(State startState, string player)
        {
            List<(State, int)> Moves = new List<(State, int)>();
            foreach(State state in GetAllMoves(startState))
            {
                int point = MiniMax_Run(state, player, 0);
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
			return bestState.pre.Value.Item2;
        }

        public static int MiniMax_Run(State state, string player, int depth)
        {
            Result result = CurrentState(state);
            if(result != Result.Pending)
            {
				if (result == Result.OWin) return 10; // "O" win
                else if (result == Result.XWin) return -10; // "X" win
                else return 0; // Draw
            }
			/*if (depth == 3)
			{
				int heuristic = Heuristic("O", state.board);
                // Debug writes
				//state.printState();
				//Console.Write($"heuristic value = {heuristic}, depth={depth}\n\n");
				return heuristic;
			}*/
			if (player == "O")
            {
                int alpha = int.MaxValue;
                foreach(State new_state in GetAllMoves(state))
                {
                    alpha = int.Min(alpha, MiniMax_Run(new_state, "X", depth + 1));
                }
                return alpha;
            }
            else
            {
                int beta = int.MinValue;
                foreach (State new_state in GetAllMoves(state))
                {
                    beta = int.Max(beta, MiniMax_Run(new_state, "O", depth + 1));
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

        public static int Heuristic(string player, string[,] board)
        {
            string enemy = player == "O" ? "X" : "O";
            int count_player = 0;
            int count_enemy = 0;

            // Count number of rows and columns that have win potential
            for(int i=0;i< BOARD_SIZE; i++)
            {
                List<string> pieces_in_row = new List<string>();
                List<string> pieces_in_column = new List<string>();
                for(int j=0;j< BOARD_SIZE; j++)
                {
                    pieces_in_row.Add(board[i,j]);
                    pieces_in_column.Add(board[j, i]);
                }

                // row
                if(pieces_in_row.Contains(enemy) && pieces_in_row.Contains(player))
                {
					count_player += pieces_in_row.Count(x => x == player);
					count_enemy += pieces_in_row.Count(x => x == enemy);
				}
                else
                {
                    if (pieces_in_row.Count(x => x == player) == 2) count_player += 3;
                    else count_enemy += 3;
				}

                // column
                if(pieces_in_column.Contains(enemy) && pieces_in_column.Contains(player))
                {
                    count_player += pieces_in_column.Count(x => x == player);
                    count_enemy += pieces_in_column.Count(x => x == enemy);
				}
                else
                {
					if (pieces_in_column.Count(x => x == player) == 2) count_player += 3;
					else count_enemy += 3;
				}
			}
            List<string> Diagonal1 = new List<string> { board[0, 0], board[1, 1], board[2, 2] };
            // ^
            if(Diagonal1.Contains(enemy) && Diagonal1.Contains(player))
            {
				count_player += Diagonal1.Count(x => x == player);
				count_enemy += Diagonal1.Count(x => x == player);
			}
            else
            {
				if (Diagonal1.Count(x => x == player) == 2) count_player += 3;
				else count_enemy += 3;
			}
			// ^
			List<string> Diagonal2 = new List<string> { board[0, 2], board[1, 1], board[2, 0] };
			if (Diagonal2.Contains(enemy) && Diagonal2.Contains(player))
			{
				count_player += Diagonal2.Count(x => x == player);
				count_enemy += 2 * Diagonal2.Count(x => x == player);
			}
			else
			{
				if (Diagonal2.Count(x => x == player) == 2) count_player += 3;
				else count_enemy += 3;
			}
			return count_player - count_enemy;
		}
    }
}
