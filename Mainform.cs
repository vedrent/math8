using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LabyrinthAStar
{
    public class Node
    {
        public int X { get; }
        public int Y { get; }
        public int G { get; set; } // стоимость от начальной точки до текущей
        public int H { get; set; } // эвристическая стоимость (Манхэттенское расстояние)
        public int F => G + H; // полная стоимость

        public Node Parent { get; set; }

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public partial class MainForm : Form
    {
        private readonly int[,,] mazes = {
            // maze0
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 0, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1 },
                { 1, 2, 1, 0, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 1, 3, 0, 0, 1, 0, 0, 1 },
                { 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1, 0, 1, 1 },
                { 1, 0, 2, 0, 1, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }
            },
            // maze1
            {
                { 1, 1, 1, 1, 3, 1, 0, 1, 1, 1 },
                { 0, 0, 0, 0, 0, 0, 2, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 1, 1, 1, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 1, 1, 1, 1, 0, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            },
            // maze2
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 0, 1, 0 },
                { 0, 1, 0, 0, 0, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 1, 1, 1, 0, 1, 0 },
                { 0, 1, 0, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            },
        };

        private (int x, int y, int Maze, int targetX, int targetY, int targetMaze)[] teleporters = {
            (8, 9, 0, 0, 1, 1),  // Телепорт из maze0 в maze1
            (8, 8, 1, 0, 0, 2),  // Телепорт из maze1 в maze2
            (5, 5, 2, 0, 1, 0)   // Телепорт из maze2 в maze0
        };

        private readonly List<MovingObstacle> movingObstacles = new List<MovingObstacle>
        {
            new MovingObstacle(4, 0, 3, 1, false),
            new MovingObstacle(6, 1, 2, 1, false),
        };

        private List<Node> path;
        private Node startNode;
        private Node endNode;
        private int cellSize = 40;
        private int currentStep = 0;
        private System.Windows.Forms.Timer moveTimer;
        private int currentLevel = 0;
        private int isSlowed = 0;
        private System.Windows.Forms.Timer obstacleTimer;
        private List<Node> playerPath = new List<Node>();

        public MainForm()
        {
            //InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = mazes.GetLength(2) * cellSize + 20;
            this.Height = mazes.GetLength(1) * cellSize + 40;

            obstacleTimer = new System.Windows.Forms.Timer();
            obstacleTimer.Interval = 300; // Интервал обновления (мс)
            obstacleTimer.Tick += UpdateObstacles;
            obstacleTimer.Start();

            startNode = new Node(0, 1);
            endNode = new Node(teleporters[0].x, teleporters[0].y);

            path = FindPath(startNode, GetNextTarget());

            moveTimer = new System.Windows.Forms.Timer();
            moveTimer.Interval = 100; // Интервал перемещения (мс)
            moveTimer.Tick += MoveAlongPath;
            moveTimer.Start();

            this.Paint += MainForm_Paint;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Отрисовка лабиринта
            for (int y = 0; y < mazes.GetLength(1); y++)
            {
                for (int x = 0; x < mazes.GetLength(2); x++)
                {
                    if (mazes[currentLevel, y, x] == 1)
                        g.FillRectangle(Brushes.Black, x * cellSize, y * cellSize, cellSize, cellSize);
                    else if (mazes[currentLevel, y, x] == 2)
                        g.FillRectangle(Brushes.Orange, x * cellSize, y * cellSize, cellSize, cellSize);
                    else if (mazes[currentLevel, y, x] == 3)
                        g.FillRectangle(Brushes.Green, x * cellSize, y * cellSize, cellSize, cellSize); // Отбрасывающие препятствия
                    else
                        g.FillRectangle(Brushes.White, x * cellSize, y * cellSize, cellSize, cellSize);
                }
            }

            // Отрисовка пути
            //foreach (var node in path)
            //{
            //    g.FillRectangle(Brushes.LightBlue, node.X * cellSize, node.Y * cellSize, cellSize, cellSize);
            //}

            if (currentStep < path.Count)
            {
                var currentNode = path[currentStep];
                g.FillRectangle(Brushes.Blue, currentNode.X * cellSize, currentNode.Y * cellSize, cellSize, cellSize);
            }

            foreach (var teleport in teleporters)
            {
                if (teleport.Maze == currentLevel)
                {
                    g.FillRectangle(Brushes.Purple, teleport.x * cellSize, teleport.y * cellSize, cellSize, cellSize);
                }
            }

        }

        private void MoveAlongPath(object sender, EventArgs e)
        {
            // Перемещаем игрока по пути
            if (currentStep < path.Count - 1)
            {
                currentStep++;
                var currentNode = path[currentStep];
                startNode = currentNode;
                playerPath.Add(new Node(startNode.X, startNode.Y)); // Добавляем текущую позицию в путь игрока

                if (mazes[currentLevel, startNode.Y, startNode.X] == 3)
                {
                    KnockBackPlayer();
                }

                if (mazes[currentLevel, currentNode.Y, currentNode.X] == 2)
                {
                    if (isSlowed == 0)
                    {
                        moveTimer.Interval = moveTimer.Interval * 3;
                    }
                    isSlowed = 4;
                }

                switch (isSlowed)
                {
                    case >1:
                        isSlowed--;
                        break;
                    case 1:
                        isSlowed--;
                        moveTimer.Interval = moveTimer.Interval / 3;
                        break;
                    case 0: break;
                }

                if (teleporters[currentLevel].x == currentNode.X && teleporters[currentLevel].y == currentNode.Y)
                {
                    currentNode = new Node(teleporters[currentLevel].targetX, teleporters[currentLevel].targetY);
                    currentLevel = teleporters[currentLevel].targetMaze;
                    path = FindPath(currentNode, GetNextTarget());
                    currentStep = 0;
                    playerPath.Clear();
                }
                Invalidate(); // Перерисовываем форму
            }
            else
            {
                moveTimer.Stop(); // Останавливаем таймер, если достигли конца пути
            }
        }

        private Node GetNextTarget()
        {
            foreach (var teleport in teleporters)
            {
                if (teleport.Maze == currentLevel)
                {
                    return new Node(teleport.x, teleport.y);
                }
            }
            return endNode;
        }

        private List<Node> FindPath(Node start, Node end)
        {
            var openList = new SortedSet<Node>(Comparer<Node>.Create((a, b) =>
        a.G == b.G ? (a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X)) : a.G.CompareTo(b.G)));
            var closedList = new HashSet<Node>();

            start.G = 0;
            openList.Add(start);

            while (openList.Count > 0)
            {
                // Находим узел с наименьшей стоимостью G
                var currentNode = openList.Min;
                openList.Remove(currentNode);

                // Если достигли цели, восстанавливаем путь
                if (currentNode.X == end.X && currentNode.Y == end.Y)
                {
                    return ReconstructPath(currentNode);
                }

                closedList.Add(currentNode);

                // Получаем соседей
                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighbor) || mazes[currentLevel, neighbor.Y, neighbor.X] == 1)
                        continue;

                    int moveCost = (mazes[currentLevel, neighbor.Y, neighbor.X] == 2) ? 3 : 1;
                    int tentativeGScore = currentNode.G + moveCost;

                    if (!openList.Contains(neighbor))
                    {
                        neighbor.Parent = currentNode;
                        neighbor.G = tentativeGScore;
                        openList.Add(neighbor);
                    }
                    else if (tentativeGScore >= neighbor.G)
                    {
                        openList.Remove(neighbor);
                        neighbor.G = tentativeGScore;
                        neighbor.Parent = currentNode;
                        openList.Add(neighbor);
                    }
                }
            }

            return new List<Node>();
        }

        private List<Node> ReconstructPath(Node node)
        {
            var path = new List<Node>();
            while (node != null)
            {
                path.Add(node);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }

        private IEnumerable<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();
            var directions = new (int dx, int dy)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };

            foreach (var (dx, dy) in directions)
            {
                int newX = node.X + dx;
                int newY = node.Y + dy;

                if (newX >= 0 && newX < mazes.GetLength(2) && newY >= 0 && newY < mazes.GetLength(1))
                {
                    neighbors.Add(new Node(newX, newY));
                }
            }
            return neighbors;
        }

        public class MovingObstacle
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Type { get; } // Тип препятствия (2 - замедляющее, 3 и 4 - другие)
            public int Level { get; } // Уровень, на котором находится препятствие
            public int Direction { get; set; }
            public bool MovesHorizontally { get; } // Направление движения: true — горизонтально, false — вертикально

            public MovingObstacle(int x, int y, int type, int level, bool movesHorizontally)
            {
                X = x;
                Y = y;
                Type = type;
                Level = level;
                Direction = 1;
                MovesHorizontally = movesHorizontally;
            }
        }

        private void UpdateObstacles(object sender, EventArgs e)
        {
            foreach (var obstacle in movingObstacles)
            {
                int newX = obstacle.X;
                int newY = obstacle.Y;

                // Определяем новое положение в зависимости от направления
                if (obstacle.MovesHorizontally)
                {
                    newX += obstacle.Direction;
                    // Проверяем границы и столкновения со стеной
                    if (newX < 0 || newX >= mazes.GetLength(2) || mazes[obstacle.Level, obstacle.Y, newX] == 1)
                    {
                        obstacle.Direction *= -1;
                        newX = obstacle.X + obstacle.Direction;
                    }
                }
                else
                {
                    newY += obstacle.Direction;
                    // Проверяем границы и столкновения со стеной
                    if (newY < 0 || newY >= mazes.GetLength(1) || mazes[obstacle.Level, newY, obstacle.X] == 1)
                    {
                        obstacle.Direction *= -1;
                        newY = obstacle.Y + obstacle.Direction;
                    }
                }

                // Обновляем позицию в массиве
                if (mazes[obstacle.Level, obstacle.Y, obstacle.X] == obstacle.Type)
                    mazes[obstacle.Level, obstacle.Y, obstacle.X] = 0; // Старая позиция

                obstacle.X = newX;
                obstacle.Y = newY;
                mazes[obstacle.Level, newY, newX] = obstacle.Type; // Новая позиция
            }

            Invalidate(); // Перерисовываем форму
        }

        private void KnockBackPlayer()
        {
            // Проверка, достаточно ли шагов для отбрасывания назад
            int stepsBack = Math.Min(4, playerPath.Count);
            if (stepsBack > 0)
            {
                // Устанавливаем позицию игрока на три шага назад
                var backNode = playerPath[playerPath.Count - stepsBack];
                startNode = backNode;

                // Удаляем последние три шага из пути игрока
                playerPath.RemoveRange(playerPath.Count - stepsBack, stepsBack);

                path = FindPath(startNode, new Node(teleporters[currentLevel].x, teleporters[currentLevel].y));
                currentStep = 0;

                // Обновляем отрисовку после отбрасывания
                Invalidate();
            }
        }


        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
