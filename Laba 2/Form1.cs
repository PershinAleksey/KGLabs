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

namespace Laba_2
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

        private class Vector
        {
            public float x, y, z;
        }

        private class Plane
        {
            public List<int> vertices = new List<int>();
            public bool visibility = false;

            public void SetVisibility(bool value)
            {
                visibility = value;
            }
        }


        List<Point> pointsList = new List<Point>();
        List<Plane> planesList = new List<Plane>();


        private void Build()
        {
            Graphics canvas = pictureBox1.CreateGraphics();
            canvas.Clear(Color.White);
            canvas.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            canvas.ScaleTransform(1, -1);
            Pen thePen = new Pen(Color.Black);

            ResetVisibility();
            CheckVisibility();

            foreach (Plane thePlane in planesList)
            {
                if (!thePlane.visibility)
                {
                    continue;
                }

                for (int i = 0; i < thePlane.vertices.Count; i++)
                {
                    Point current = pointsList[thePlane.vertices[i] - 1];
                    Point previous = new Point();
                    if (i == 0)
                    {
                        previous = pointsList[thePlane.vertices.Last() - 1];
                    }
                    else
                    {
                        previous = pointsList[thePlane.vertices[i - 1] - 1];
                    }

                    canvas.DrawLine(thePen, (current.x / current.h), (current.y / current.h),
                                            (previous.x / previous.h), (previous.y / previous.h));
                }
            }
        }

        private void ResetVisibility()
        {
            foreach (Plane thePlane in planesList)
            {
                thePlane.SetVisibility(false);
            }
        }

        private void CheckVisibility()
        {
            Vector gazeDirection = new Vector
            {
                x = 0,
                y = 0,
                z = -1
            };

            foreach (Plane thePlane in planesList)
            {
                Point firstPoint = pointsList[thePlane.vertices[0] - 1];
                Point secondPoint = pointsList[thePlane.vertices[1] - 1];
                Point thirdPoint = pointsList[thePlane.vertices[2] - 1];

                Vector firstVector = new Vector
                {
                    x = secondPoint.x / secondPoint.h - firstPoint.x / firstPoint.h,
                    y = secondPoint.y / secondPoint.h - firstPoint.y / firstPoint.h,
                    z = secondPoint.z / secondPoint.h - firstPoint.z / firstPoint.h
                };
                Vector secondVector = new Vector
                {
                    x = thirdPoint.x / thirdPoint.h - secondPoint.x / secondPoint.h,
                    y = thirdPoint.y / thirdPoint.h - secondPoint.y / secondPoint.h,
                    z = thirdPoint.z / thirdPoint.h - secondPoint.z / secondPoint.h
                };
                Vector normal = VectorProduct(firstVector, secondVector);

                Point externalPoint = new Point();
                for (int j = 1; j <= pointsList.Count; j++)
                {
                    if (!thePlane.vertices.Contains(j))
                    {
                        externalPoint = pointsList[j - 1];
                        break;
                    }
                }
                Vector externalVector = new Vector
                {
                    x = externalPoint.x / externalPoint.h - firstPoint.x / firstPoint.h,
                    y = externalPoint.y / externalPoint.h - firstPoint.y / firstPoint.h,
                    z = externalPoint.z / externalPoint.h - firstPoint.z / firstPoint.h
                };

                if (ScalarProduct(normal, externalVector) > 0)
                {
                    normal.x = -normal.x;
                    normal.y = -normal.y;
                    normal.z = -normal.z;
                }

                if (ScalarProduct(normal, gazeDirection) < 0)
                {
                    thePlane.SetVisibility(true);
                }
            }
        }

        private Vector VectorProduct(Vector first, Vector second)
        {
            Vector product = new Vector
            {
                x = first.y * second.z - first.z * second.y,
                y = first.z * second.x - first.x * second.z,
                z = first.x * second.y - first.y * second.x
            };
            return product;
        }

        private float ScalarProduct(Vector first, Vector second)
        {
            float product = first.x * second.x + first.y * second.y + first.z * second.z;
            return product;
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
            planesList.Clear();

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

            int planesNum = Convert.ToInt32(reader.ReadLine());
            for (int i = 0; i < planesNum; i++)
            {
                string line = reader.ReadLine();
                string[] verticesArray = line.Split(' ');

                List<int> verticesList = new List<int>();
                foreach (string vertex in verticesArray)
                {
                    verticesList.Add(Convert.ToInt32(vertex));
                }

                Plane pln = new Plane
                {
                    vertices = verticesList
                };
                planesList.Add(pln);
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