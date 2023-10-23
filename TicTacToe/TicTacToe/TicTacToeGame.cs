using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Field CurrentPlayer { get; private set; }

        public int IsGameOver { 
            get
            {
                // Horizontal lines
                if (Board[0] == Board[1] && Board[1] == Board[2]) return 1;
                if (Board[3] == Board[4] && Board[4] == Board[5]) return 1;
                if (Board[6] == Board[7] && Board[7] == Board[8]) return 1;
                // Vertical lines
                if (Board[0] == Board[3] && Board[3] == Board[6]) return 1;
                if (Board[1] == Board[4] && Board[4] == Board[7]) return 1;
                if (Board[2] == Board[5] && Board[5] == Board[8]) return 1;
                // Diagonals
                if (Board[0] == Board[4] && Board[4] == Board[8]) return 1;
                if (Board[2] == Board[4] && Board[4] == Board[6]) return 1;

                bool isDraw = true;
                for (int i = 0; i < 9; i++)
                {
                    if (Board[i] == Field.Empty)
                    {
                        isDraw = false; 
                        break;
                    }
                }
                return isDraw ? 2 : 0; 
            } 
        }

        public Field[] Board { get; private set; }

        private (int, int)[] _coords = new (int, int)[9] { (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 0) };

        public TicTacToeGame(Field firstPlayer=Field.Cross)
        {
            Board = new Field[9];
            for (int i = 0; i < 9; i++)
            {
                Board[i] = Field.Empty;
            }
            CurrentPlayer = firstPlayer;
        }

        public TicTacToeGame Copy()
        {
            TicTacToeGame copy = new TicTacToeGame();
            copy.CurrentPlayer = CurrentPlayer;
            Board.CopyTo(copy.Board, 0);
            return copy;
        }

        public bool MakeMove(int field)
        {
            Debug.WriteLine(field);
            if (Board[field] == Field.Empty)
            {
                Board[field] = CurrentPlayer;
                CurrentPlayer = CurrentPlayer == Field.Cross ? Field.Circle : Field.Cross;
                return true;
            }
            else return false;
        }

        public bool ComputerMove()
        {
            MinMax ai = new MinMax(this);
            ai.Calculate();
            return MakeMove(ai.BestChoice);
        }

        private class MinMax
        {
            private Dictionary<int, int> _scores;
            private TicTacToeGame _startGame;
            private Field _currentPlayer;
            private int _depth = 0;
            public int BestChoice;

            public MinMax(TicTacToeGame game)
            {
                _startGame = game;
                _currentPlayer = _startGame.CurrentPlayer;
                _scores = new Dictionary<int, int>();
            }

            private int GetScore(TicTacToeGame game, int depth)
            {
                if (game.IsGameOver > 0)
                {
                    if (_currentPlayer == game.CurrentPlayer) return -10 + depth;
                    else return 10 - depth;
                }
                else return 0;
            }

            public void Calculate()
            {
                MinMaxAlgorithm(_startGame, 0);
            }

            private int MinMaxAlgorithm(TicTacToeGame game, int depth)
            {
                if (game.IsGameOver) return GetScore(game, depth);

                List<int> scores = new();
                List<int> moves = new();

                for (int i = 0; i < 9; i++)
                {
                    if (game.Board[i] == Field.Empty)
                    {
                        var nextState = game.Copy();
                        nextState.MakeMove(i);
                        moves.Add(i);
                        scores.Add(MinMaxAlgorithm(nextState, depth + 1));
                    }
                }

                if (game.CurrentPlayer == _currentPlayer)
                {
                    int maxScoreIndex = scores.IndexOf(scores.Max());
                    BestChoice = moves[maxScoreIndex];
                    return scores[maxScoreIndex];
                }
                else
                {
                    int minScoreIndex = scores.IndexOf(scores.Min());
                    BestChoice = moves[minScoreIndex];
                    return scores[minScoreIndex];
                }
            }
        }
    }     
}
