using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Class for playing TicTacToe being at the same time a ViewModel for the application.
    /// </summary>
    public class TicTacToeGame : INotifyPropertyChanged
    {
        // -------------------- Required element for data binding -------------------- 

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        // --------------------  Members and Properties -------------------- 

        /// <summary>
        /// Type representing content of a single field in TicTacToe game board.
        /// </summary>
        public enum Field { Empty = 2, Cross = 0, Circle = 1 };

        /// <summary>
        /// Active player
        /// </summary>
        public Field Player { get; private set; }
        /// <summary>
        /// Actuve opponent
        /// </summary>
        public Field Opponent { get; private set; }

        /// <summary>
        /// Private member containing info about board.
        /// </summary>
        private ObservableCollection<Field> _board;
        /// <summary>
        /// Property containing info about the board. It is binded to the fields in UI.
        /// </summary>
        public ObservableCollection<Field> Board { 
            get => _board; 
            set
            {
                _board = value;
                OnPropertyChanged(nameof(Board));
            }
        }


        // --------------------  Mehtods --------------------  
        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="firstPlayer">Sign of the first player</param>
        public TicTacToeGame(Field firstPlayer=Field.Cross)
        {
            this._board = new();
            for (int i = 0; i < 9; i++) _board.Add(Field.Empty);
            Player = firstPlayer;
            Opponent = Player == Field.Cross ? Field.Circle : Field.Cross;
        }

        /// <summary>
        /// Resets the board and players. Allows to play another game using the same object.
        /// </summary>
        /// <param name="firstPlayer">Sign of the first player</</param>
        public void ResetGame(Field firstPlayer=Field.Cross)
        {
            for (int i = 0; i < 9; i++)
            {
                Board[i] = Field.Empty;
            }
            Player = firstPlayer;
            Opponent = Player == Field.Cross ? Field.Circle : Field.Cross;
        }

        /// <summary>
        /// Makes a move of the active player.
        /// </summary>
        /// <param name="field">Number of field to place the sign</param>
        /// <returns>The field of move or -1 if the move was impossible.</returns>
        public int MakeMove(int field)
        {
            if (Board[field] == Field.Empty)
            {
                Board[field] = Player;
                (Player, Opponent) = (Opponent, Player);
                return field;
            }
            else return -1;
        }

        /// <summary>
        /// Makes a move based on the MiniMax algorithm for the active player.
        /// </summary>
        /// <returns>The field of move or -1 if the move was impossible.</returns>
        public int ComputerMove()
        {
            return MakeMove(FindBestMove(Board.ToArray()));
        }

        /// <summary>
        /// Find the best move for the active player on the given board.
        /// </summary>
        /// <param name="board">Board of the game</param>
        /// <returns>The field number of the best move.</returns>
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

        /// <summary>
        /// MiniMax algorithm for TicTacToe game.
        /// </summary>
        /// <param name="board">Board of the game</param>
        /// <param name="depth">Current depth of the algorithm</param>
        /// <param name="isMaxing">Whether the algorithm should maximize or minimize tha score</param>
        /// <returns></returns>
        private int MiniMax(Field[] board, int depth, bool isMaxing)
        {
            // Ending condition - the game has concluded
            Field? res = IsGameOver(board);
            if (res != null)
            {
                if (res == Field.Empty) return 0; // Zero points for the draw
                else if (res == Player) return 10 - depth; // Positive points for winning the game, penalized by the number of steps
                else return -10 + depth; // Negative the points for losing the game, rewarded by the number of steps
            }

            // Maximizing the score, when it's the player's turn
            if (isMaxing)
            {
                int best = int.MinValue;
                // Finding the best position by recurrence
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
            // Minimizing the score, when it's the opoonent's turn
            else
            {
                int best = int.MaxValue;
                // Finding the best position by recurrence
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

        /// <summary>
        /// Evaluates the game based on the board
        /// </summary>
        /// <param name="board"></param>
        /// <returns>Null if the game is not concluded, otherwise the winner or <c>Field.Empty</c> in the case of a draw.</returns>
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

        /// <summary>
        /// Evaluates the current game.
        /// </summary>
        /// <returns>Null if the game is not concluded, otherwise the winner or <c>Field.Empty</c> in the case of a draw.</returns>
        public Field? IsGameOver() => IsGameOver(Board.ToArray());
    }     
}
