using System;
using System.Drawing;
using System.Windows.Forms;

namespace math8
{
    public partial class MainForm : Form
    {
        private int[,] maze1 = new int[10, 10];
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

        private int[,] maze0 = {
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

        private int playerX = 0, playerY = 1;
        private bool editMode = true;
        //private Bitmap pathMap;
        //private PathForm pathForm;
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
                    Array.Copy(maze1, maze0, maze1.Length);
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
                    if (editMode && maze1[playerY, playerX] == 0)
                        maze1[playerY, playerX] = 1;
                    break;
            }
            if (newX >= 0 && newX < maze0.GetLength(1) && newY >= 0 && newY <
           maze0.GetLength(0))
            {
                if (editMode || (!editMode && maze0[newY, newX] == 0)) {
                    playerX = newX;
                    playerY = newY;
                }
                //using (Graphics g = Graphics.FromImage(pathMap))
                //{
                //    g.FillRectangle(Brushes.Red, playerX * 40, playerY * 40, 40, 40);
                //}
                
                //pathForm.Invalidate();
            }
            Invalidate();
        }
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int cellSize = 40;
            var maze = editMode ? maze1 : maze0;

            for (int y = 0; y < maze.GetLength(0); y++)
            {
                for (int x = 0; x < maze.GetLength(1); x++)
                {
                    if (maze[y, x] == 1)
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
                    if (!editMode && x == 8 && y == 9)
                    {
                        g.FillRectangle(Brushes.Green, x * cellSize, y * cellSize,
                       cellSize, cellSize);
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
