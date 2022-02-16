using System;
using System.ComponentModel;
using System.Windows;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Linq;
using Point = System.Windows.Point;

namespace Particles
{
    /// <summary>
    /// Interaction logic for LW1.xaml
    /// </summary>
    public partial class LW1 : Window
    {
        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }
        public LW1() => InitializeComponent();
        private readonly Random rnd = new Random();
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
        private void MyCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            CanvasWidth = myCanvas.ActualWidth;
            CanvasHeight = myCanvas.ActualHeight;
        }
        public double X { get; set; }
        public double XStart { get; set; }
        public double U { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double M { get; set; }
        public double N{ get; set; }
        public float Radius { get; set; } = 30;
        public double[] XResult { get; private set; }
        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            X = Convert.ToDouble(xField.Text);
            XStart = X;
            U = Convert.ToDouble(uField.Text);
            A = Convert.ToDouble(aField.Text);
            B = Convert.ToDouble(bField.Text);
            M = Convert.ToDouble(mField.Text);
            N = Convert.ToDouble(nField.Text);
            Task.Run(Go);
        }
        private void Go()
        {
           
            double                
                unew,
                xnew;
            double tau = 1e-5;
            double tb = 0.0, te = 10.0;
            int count = (int)((te - tb) / tau);
           
            XResult = new double[count];
            int i = 0;
            while (i < count)
            {
                unew = U + tau * 1.0 / M * (-Math.Pow(B, 2) * X + Math.Pow(A, 2) / Math.Pow(X, N));
                xnew = X + tau * U;
                U = unew;
                X = xnew;
                XResult[i] = X;
                i++;              
            }
            var maxV = XResult.Max();
            var minV = XResult.Min();
            //Вписать в другой диапазон
            for (int j = 0; j < count; j++)
            {
                X = (XResult[j] - minV) / (maxV - minV) * (CanvasWidth - XStart)+XStart;
                Render();
            }
           

        }
        
        private void Render()
        {
            using (var bmp = new Bitmap((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight))
            using (var gfx = Graphics.FromImage(bmp))
            using (var pen = new Pen(Color.Aqua))
           // for (int i = 0; i < XResult.Length; i++)
            {
                // draw one thousand random white lines on a dark blue background
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(Color.FromArgb(30, 30, 30));               
                    this.Dispatcher.Invoke(() =>
                    {
                        gfx?.DrawLine(new Pen(Color.Yellow), 0, (float)(CanvasHeight / 2), (float)CanvasWidth, (float)(CanvasHeight / 2));
                        gfx?.FillEllipse(new SolidBrush(Color.Red), (float)(X), (float)((CanvasHeight-Radius) / 2), Radius, Radius);
                        Result = X;

                    });
                    this.Dispatcher.Invoke(() =>
                {
                    myImage.Source = BmpImageFromBmp(bmp);
                });
            }
        }

        #region readonly Result : double - Результат работы алгоритма

        /// <summary>Результат работы алгоритма</summary>
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register(
                nameof(Result),
                typeof(double),
                typeof(LW1),
                new PropertyMetadata(default(double)));

        /// <summary>Результат работы алгоритма</summary>
        [Description("Результат работы алгоритма")]
        public double Result
        {
            get => (double)GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }
        #endregion
        
    }
}
