using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace TicTacToe
{
    public class TicTacToeGame
    {
        public enum Field { Empty = 2, Cross = 0, Circle = 1 };

        public Field Player { get; private set; }
        public Field Opponent { get; private set; }
        public Field[] Board { get; private set; }

        private (int, int)[] _coords = new (int, int)[9] { (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 0) };

        public TicTacToeGame(Field firstPlayer=Field.Cross)
        {
            Board = new Field[9];
            for (int i = 0; i < 9; i++)
            {
                Board[i] = Field.Empty;
            }
            Player = firstPlayer;
            Opponent = Player == Field.Cross ? Field.Circle : Field.Cross;
        }

        public TicTacToeGame Copy()
        {
            TicTacToeGame copy = new TicTacToeGame();
            copy.Player = Player;
            copy.Opponent = Opponent;
            Board.CopyTo(copy.Board, 0);
            return copy;
        }

        public int MakeMove(int field)
        {
            Debug.WriteLine(field);
            if (Board[field] == Field.Empty)
            {
                Debug.WriteLine("Done");
                Board[field] = Player;
                (Player, Opponent) = (Opponent, Player);
                return field;
            }
            else return -1;
        }

        public int ComputerMove()
        {
            return MakeMove(FindBestMove(Board));
        }

        private int FindBestMove(Field[] board)
        {
            int max = int.MinValue;
            int bestMove = -1;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == Field.Empty)
                {
                    board[i] = Player;
                    int value = MiniMax(board, 0, false);
                    board[i] = Field.Empty;

                    if (value > max)
                    {
                        max = value;
                        bestMove = i;
                    }
                }
            }
            return bestMove;
        }
        private int MiniMax(Field[] board, int depth, bool isMaxing)
        {
            Field? res = IsGameOver(board);
            //Debug.WriteLine($"{depth} | {res} | {StringBoard(board)}");
            if (res != null)
            {
                if (res == Field.Empty) return 0;
                else if (res == Player) return 10 - depth;
                else return -10 + depth;
            }

            if (isMaxing)
            {
                int best = int.MinValue;
                for (int i = 0; i < board.Length; i++)
                {
                    if (board[i] == Field.Empty)
                    {
                        board[i] = Player;
                        best = Math.Max(best, MiniMax(board, depth + 1, !isMaxing));
                        board[i] = Field.Empty;
                    }
                }
                return best;
            }
            else
            {
                int best = int.MaxValue;
                for (int i = 0; i < board.Length; i++)
                {
                    if (board[i] == Field.Empty)
                    {
                        board[i] = Opponent;
                        best = Math.Min(best, MiniMax(board, depth + 1, !isMaxing));
                        board[i] = Field.Empty;
                    }
                }
                return best;
            }
        }
        public string StringBoard(Field[] board)
        {
            string s = "";
            foreach (Field f in board) s += f == Field.Empty ? "_" : f == Field.Cross ? "X" : "O";
            return s;
        }
        private Field? IsGameOver(Field[] board)
        {
            // Horizontal lines
            if (board[0] != Field.Empty && board[0] == board[1] && board[1] == board[2]) return board[0];
            if (board[3] != Field.Empty && board[3] == board[4] && board[4] == board[5]) return board[3];
            if (board[6] != Field.Empty && board[6] == board[7] && board[7] == board[8]) return board[6];
            // Vertical lines
            if (board[0] != Field.Empty && board[0] == board[3] && board[3] == board[6]) return board[0];
            if (board[1] != Field.Empty && board[1] == board[4] && board[4] == board[7]) return board[1];
            if (board[2] != Field.Empty && board[2] == board[5] && board[5] == board[8]) return board[2];
            // Diagonals
            if (board[0] != Field.Empty && board[0] == board[4] && board[4] == board[8]) return board[0];
            if (board[2] != Field.Empty && board[2] == board[4] && board[4] == board[6]) return board[2];

            bool isDraw = true;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == Field.Empty)
                {
                    isDraw = false;
                    break;
                }
            }
            return isDraw ? Field.Empty : null;
        }

        public Field? IsGameOver() => IsGameOver(Board);
    }     
}
