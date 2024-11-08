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
        private readonly int[,] maze = {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 0, 1, 0, 1, 1, 0, 1 },
            { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 1, 0, 1 },
            { 1, 0, 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 },
            { 1, 0, 1, 0, 0, 0, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }
        };

        private List<Node> path;
        private Node startNode;
        private Node endNode;
        private int cellSize = 40;
        private int currentStep = 0;
        private System.Windows.Forms.Timer moveTimer;

        public MainForm()
        {
            //InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = maze.GetLength(1) * cellSize + 20;
            this.Height = maze.GetLength(0) * cellSize + 40;

            startNode = new Node(0, 1);
            endNode = new Node(8, 9);

            path = FindPath(startNode, endNode);

            moveTimer = new System.Windows.Forms.Timer();
            moveTimer.Interval = 500; // Интервал перемещения (мс)
            moveTimer.Tick += MoveAlongPath;
            moveTimer.Start();

            this.Paint += MainForm_Paint;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Отрисовка лабиринта
            for (int y = 0; y < maze.GetLength(0); y++)
            {
                for (int x = 0; x < maze.GetLength(1); x++)
                {
                    if (maze[y, x] == 1)
                        g.FillRectangle(Brushes.Black, x * cellSize, y * cellSize, cellSize, cellSize);
                    else
                        g.FillRectangle(Brushes.White, x * cellSize, y * cellSize, cellSize, cellSize);
                }
            }

            // Отрисовка пути
            foreach (var node in path)
            {
                g.FillRectangle(Brushes.LightBlue, node.X * cellSize, node.Y * cellSize, cellSize, cellSize);
            }

            if (currentStep < path.Count)
            {
                var currentNode = path[currentStep];
                g.FillRectangle(Brushes.Blue, currentNode.X * cellSize, currentNode.Y * cellSize, cellSize, cellSize);
            }

            // Начальная и конечная точки
            g.FillRectangle(Brushes.Green, startNode.X * cellSize, startNode.Y * cellSize, cellSize, cellSize);
            g.FillRectangle(Brushes.Red, endNode.X * cellSize, endNode.Y * cellSize, cellSize, cellSize);
        }

        private void MoveAlongPath(object sender, EventArgs e)
        {
            // Перемещаем игрока по пути
            if (currentStep < path.Count - 1)
            {
                currentStep++;
                Invalidate(); // Перерисовываем форму
            }
            else
            {
                moveTimer.Stop(); // Останавливаем таймер, если достигли конца пути
            }
        }

        private List<Node> FindPath(Node start, Node end)
        {
            var openList = new List<Node>();
            var closedList = new HashSet<Node>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                // Находим узел с наименьшей стоимостью F
                openList.Sort((node1, node2) => node1.F.CompareTo(node2.F));
                var currentNode = openList[0];

                // Если достигли цели, восстанавливаем путь
                if (currentNode.X == end.X && currentNode.Y == end.Y)
                {
                    return ReconstructPath(currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // Получаем соседей
                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighbor) || maze[neighbor.Y, neighbor.X] == 1)
                        continue;

                    int tentativeGScore = currentNode.G + 1;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                    else if (tentativeGScore >= neighbor.G)
                    {
                        continue;
                    }

                    // Обновляем данные узла
                    neighbor.Parent = currentNode;
                    neighbor.G = tentativeGScore;
                    neighbor.H = Math.Abs(neighbor.X - end.X) + Math.Abs(neighbor.Y - end.Y);
                }
            }

            return new List<Node>(); // Путь не найден
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

                if (newX >= 0 && newX < maze.GetLength(1) && newY >= 0 && newY < maze.GetLength(0))
                {
                    neighbors.Add(new Node(newX, newY));
                }
            }
            return neighbors;
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
