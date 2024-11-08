using System;
using System.Drawing;
using System.Windows.Forms;

namespace math8
{
    public partial class MainForm : Form
    {
        private int currentMazeIndex = 0;
        private int[,,] mazes = new int[3, 10, 10]
        {
            { // maze0
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
            },
            { // maze1 (создаваемый вручную)
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            },
            { // maze2
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0, 1 },
                { 1, 1, 0, 0, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 0, 1, 1, 1, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }
            }
        };
        //private int[,] maze1 = new int[10, 10];
        //private int[,] maze1 = {
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        //                      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        //                      };

        //private int[,] maze0 = {
        //                      { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        //                      { 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        //                      { 1, 1, 1, 0, 1, 0, 1, 1, 0, 1 },
        //                      { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1 },
        //                      { 1, 0, 1, 0, 1, 1, 1, 1, 0, 1 },
        //                      { 1, 0, 1, 0, 0, 0, 0, 0, 0, 1 },
        //                      { 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 },
        //                      { 1, 0, 1, 0, 0, 0, 1, 1, 1, 1 },
        //                      { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        //                      { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }
        //                      };

        private int playerX = 0, playerY = 1;
        private bool editMode = false;

        private (int x, int y, int Maze, int targetX, int targetY, int targetMaze)[] teleporters = {
            (8, 9, 0, 1, 1, 1),  // Телепорт из maze0 в maze1
            (8, 8, 1, 0, 1, 2),  // Телепорт из maze1 в maze2
            (6, 1, 2, 0, 1, 0)   // Телепорт из maze2 в maze0
        };

        public MainForm()
        {
            //InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.Paint += new PaintEventHandler(OnPaint);
            this.Width = 450;
            this.Height = 450;
            //pathMap = new Bitmap(400, 400);
            //pathForm = new PathForm(pathMap);
            //pathForm.Show();
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            int newX = playerX;
            int newY = playerY;

            if (e.KeyCode == Keys.Q)
            {
                editMode = !editMode;

                if (!editMode)
                {
                    //Array.Copy(maze1, maze0, maze1.Length);
                }

                playerX = 0;
                playerY = 1;
                Invalidate();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Up: newY--; break;
                case Keys.Down: newY++; break;
                case Keys.Left: newX--; break;
                case Keys.Right: newX++; break;
                case Keys.Enter:
                    if (editMode && mazes[currentMazeIndex, playerY, playerX] == 0)
                        mazes[currentMazeIndex, playerY, playerX] = 1;
                    break;
            }
            if (newX >= 0 && newX < mazes.GetLength(2) && newY >= 0 && newY <
           mazes.GetLength(1))
            {
                if (editMode || (!editMode && mazes[currentMazeIndex, newY, newX] == 0)) {
                    playerX = newX;
                    playerY = newY;
                }
                //using (Graphics g = Graphics.FromImage(pathMap))
                //{
                //    g.FillRectangle(Brushes.Red, playerX * 40, playerY * 40, 40, 40);
                //}
                
                //pathForm.Invalidate();
            }
            CheckTeleport();
            Invalidate();
        }
        private void CheckTeleport()
        {
            foreach (var teleporter in teleporters)
            {
                if (teleporter.x == playerX && teleporter.y == playerY && currentMazeIndex == teleporter.Maze)
                {
                    playerX = teleporter.targetX;
                    playerY = teleporter.targetY;
                    currentMazeIndex = teleporter.targetMaze;
                    break;
                }
            }
        }
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int cellSize = 40;
            //var maze = editMode ? maze1 : maze0;

            for (int y = 0; y < mazes.GetLength(1); y++)
            {
                for (int x = 0; x < mazes.GetLength(2); x++)
                {
                    if (mazes[currentMazeIndex, y, x] == 1)
                    {
                        g.FillRectangle(Brushes.Black, x * cellSize, y * cellSize,
                       cellSize, cellSize);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.White, x * cellSize, y * cellSize,
                       cellSize, cellSize);
                    }
                    if (x == playerX && y == playerY)
                    {
                        g.FillRectangle(Brushes.Blue, x * cellSize, y * cellSize,
                       cellSize, cellSize);
                    }
                    foreach (var teleporter in teleporters)
                    {
                        if (teleporter.x == x && teleporter.y == y && currentMazeIndex == teleporter.Maze)
                            g.FillRectangle(Brushes.Purple, x * cellSize, y * cellSize, cellSize, cellSize);
                    }
                }
            }
        }
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}
