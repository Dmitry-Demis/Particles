using System;
using System.Windows;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace Particles
{    
    public partial class LW3_MolecularDynamicsMethod : Window
    {
        public LW3_MolecularDynamicsMethod() => InitializeComponent();
        private const double N1 = 11.0;
        private const double N2 = 5.0;
        private const int A = 3;
        private const int B = 4;
        private static int N=25;

        private double[]
            x,
            y,
            xOld,
            yOld ,
            m ;

        private double[] ux;
        private double[] uy;
        private double[] uOldX;
        private double[] uOldY;
        private double t = 0.0;
        private double tmax = 100.0;
        private double tau = 0.01;
        private double xLen = 100;
        private double yLen = 100;
        private double maxR = 100;
        private double radius;
        private double startSpeed = 1;

        private Random rand = new Random();
        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        private void Render()
        {
            using (var bmp = new Bitmap((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight))
            using (var gfx = Graphics.FromImage(bmp))
            using (var pen = new Pen(Color.Aqua))
            {
                // draw one thousand random white lines on a dark blue background
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(System.Drawing.Color.Navy);
                for (int i = 0; i < N; i++)
                {
                    var pt = new Point(x[i], y[i]);
                    this.Dispatcher.Invoke(() =>
                    {
                       gfx?.DrawEllipse(pen, (float)(pt.X), (float)(pt.Y), (int)radius, (int)radius);
                        //gfx?.FillEllipse(new SolidBrush(Color.Aquamarine), (float)pt.X, (float)pt.Y, sizeOfEllipse, sizeOfEllipse);
                       
                    });
                }
                this.Dispatcher.Invoke(() =>
                {
                    myImage.Source = BmpImageFromBmp(bmp);
                });
            }
        }
        private void MyCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            N = Convert.ToInt32(atomCount.Text);
            radius = Convert.ToDouble(atomRadius.Text, CultureInfo.InvariantCulture);
            startSpeed = Convert.ToDouble(startVelocity.Text, CultureInfo.InvariantCulture);
            x = new double[N];
            y = new double[N];
            xOld = new double[N];
            yOld = new double[N];
            ux = new double[N];
            uy = new double[N];
            uOldX = new double[N];
            uOldY = new double[N];
            m = new double[N];
            int minMass = 1;
            int maxMass = 8;
            for (int i = 0; i < N; i++)
            {
                x[i] = rand.Next((int)radius, (int)(myCanvas.ActualWidth - radius));
                y[i] = rand.Next((int)radius, (int)(myCanvas.ActualHeight - radius));
                xOld[i] = x[i];
                yOld[i] = y[i];
                m[i] = rand.Next(minMass, maxMass);
                ux[i] = (float)(rand.NextDouble() - 0.5) * startSpeed;
                uy[i] = (float)(rand.NextDouble() - 0.5) * startSpeed;
                uOldX[i] = ux[i];
                uOldY[i] = uy[i];
                //xc += 25;
                //yc += 10;
            }
            Render();
            Task.Run(Solution);
        }
        private void Solution()
        {
            while (true)
            {
                for (int i = 0; i < N; i++)
                {
                    double xSum = 0.0;
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j)
                        {
                            var r = Math.Sqrt(Math.Pow(xOld[i] - xOld[j], 2) + Math.Pow(yOld[i] - yOld[j], 2));
                            if (Math.Abs(r - 2*radius) < 0.001)
                            {
                                continue;
                                
                            }
                            xSum += (A / Math.Pow(r - 2 * radius, N1 + 1) - B / Math.Pow(r - 2 * radius, N2 + 1)) * (x[j] - x[i]);

                        }
                    }
                    ux[i] = 1 / m[i] * (uOldX[i] + tau * xSum);
                    double ySum = 0.0;
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j)
                        {
                            var r = Math.Sqrt(Math.Pow(xOld[i] - xOld[j], 2) + Math.Pow(yOld[i] - yOld[j], 2));
                            if (Math.Abs(r - 2 * radius) < 0.001)
                            {
                                continue;

                            }
                            ySum += (A / Math.Pow(r - 2 * radius, N1 + 1) - B / Math.Pow(r - 2 * radius, N2 + 1)) * (y[j] - y[i]);

                        }
                    }
                    uy[i] = 1 / m[i] * (uOldY[i] + tau * ySum);
                    x[i] = xOld[i] + tau * uOldX[i];
                    y[i] = yOld[i] + tau * uOldY[i];
                    //double t = x[i];
                    //if (t > 0.9 * myCanvas.ActualWidth || t < 0.1 * myCanvas.ActualWidth)
                    //{
                    //    x[i] = xOld[i] + tau * (-ux[i]);
                    //}
                    //t = y[i];
                    //if (t > 0.9 * myCanvas.ActualHeight || t < 0.1 * myCanvas.ActualHeight)
                    //{
                    //    y[i] = yOld[i] + tau * (-uy[i]);
                    //}
                    double t = x[i] + tau * ux[i];
                    int v = 1;
                    int l = 1;
                    if (t >  myCanvas.ActualWidth - radius || t < 0)
                    {
                        x[i] = xOld[i] + tau * (-ux[i]);
                        xOld[i] = x[i];
                        v *= -1;
                    }
                    else
                    {
                        xOld[i] = t;
                    }
                    t = y[i] + tau * uy[i];
                    if (t > myCanvas.ActualHeight - radius || t < 0)
                    {
                        y[i] = yOld[i] + tau * (-uy[i]);
                        yOld[i] = y[i];
                        l *= -1;
                    }
                    else
                    {
                        yOld[i] = t;
                    }

                    uOldX[i] = v*ux[i];
                    uOldY[i] = l*uy[i];
                    
                    
                   
                }
                // t += tau;
                Render();
            }
        }

        private double F(int i, string coord)
        {
            if (string.IsNullOrWhiteSpace(coord))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(coord));
            double sum = 0.0;
            for (int j = 0; j < N; j++)
            {
                if (i != j)
                {
                    double r = Math.Sqrt(Math.Pow(xOld[i] - xOld[j], 2) + Math.Pow(yOld[i] - yOld[j], 2));

                    sum += A / Math.Pow(r - 2 * radius, N1 + 1) + B / Math.Pow(r - 2 * radius, N2 + 1);
                    sum *= (string.Equals(coord, "x", StringComparison.OrdinalIgnoreCase))
                        ? (x[j] - x[i])
                        : (y[j] - y[i]);
                }
            }
            return sum;
        }
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            
        }
        double F1(int i)
        {
            double res = 0.0;

            for (int j = 0; j < N; j++)
            {
                double sum = 0.0;
                if (i != j)
                {
                    sum += A / Math.Pow(Math.Pow((xOld[i] - xOld[j]) * (xOld[i] - xOld[j]) + (yOld[i] - yOld[j]) * (yOld[i] - yOld[j]), 0.5), N1 + 1) - B / Math.Pow(Math.Pow((xOld[i] - xOld[j]) * (xOld[i] - xOld[j]) + (yOld[i] - yOld[j]) * (yOld[i] - yOld[j]), 0.5), N2 + 1);
                    res = sum * (x[j] - x[i]);
                }

            }
            return res;
        }
        double F2(int i)
        {
            double res = 0.0;
            for (int j = 0; j < N; j++)
            {
                if (i != j)
                    res += A / Math.Pow(Math.Pow((xOld[i] - xOld[j]) * (xOld[i] - xOld[j]) + (yOld[i] - yOld[j]) * (yOld[i] - yOld[j]), 0.5), N1 + 1) - B / Math.Pow(Math.Pow((xOld[i] - xOld[j]) * (xOld[i] - xOld[j]) + (yOld[i] - yOld[j]) * (yOld[i] - yOld[j]), 0.5), N2 + 1) * (y[j] - y[i]);
            }
            return res;
        }
    }
}
