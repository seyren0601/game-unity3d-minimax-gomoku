using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniMax.MiniMax;

namespace MiniMax
{
    internal class Game
    {
        enum Turn
        {
            XTurn,
            OTurn
        }

        static State currentState { get; set; } = new State() { };

        public void startGame()
        {
            Turn currentTurn = Turn.XTurn;
            Result result = Result.Pending;
            do
            {
                Console.Clear();
                currentState.printState();
                Console.WriteLine();
                if (currentTurn == Turn.XTurn)
                {
                    Console.WriteLine("X chọn vị trí đặt quân: ");
                    string[] input = Console.ReadLine().Split(" ");
                    Point move = new Point(int.Parse(input[0]), int.Parse(input[1]));
                    string[,] newBoard = (string[,])currentState.board.Clone();
                    if (isValid(move)) 
                    {
                        newBoard[move.x, move.y] = "X";
                        currentState = new State(newBoard, (currentState, move, "X"));
                        currentTurn = Turn.OTurn;
                    }
                    else
                    {
                        Console.WriteLine("Nước đi không hợp lệ, chọn lại!");
                        continue;
                    }
                }
                else
                {
                    // Player play
                    /*Console.WriteLine("O chọn vị trí đặt quân: ");
                    string[] input = Console.ReadLine().Split(" ");
                    Point move = new Point(int.Parse(input[0]), int.Parse(input[1]));
                    string[,] newBoard = (string[,])currentState.board.Clone();
                    if (isValid(move))
                    {
                        newBoard[move.x, move.y] = "O";
                        currentState = new State(newBoard, (currentState, move, "O"));
                        currentTurn = Turn.XTurn;
                    }
                    else
                    {
                        Console.WriteLine("Nước đi không hợp lệ, chọn lại!");
                        continue;
                    }*/

                    //Auto play
                    Point move = AutoPlay_GetMove(currentState, "O");
                    string[,] newBoard = (string[,])currentState.board.Clone();
                    newBoard[move.x, move.y] = "O";
                    currentState = new State(newBoard, (currentState, move, "O"));
                    currentTurn = Turn.XTurn;
                }
                result = CurrentState(currentState);
            } while (result == Result.Pending);
            Console.Clear();
            currentState.printState();
            if (result == Result.Draw) Console.WriteLine("HÒA");
            else if (result == Result.XWin) Console.WriteLine("X THẮNG");
            else Console.WriteLine("O THẮNG");
        }

        public bool isValid(Point point)
        {
            if (point.x < 0 || point.y < 0 || point.x == BOARD_SIZE || point.y == BOARD_SIZE || currentState.board[point.x, point.y] != " ") return false;
            return true;
        }

        internal class GameTree
        {
            public Node Root { get; set; }

            public List<Node> GetAllMoves(Node node)
            {
                List<Node> Moves = new List<Node>();
                string current_type = node.info.pre.Value.Item3 == "X" ? "O" : "X";
                string[,] board = node.info.board;
                for(int i = 0; i < BOARD_SIZE; i++)
                {
                    for(int j=0; j < BOARD_SIZE; j++)
                    {
                        if (board[i, j] == " ")
                        {
                            Point point = new Point (i, j);
                            string[,] newboard = (string[,])board.Clone();
                            newboard[i, j] = current_type;
                            State newState = new State(newboard, (node.info, new Point(i, j), current_type));
                            Moves.Add(new Node(newState, node, node.depth + 1));
                        }
                    }
                }
                return Moves;
            }
        }

        internal class Node
        {
            public State info;
            public Node? parent;
            public int score;
            public int depth;
            public List<Node> children = new List<Node>();
            public Node(State state, Node? parent, int depth)
            {
                info = state;
                this.parent = parent;
                this.depth = depth;
            }
        }
    }
}
