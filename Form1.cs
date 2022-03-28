using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace Ex1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }
        
        
        List<double> u = new List<double>();
        List<double> v = new List<double>();
        //double[] u = new double[2];
        //double[] v = new double[2];
        double[] P = { 0.0, 0.0 };
        double[] E = { 0.0, 6.0 };
        double k, x, b, y;
        double T;
        double delta;
        double xmin, xmax, ymin, ymax ;
        double phi;

        private void Form1_Load(object sender, EventArgs e)
        {
            delta = 0.00001;
            k = -1.5;
            b = 2.0;
            x = 0.0;
            //y = k * x + b;
            phi = 0.0;
            //v[0] = cos(phi);
            //v[1] = sin(phi);
            //u[0] = v[0];
            //u[1] = v[0] * k + b;
            v.Add(cos(phi));
            v.Add(sin(phi));
            u.Add(v[0]);
            u.Add(v[0] * k + b);

            xmin = -6.0;
            xmax = 6.0 ;
            ymin = -2.0;
            ymax = 13.0;

            T = Norm(P[0], P[1], E[0], E[1]) / (u[1] - v[1]);


            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGBA | Glut.GLUT_DOUBLE);

            Gl.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(xmin, xmax, ymin, ymax);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }

        void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            

            DrawCoords();
            DrawPursuerSet();
            DrawEscaperSet();
            DrawCaptureSet();
            DrawCapture(Math.PI / 3.0);
            
            Gl.glEnd();
           
            AnT.Invalidate();
        }

        void PrintText2D(double x, double y, string text)
        {
            Gl.glRasterPos2d(x, y);
            foreach (char a in text)
            {
                //Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_8_BY_13, a);
                Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_TIMES_ROMAN_24, a);
            }
        }

        void DrawCoords()
        {
            Gl.glColor3f(0.0f, 0.0f, 0.0f);
            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex2d(xmin, 0.0);
            Gl.glVertex2d(xmax, 0.0);

            Gl.glVertex2d(0.0, ymin);
            Gl.glVertex2d(0.0, ymax);

            Gl.glVertex2d(xmax, 0.0);
            Gl.glVertex2d(xmax-0.2, 0.15);

            Gl.glVertex2d(xmax, 0.0);
            Gl.glVertex2d(xmax - 0.2, -0.15);

            Gl.glVertex2d(0.0, ymax);
            Gl.glVertex2d(0.15, ymax - 0.2);

            Gl.glVertex2d(0.0, ymax);
            Gl.glVertex2d(-0.15, ymax - 0.2);

            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINES);
            for (double i = xmin; i < xmax; i++)
            {
                Gl.glVertex2d(i, -0.2);
                Gl.glVertex2d(i, 0.2); 
            }

            for (double j = ymin; j < ymax; j++)
            {
                    Gl.glVertex2d(-0.2, j);
                    Gl.glVertex2d(0.2, j);
            }
            Gl.glEnd();

            PrintText2D(1.1, 0.1, "1");
            PrintText2D(0.1, 1.1, "1");

            Gl.glPointSize(6.0f);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glColor3f(1.0f, 0.0f, 0.0f);
            Gl.glVertex2d(P[0], P[1]);

            
            Gl.glColor3f(0.0f, 0.0f, 1.0f);
            Gl.glVertex2d(E[0], E[1]);
            
            Gl.glEnd();

            

            Gl.glColor3f(1.0f, 0.0f, 0.0f);
            PrintText2D(P[0] + 0.1, P[1] + 0.1, "P(0)");
            Gl.glColor3f(0.0f, 0.0f, 1.0f);
            PrintText2D(E[0] + 0.1, E[1] + 0.1, "E(0)");
        }

        void DrawEscaperSet()
        {
            Gl.glPointSize(1.0f);
            Gl.glColor3f(0.0f, 0.0f, 1.0f);
            Gl.glBegin(Gl.GL_POINTS);
            for (double i = 0.0; i <= 2.0 * Math.PI; i += delta)
            {
                v[0] = cos(phi);
                v[1] = sin(phi);
               
                Gl.glVertex2d(E[0] + v[0], E[1] + v[1]);
                Gl.glVertex2d(P[0] + v[0], P[1] + v[1]);


                phi += i;
            }
            Gl.glEnd();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Draw();
        }

        double Func(double x, double k, double b)
        {
            return k * x + b;
        }

        //SAVE
        private void button2_Click(object sender, EventArgs e)
        {
            Draw();
            
            Bitmap image = new Bitmap(AnT.Width, AnT.Height);
            BitmapData imgData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);

            Gl.glPushClientAttrib(Gl.GL_CLIENT_PIXEL_STORE_BIT);
            Gl.glPixelStoref(Gl.GL_PACK_ALIGNMENT, 4);

            Gl.glReadPixels(0, 0, image.Width, image.Height, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, imgData.Scan0);
            Gl.glPopClientAttrib();
            
            image.UnlockBits(imgData);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            
            // формат можно поменять (меняется и в расширении названия и второй параметр метода Save)
            
            image.Save("C:\\Users\\User\\Desktop\\screenshot\\screen.jpg", ImageFormat.Jpeg);
        }

        double Norm(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        double cos(double phi)
        {
            return Math.Cos(phi);
        }

        double sin(double phi)
        {
            return Math.Sin(phi);
        }

        void DrawPursuerSet()
        {
            Gl.glColor3f(1.0f, 0.0f, 0.0f);

            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex2d(xmin, Func(xmin, k, b));
            Gl.glVertex2d(xmax, Func(xmax, k, b));

            Gl.glEnd();
        }

        void DrawCaptureSet()
        {
            Gl.glPointSize(3.0f);
            Gl.glColor3f(0.0f, 1.0f, 0.0f);
            Gl.glBegin(Gl.GL_POINTS);
            for (double i = 0.0; i <= 2.0 * Math.PI; i += delta)
            {
               

                v[0] = cos(phi);
                v[1] = sin(phi);
                u[0] = v[0];
                u[1] = v[0] * k + b;
                T = Norm(P[0], P[1], E[0], E[1]) / (u[1] - v[1]);

                Gl.glVertex2d(E[0] + v[0] * T, E[1] + v[1] * T);
                
                phi += i;
            }
            Gl.glEnd();
        }

        void DrawCapture(double phi)
        {
            v[0] = cos(phi);
            v[1] = sin(phi);
            u[0] = v[0];
            u[1] = v[0] * k + b;
            T = Norm(P[0], P[1], E[0], E[1]) / (u[1] - v[1]);

            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3f(0.0f, 0.0f, 1.0f);//движение убегающего
            Gl.glVertex2d(E[0], E[1]);
            Gl.glVertex2d(E[0] + v[0] * T, E[1] + v[1] * T);

            Gl.glColor3f(1.0f, 0.0f, 0.0f);//движение преследователя
            Gl.glVertex2d(P[0], P[1]);
            Gl.glVertex2d(P[0] + u[0] * T, P[1] + u[1] * T);
            Gl.glEnd();

            Gl.glEnable(Gl.GL_LINE_STIPPLE);//управление преследователя

            Gl.glLineStipple(1, 0x00FF);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(P[0], P[1]);
            Gl.glVertex2d(P[0] + v[0], P[1] + v[1]);
            
            Gl.glVertex2d(P[0] + v[0], P[1] + v[1]);
            Gl.glVertex2d(P[0] + v[0], Func(P[0] + v[0],k , b));
            
            Gl.glEnd();
            
            Gl.glDisable(Gl.GL_LINE_STIPPLE);

            Gl.glPointSize(6.0f);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glColor3f(1.0f, 0.0f, 1.0f);

            Gl.glVertex2d(E[0] + v[0] * T, E[1] + v[1] * T);
           
            Gl.glEnd();

            PrintText2D(E[0] + v[0] * T + 0.1, E[1] + v[1] * T + 0.1, "E(T)");
        }
    }
}
