using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private short _turn = 0;
        private ImageBrush _crossImage = Utils.ImageToImageBrush("images/cross.png");
        private ImageBrush _circleImage = Utils.ImageToImageBrush("images/circle.png");
        public TicTacToeGame game;
        public MainWindow()
        {
            InitializeComponent();

            game = new TicTacToeGame();
        }

        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle field = (Rectangle)sender;
            if (game.MakeMove(int.Parse(field.Uid)))
            {
                field.Fill = game.CurrentPlayer == TicTacToeGame.Field.Cross ? _circleImage : _crossImage;
            }
            Thread.Sleep(1000);
            if (game.ComputerMove())
            {
                field.Fill = game.CurrentPlayer == TicTacToeGame.Field.Cross ? _circleImage : _crossImage;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }
    }
}
