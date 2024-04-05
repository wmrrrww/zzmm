using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SnakeGame.MainWindow;

namespace zzmm
{
    /// <summary>
    /// Логика взаимодействия для Score.xaml
    /// </summary>
    public partial class Score : Window
    {
        public Score()
        {
            InitializeComponent();
        }

        public void UpdateScoreData(List<GameResult> results)
        {
            bestScoresDataGrid.ItemsSource = results;
        }

        private List<GameResult> bestResults = new List<GameResult>();

        public void UpdateScoreData(GameResult newResult)
        {
            bestResults.Add(newResult);
            bestResults = bestResults.OrderByDescending(r => r.Score).Take(10).ToList();
            bestScoresDataGrid.ItemsSource = bestResults;
        }




    }
}
