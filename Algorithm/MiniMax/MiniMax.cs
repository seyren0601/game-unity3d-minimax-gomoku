//REFERENCES
//https://cs.stackexchange.com/questions/70563/the-fastest-way-to-check-if-a-move-is-a-winning-move-in-tic-tac-toe Check winning move (generalized)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
        // Kết quả cuối cùng của trò chơi
        public enum Result
        {
            OWin,
            XWin,
            Draw,
            Pending
        }
        // Giá trị alpha và beta để chạy thuật toán minimax
        public static int alpha_root;
        public static int beta_root;

        // Các thông số của trò chơi
        // BOARD_SIZE là cạnh n của bàn cờ, kích thước của bàn cờ là n x n
        // LINE_SIZE là số lượng quân cờ liên tiếp nhau để chiến thắng
        // DEPTH là chiều sau của thuật toán minimax
        public const int BOARD_SIZE = 6;
        const int LINE_SIZE = 4;
        const int DEPTH = 5; // Giá trị bắt buộc phải là số lẻ, vì thuật toán minimax đang trả về kết quả theo quân "X"

		// Hàm chọn nước đi tiếp theo cho máy
		// Giao ước máy là "O" và người chơi là "X"
		public static Point AutoPlay_GetMove(State startState, string player)
		{
			// List chứa các bước đi tiếp theo cùng điểm số của từng bước đi
			List<(State, int)> Moves = new List<(State, int)>();

			// Khởi tạo giá trị alpha và beta
			alpha_root = int.MinValue;
			beta_root = int.MaxValue;

			// Tính điểm số của từng nước đi, sau đó lưu vào List Moves
			foreach (State state in GetAllMoves(startState))
			{
				int point = MiniMax_Run(state, alpha_root, beta_root, "X", 1);
				Moves.Add((state, point));
			}

			// Chọn bước đi có điểm số tối đa, vì root của cây trò chơi là 
			// "O" => "O" là Max
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

		// Hàm tính điểm cho bước đi bằng thuật toán MiniMax
		public static int MiniMax_Run(State state, int alpha, int beta, string player, int depth)
		{
			Result result = CurrentState(state);
			if (result != Result.Pending)
			{
				if (result == Result.OWin) return int.MaxValue; // "O" win
				else if (result == Result.XWin) return int.MinValue; // "X" win
				else return 0; // Draw
			}
			if (depth == DEPTH)
			{
				int heuristic = Heuristic(state, "X");
				// Debug writes
				//state.printState();
				//Console.Write($"heuristic value = {heuristic}, depth={depth}\n\n");
				return heuristic;
			}
			if (player == "O")
			{
				foreach (State new_state in GetAllMoves(state))
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

		// Hàm sinh ra các nước đi mới từ trạng thái gửi vào
		public static List<State> GetAllMoves(State state)
		{
			List<State> Moves = new List<State>();
			// Kiểm tra xem trạng thái hiện tại đang là lượt của ai
			string current_type = state.pre.Value.Item3 == "X" ? "O" : "X";
			string[,] board = state.board;

			// Thuật toán kiểm tra và trả lại các nước đi khả thi
			// của quân cờ hiện tại trong trạng thái hiện tại
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					// Nếu ô trên bàn cờ còn trống
					if (board[i, j] == " ")
					{
						// Tạo một nước đi mới và Add nước đi này vào List Moves
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

		// Kiểm tra kết quả của trạng thái hiện tại
		// Có 3 giá trị có thể xảy ra
		// Biểu hiện bằng Enum Result:
		// XWin, OWin, Draw hoặc Pending
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

		// Kiểm tra xem bàn cờ đã đầy hay chưa
		public static bool isFull(string[,] state)
		{
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					if (state[i, j] == " ") return false;
				}
			}
			return true;
		}

		// Kiểm tra xem nước đi vừa rồi có phải là nước đi kết thúc trò chơi không
		public static (bool, string?) isWinMove(State state)
		{
			if (state.pre is null) return (false, null);
			// Nếu hàm countLine trả về giá trị lớn hơn hoặc bằng LINE_SIZE
			// => nước đi vừa rồi là nước đi kết thúc trò chơi
			if (countLine(state, state.pre.Value.Item2) >= LINE_SIZE)
				return (true, state.pre.Value.Item3);
			return (false, null);
		}

		// Hàm đếm số quân cờ liền nhau từ quân cờ (tọa độ hiện tại)
		// Hàm này trả về số lượng quân cờ liền nhau lớn nhất
		// Sau khi kiểm tra qua các chiều ngang, dọc và chéo
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

		// Hàm kiểm tra xem tọa độ truyền vào có hợp lệ hay không
		public static bool isValid(Point point)
		{
			if (point.x < 0 || point.y < 0 || point.x == BOARD_SIZE || point.y == BOARD_SIZE) return false;
			return true;
		}

		// Hàm trả về giá trị điểm số của trạng thái hiện tại
		// Mặc định là trả về giá trị điểm số theo "O", vì "O" là máy
		public static int Heuristic(State state, string X)
		{
			string O = "O";
			int O_point = 0;
			Point O_move = state.pre.Value.Item2;

			// Hàm xét quả TẤT CẢ (n x n) ô trên bàn cờ
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					Point currentPoint = new Point(i, j);
					// Trong trường hợp quân cờ là "X"
					if (state.board[i, j] == X)
					{
						// Đếm số quân cờ liên tiếp trên hàng dọc, ngang và hai đường chéo
						(int, int) horizontal = CountHorizontal(currentPoint, X, state.board);
						(int, int) vertical = CountVertical(currentPoint, X, state.board);
						(int, int) diagonal1 = CountDiagonal1(currentPoint, X, state.board);
						(int, int) diagonal2 = CountDiagonal2(currentPoint, X, state.board);

						// Đánh giá trạng thái bằng heuristic cho từng quân đã đặt lên bàn cờ
						// Nếu có LINE_SIZE - 1 quân liền nhau và hở 2 hoặc 1 đầu
						if (horizontal.Item1 == 2 && horizontal.Item2 == LINE_SIZE - 1) O_point = int.MinValue;
						else if (horizontal.Item1 == 1) O_point -= 50;
						if (vertical.Item1 == 2 && vertical.Item2 == LINE_SIZE - 1) O_point = int.MinValue;
						else if (vertical.Item1 == 1) O_point -= 50;
						if (diagonal1.Item1 == 2 && diagonal1.Item2 == LINE_SIZE - 1) O_point = int.MinValue;
						else if (diagonal1.Item1 == 1) O_point -= 50;
						if (diagonal2.Item1 == 2 && diagonal2.Item2 == LINE_SIZE - 1) O_point = int.MinValue;
						else if (diagonal2.Item1 == 1) O_point -= 50;



						// Nếu có LINE_SIZE - 2 quân liền nhau và hở 2 hoặc 1 đầu
						if (LINE_SIZE - 3 > 0)
						{
							if (horizontal.Item1 > 0 && horizontal.Item2 == LINE_SIZE - 2) O_point -= 5;
							if (vertical.Item1 > 0 && vertical.Item2 == LINE_SIZE - 2) O_point -= 5;
							if (diagonal1.Item1 > 0 && diagonal1.Item2 == LINE_SIZE - 2) O_point -= 5;
							if (diagonal2.Item1 > 0 && diagonal2.Item2 == LINE_SIZE - 2) O_point -= 5;
						}
					}
					// Trong trường hợp quân cờ là "O"
					else if (state.board[i, j] == O)
					{
						(int, int) horizontal_enemy = CountHorizontal(currentPoint, O, state.board);
						(int, int) vertical_enemy = CountVertical(currentPoint, O, state.board);
						(int, int) diagonal1_enemy = CountDiagonal1(currentPoint, O, state.board);
						(int, int) diagonal2_enemy = CountDiagonal2(currentPoint, O, state.board);

						if (horizontal_enemy.Item1 == 2 && horizontal_enemy.Item2 == LINE_SIZE - 1) O_point = int.MaxValue;
						else if (horizontal_enemy.Item1 == 1) O_point += 50;
						if (vertical_enemy.Item1 == 2 && vertical_enemy.Item2 == LINE_SIZE - 1) O_point = int.MaxValue;
						else if (vertical_enemy.Item1 == 1) O_point += 50;
						if (diagonal1_enemy.Item1 == 2 && diagonal1_enemy.Item2 == LINE_SIZE - 1) O_point = int.MaxValue;
						else if (diagonal1_enemy.Item1 == 1) O_point += 50;
						if (diagonal2_enemy.Item1 == 2 && diagonal2_enemy.Item2 == LINE_SIZE - 1) O_point = int.MaxValue;
						else if (diagonal2_enemy.Item1 == 1) O_point += 50;

						if (LINE_SIZE - 3 > 0)
						{
							if (horizontal_enemy.Item1 > 0 && horizontal_enemy.Item2 == LINE_SIZE - 2) O_point += 5;
							if (vertical_enemy.Item1 > 0 && vertical_enemy.Item2 == LINE_SIZE - 2) O_point += 5;
							if (diagonal1_enemy.Item1 > 0 && diagonal1_enemy.Item2 == LINE_SIZE - 2) O_point += 5;
							if (diagonal2_enemy.Item1 > 0 && diagonal2_enemy.Item2 == LINE_SIZE - 2) O_point += 5;
						}

					}
				}
			}

			// Đánh giá nước đi trước của "O"
			Point left = O_move.Left;
			Point right = O_move.Right;
			Point down = O_move.Down;
			Point up = O_move.Up;
			Point upleft = O_move.UpLeft;
			Point upright = O_move.UpRight;
			Point downleft = O_move.DownLeft;
			Point downright = O_move.DownRight;
			// Tính số quân cờ đối phương ("X") nằm cạnh quân cờ này
			// Với mỗi quân cờ cộng thêm 10 điểm để khuyến khích chọn nước đi
			// gần đối phương hơn
			if (isValid(left) && state.board[left.x, left.y] == X) O_point += 10;
			if (isValid(right) && state.board[right.x, right.y] == X) O_point += 10;
			if (isValid(down) && state.board[down.x, down.y] == X) O_point += 10;
			if (isValid(up) && state.board[up.x, up.y] == X) O_point += 10;
			if (isValid(upleft) && state.board[upleft.x, upleft.y] == X) O_point += 10;
			if (isValid(downleft) && state.board[downleft.x, downleft.y] == X) O_point += 10;
			if (isValid(upright) && state.board[upright.x, upright.y] == X) O_point += 10;
			if (isValid(downright) && state.board[downright.x, downright.y] == X) O_point += 10;
			return O_point;
		}

		// Các hàm đếm số quân liền nhau từ một tọa độ truyền vào
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
