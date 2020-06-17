using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Laba_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private class Point
        {
            public float x, y, z, h;
        }

        private class Segment
        {
            public int first, second;
        }


        List<Point> pointsList = new List<Point>();
        List<Segment> segmentsList = new List<Segment>();

        
        private void Build()
        {
            Graphics canvas = pictureBox1.CreateGraphics();
            canvas.Clear(Color.White);
            canvas.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            canvas.ScaleTransform(1, -1);
            Pen thePen = new Pen(Color.Black);

            foreach (Segment seg in segmentsList)
            {
                Point first = pointsList[seg.first - 1];
                Point second = pointsList[seg.second - 1];

                canvas.DrawLine(thePen, (first.x / first.h), (first.y / first.h),
                                        (second.x / second.h), (second.y / second.h));
            }
        }

        private List<Point> PointsTransform(List<Point> list, float[,] matrix)
        {
            List<Point> result = new List<Point>();

            foreach (Point dot in list)
            {
                Point dotTransformed = new Point
                {
                    x = dot.x * matrix[0, 0] + dot.y * matrix[1, 0] + dot.z * matrix[2, 0] + dot.h * matrix[3, 0],
                    y = dot.x * matrix[0, 1] + dot.y * matrix[1, 1] + dot.z * matrix[2, 1] + dot.h * matrix[3, 1],
                    z = dot.x * matrix[0, 2] + dot.y * matrix[1, 2] + dot.z * matrix[2, 2] + dot.h * matrix[3, 2],
                    h = dot.x * matrix[0, 3] + dot.y * matrix[1, 3] + dot.z * matrix[2, 3] + dot.h * matrix[3, 3]
                };
                result.Add(dotTransformed);
            }

            return result;
        }

        // Загрузить фигуру
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            string figure = dialog.FileName;
            StreamReader reader = new StreamReader(figure);

            pointsList.Clear();
            segmentsList.Clear();

            int pointsNum = Convert.ToInt32(reader.ReadLine());
            for (int i = 0; i < pointsNum; i++)
            {
                string line = reader.ReadLine();
                string[] coords = line.Split(' ');
                Point dot = new Point
                {
                    x = float.Parse(coords[0]),
                    y = float.Parse(coords[1]),
                    z = float.Parse(coords[2]),
                    h = float.Parse(coords[3])
                };
                pointsList.Add(dot);
            }

            int segmentsNum = Convert.ToInt32(reader.ReadLine());
            for (int i = 0; i < segmentsNum; i++)
            {
                string line = reader.ReadLine();
                string[] ends = line.Split(' ');
                Segment seg = new Segment
                {
                    first = Convert.ToInt32(ends[0]),
                    second = Convert.ToInt32(ends[1])
                };
                segmentsList.Add(seg);
            }

            reader.Close();
            MessageBox.Show("Фигура загружена");
        }

        // Построить фигуру
        private void button2_Click(object sender, EventArgs e)
        {
            Build();
        }

        // Параллельный перенос
        private void button3_Click(object sender, EventArgs e)
        {
            float aX = float.Parse(textBox1.Text);
            float aY = float.Parse(textBox2.Text);
            float aZ = float.Parse(textBox3.Text);

            float[,] matrix = new float[,]
            {
                {1, 0, 0, 0},
                {0, 1, 0, 0},
                {0, 0, 1, 0},
                {aX, aY, aZ, 1}
            };

            pointsList = PointsTransform(pointsList, matrix);
            Build();
        }

        // Масштабирование
        private void button4_Click(object sender, EventArgs e)
        {
            float kX = float.Parse(textBox4.Text);
            float kY = float.Parse(textBox5.Text);
            float kZ = float.Parse(textBox6.Text);

            float[,] matrix = new float[,]
            {
                {kX, 0, 0, 0},
                {0, kY, 0, 0},
                {0, 0, kZ, 0},
                {0, 0, 0, 1}
            };

            pointsList = PointsTransform(pointsList, matrix);
            Build();
        }

        // Поворот
        private void button5_Click(object sender, EventArgs e)
        {
            float alX = float.Parse(textBox7.Text) * (float)Math.PI / 180;
            float alY = float.Parse(textBox8.Text) * (float)Math.PI / 180;
            float alZ = float.Parse(textBox9.Text) * (float)Math.PI / 180;

            if (alX != 0)
            {
                float[,] matrixX = new float[,]
                {
                    {1, 0, 0, 0},
                    {0, (float)Math.Cos(alX), (float)Math.Sin(alX), 0},
                    {0, -(float)Math.Sin(alX), (float)Math.Cos(alX), 0},
                    {0, 0, 0, 1}
                };

                pointsList = PointsTransform(pointsList, matrixX);
            }

            if (alY != 0)
            {
                float[,] matrixY = new float[,]
                {
                    {(float)Math.Cos(alY), 0, -(float)Math.Sin(alY), 0},
                    {0, 1, 0, 0},
                    {(float)Math.Sin(alY), 0, (float)Math.Cos(alY), 0},
                    {0, 0, 0, 1}
                };

                pointsList = PointsTransform(pointsList, matrixY);
            }

            if (alZ != 0)
            {
                float[,] matrixZ = new float[,]
                {
                    {(float)Math.Cos(alZ), (float)Math.Sin(alZ), 0, 0},
                    {-(float)Math.Sin(alZ), (float)Math.Cos(alZ), 0, 0},
                    {0, 0, 1, 0},
                    {0, 0, 0, 1}
                };

                pointsList = PointsTransform(pointsList, matrixZ);
            }

            Build();
        }

        // ОПП
        private void button6_Click(object sender, EventArgs e)
        {
            float fX = float.Parse(textBox10.Text);
            float fY = float.Parse(textBox11.Text);
            float fZ = float.Parse(textBox12.Text);

            if (fX != 0 &
                fY != 0 &
                fZ != 0)
            {
                float[,] matrix = new float[,]
                {
                    {1, 0, 0, 1 / fX},
                    {0, 1, 0, 1 / fY},
                    {0, 0, 1, 1 / fZ},
                    {0, 0, 0, 1}
                };

                pointsList = PointsTransform(pointsList, matrix);
                Build();
            }
            else
            {
                MessageBox.Show("Коэффициенты не должны равняться 0");
            }
        }

        // Косой сдвиг
        private void button7_Click(object sender, EventArgs e)
        {
            float K = float.Parse(textBox13.Text);

            float[,] matrix = new float[,]
            {
                {1, K, 0, 0},
                {0, 1, 0, 0},
                {0, 0, 1, 0},
                {0, 0, 0, 1}
            };

            pointsList = PointsTransform(pointsList, matrix);
            Build();
        }

        // Вписать фигуру в экран
        private void button8_Click(object sender, EventArgs e)
        {
            float maxX = pointsList.Max(P => P.x);
            float maxY = pointsList.Max(P => P.y);

            float kX = (pictureBox1.Width / 2) / maxX;
            float kY = (pictureBox1.Height / 2) / maxY;
            float k = Math.Min(kX, kY);

            float[,] matrix = new float[,]
            {
                {k, 0, 0, 0},
                {0, k, 0, 0},
                {0, 0, k, 0},
                {0, 0, 0, 1}
            };

            pointsList = PointsTransform(pointsList, matrix);
            Build();
        }
    }
}