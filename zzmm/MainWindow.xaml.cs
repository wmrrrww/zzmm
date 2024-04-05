using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using zzmm;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private const int CellSize = 20;
        private const int Rows = 20;
        private const int Columns = 20;
        private readonly SolidColorBrush SnakeBrush = Brushes.Violet;
        private readonly SolidColorBrush FoodBrush = Brushes.Yellow;
        private readonly SolidColorBrush BackgroundBrush = Brushes.Gray;
        private readonly SolidColorBrush WallBrush = Brushes.Black;

        private readonly List<Point> walls = new List<Point>();
        private readonly List<Point> snake = new List<Point>();

        private enum Direction { Up, Down, Left, Right };
        private Direction snakeDirection = Direction.Right;

        private Point food;
        private int score = 0;
        private int bestScore = 0;

        private DispatcherTimer timer;
        private bool isPaused = false;

        private const int WallsCount = 5;

        public MainWindow()
        {


            InitializeComponent();
            LoadBestScore();
            scoreWindow = new Score();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            snake.Clear();
            snake.Add(new Point(5, 5));
            score = 0;
            isPaused = false;
            GenerateFood();
            DrawGame();
            StartGame();
            UpdateScore();
            GenerateWalls();
        }

        private void GenerateWalls()
        {
            Random rand = new Random();
            for (int i = 0; i < WallsCount; i++)
            {
                int x, y;
                do
                {
                    x = rand.Next(1, Columns - 1); // исключаем крайние столбцы 
                    y = rand.Next(1, Rows - 1);    // исключаем крайние строки 
                } while (snake.Contains(new Point(x, y)) || walls.Contains(new Point(x, y))); // стены не пересекались с змейкой или друг с другом

                walls.Add(new Point(x, y));
            }
        }

        private void StartGame()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(400);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                MoveSnake();
                CheckCollision();
                DrawGame();
            }
        }

        private void MoveSnake()
        {
            // сохранение последней позиции хвоста 
            Point tail = snake[snake.Count - 1];
            for (int i = snake.Count - 1; i > 0; i--)
                snake[i] = snake[i - 1];

            switch (snakeDirection)
            {
                case Direction.Up:
                    snake[0] = new Point(snake[0].X, snake[0].Y - 1);
                    break;
                case Direction.Down:
                    snake[0] = new Point(snake[0].X, snake[0].Y + 1);
                    break;
                case Direction.Left:
                    snake[0] = new Point(snake[0].X - 1, snake[0].Y);
                    break;
                case Direction.Right:
                    snake[0] = new Point(snake[0].X + 1, snake[0].Y);
                    break;
            }

            if (snake[0] == food)
            {
                score++;
                UpdateScore();
                GenerateFood();
                snake.Add(tail);
            }
        }

        private void CheckCollision()
        {
            if (snake[0].X < 0 || snake[0].X >= Columns || snake[0].Y < 0 || snake[0].Y >= Rows)
                EndGame();

            for (int i = 1; i < snake.Count; i++)
            {
                if (snake[0] == snake[i])
                {
                    EndGame();
                    break;
                }
            }
            if (walls.Contains(snake[0]))
            {
                EndGame();
                return;
            }
        }

        private void GenerateFood()
        {
            Random rand = new Random();
            int x, y;
            do
            {
                x = rand.Next(0, Columns);
                y = rand.Next(0, Rows);
            } while (snake.Contains(new Point(x, y))); // еда не появляется на змейке

            food = new Point(x, y);
        }

        private void DrawGame()
        {
            canvas.Children.Clear();

            Rectangle bg = new Rectangle
            {
                Width = Columns * CellSize,
                Height = Rows * CellSize,
                Fill = BackgroundBrush
            };
            canvas.Children.Add(bg);

            foreach (Point wall in walls)
            {
                Rectangle wallPiece = new Rectangle
                {
                    Width = CellSize,
                    Height = CellSize,
                    Fill = WallBrush
                };
                Canvas.SetLeft(wallPiece, wall.X * CellSize);
                Canvas.SetTop(wallPiece, wall.Y * CellSize);
                canvas.Children.Add(wallPiece);
            }

            foreach (Point p in snake)
            {
                Rectangle snakePart = new Rectangle
                {
                    Width = CellSize,
                    Height = CellSize,
                    Fill = SnakeBrush
                };
                Canvas.SetLeft(snakePart, p.X * CellSize);
                Canvas.SetTop(snakePart, p.Y * CellSize);
                canvas.Children.Add(snakePart);
            }

            Rectangle foodPiece = new Rectangle
            {
                Width = CellSize,
                Height = CellSize,
                Fill = FoodBrush
            };
            Canvas.SetLeft(foodPiece, food.X * CellSize);
            Canvas.SetTop(foodPiece, food.Y * CellSize);
            canvas.Children.Add(foodPiece);

            if (score > bestScore)
            {
                bestScore = score;
                SaveBestScore(); // сохраняем новый лучший результат
            }
            bestScoreLabel.Content = "Best Score: " + bestScore;
        }

        private void SaveBestScore()
        {
            try
            {
                // сохраняем лучший результат в файл
                File.WriteAllText("bestscore.txt", bestScore.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving best score: " + ex.Message);
            }
        }

        private void LoadBestScore()
        {
            try
            {
                // загружаем лучший результат из файла
                if (File.Exists("bestscore.txt"))
                {
                    bestScore = int.Parse(File.ReadAllText("bestscore.txt"));
                    bestScoreLabel.Content = "Best Score: " + bestScore;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading best score: " + ex.Message);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (snakeDirection != Direction.Down)
                        snakeDirection = Direction.Up;
                    break;
                case Key.Down:
                    if (snakeDirection != Direction.Up)
                        snakeDirection = Direction.Down;
                    break;
                case Key.Left:
                    if (snakeDirection != Direction.Right)
                        snakeDirection = Direction.Left;
                    break;
                case Key.Right:
                    if (snakeDirection != Direction.Left)
                        snakeDirection = Direction.Right;
                    break;
                case Key.Space:
                    isPaused = !isPaused;
                    pauseButton.Content = isPaused ? "Resume" : "Pause";
                    break;
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;
            pauseButton.Content = isPaused ? "Resume" : "Pause";
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UpdateScore()
        {
            scoreLabel.Content = "Score: " + score;
        }

        //private void EndGame()
        //{
        //    timer.Stop();
        //    MessageBox.Show("Game Over! Your score: " + score);
        //}

        public class GameResult
        {
            public string Name { get; set; }
            public int Score { get; set; }

            public GameResult(string name, int score)
            {
                Name = name;
                Score = score;
            }
        }

        private string GenerateRandomName()
        {
            Random rand = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, 3).Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        private void SaveBestScores()
        {
            try
            {
                // Создаем экземпляр окна Score
                Score scoreWindow = new Score();

                // Сохраняем результаты игры в файл
                List<GameResult> results = new List<GameResult>();
                results.Add(new GameResult(GenerateRandomName(), score)); // Генерируем случайное имя
                                                                          // Добавьте сюда остальную логику сохранения результатов

                // Выводим результаты игры в порядке убывания счета
                results = results.OrderByDescending(r => r.Score).ToList();

                // Записываем результаты в DataGrid в Score.xaml
                scoreWindow.bestScoresDataGrid.ItemsSource = results;

                // Показываем окно Score
                scoreWindow.Show();

                // Сохраняем результаты игры в файл
                // Не забудьте изменить эту часть кода в соответствии с вашей логикой сохранения данных
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving best scores: " + ex.Message);
            }
        }

        private void ViewBestScore_Click(object sender, RoutedEventArgs e)
        {
            Score scoreWindow = new Score();
            scoreWindow.Show();
        }

        private Score scoreWindow;




        private void EndGame()
        {
            timer.Stop();
            MessageBox.Show("Game Over! Your score: " + score);

            // Создаем новый результат игры
            GameResult newResult = new GameResult(GenerateRandomName(), score);

            // Обновляем данные в окне Score
            scoreWindow.UpdateScoreData(newResult);

            // Отображаем окно Score
            scoreWindow.Show();
        }







    }
}
