using MiniMax;

// See https://aka.ms/new-console-template for more information
Console.OutputEncoding = System.Text.Encoding.UTF8;

string[,] board1 = { { "X", " ", "X" }, 
                     { "X", " ", " " }, 
                     { "X", " ", " " } };

string[,] board2 = { { "X", " ", "X" }, 
                     { "X", "X", " " }, 
                     { "X", " ", " " } };

State state1 = new State(board1, null);
State state2 = new State(board2, (state1, new Point(1, 1), "X"));
//Console.WriteLine(MiniMax.MiniMax.countLine(state2, state2.pre.Value.Item2));
Game game = new Game();
game.startGame();