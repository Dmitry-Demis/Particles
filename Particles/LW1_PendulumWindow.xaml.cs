using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Particles
{
    /// <summary>
    /// Interaction logic for LW1_PendulumWindow.xaml
    /// </summary>
    public partial class LW1_PendulumWindow : Window, INotifyPropertyChanged
    {

        public LW1_PendulumWindow()
        {
            InitializeComponent();
        }
        private Ellipse ellipse { get; set; }

        #region M : double - Масса тела

        /// <summary>Масса тела</summary>

        private double _M = 1;

        /// <summary>Масса тела</summary>

        public double M
        {
            get => _M;
            set => SetProperty(ref _M, value);
        }

        #endregion

        #region N : double - Степень

        /// <summary>Степень</summary>

        private double _N = 3;

        /// <summary>Степень</summary>

        public double N
        {
            get => _N;
            set => SetProperty(ref _N, value);
        }

        #endregion

        #region A : double - Коэффициент a

        /// <summary>Коэффициент a^</summary>

        private double _A = 1;

        /// <summary>Коэффициент a^</summary>

        public double A
        {
            get => _A;
            set => SetProperty(ref _A, value);
        }

        #endregion

        #region B : double - Коэффициент b

        /// <summary>Коэффициент b^2</summary>

        private double _B = 1;

        /// <summary>Коэффициент b^2</summary>

        public double B
        {
            get => _B;
            set => SetProperty(ref _B, value);
        }

        #endregion

        #region X : double - Начальное значение X

        /// <summary>Начальное значение X</summary>

        private double _X = 10;

        /// <summary>Начальное значение X</summary>

        public double X
        {
            get => _X;
            set => SetProperty(ref _X, value);
        }

        #endregion

        #region U : double - Начальное значение скорости U

        /// <summary>Начальное значение скорости U</summary>

        private double _U;

        /// <summary>Начальное значение скорости U</summary>

        public double U
        {
            get => _U;
            set => SetProperty(ref _U, value);
        }

        #endregion

        #region XCurrent : double - Текущее значение X

        /// <summary>Текущее значение X</summary>

        private double _XCurrent;

        /// <summary>Текущее значение X</summary>

        public double XCurrent
        {
            get => _XCurrent;
            set => SetProperty(ref _XCurrent, value);
        }

        #endregion

        #region UCurrent : double - Текущее значение скорости U

        /// <summary>Текущее значение скорости U</summary>

        private double _UCurrent;

        /// <summary>Текущее значение скорости U</summary>

        public double UCurrent
        {
            get => _UCurrent;
            set => SetProperty(ref _UCurrent, value);
        }

        #endregion

        #region Result : double - Результирующий ответ

        /// <summary>Результирующий ответ</summary>

        private double _Result;

        /// <summary>Результирующий ответ</summary>

        public double Result
        {
            get => _Result;
            set => SetProperty(ref _Result, value);
        }

        #endregion

        #region LeftDisplacement : double - Смещение слева от Canvas

        /// <summary>Смещение слева от Canvas</summary>

        private double _LeftDisplacement;

        /// <summary>Смещение слева от Canvas</summary>

        public double LeftDisplacement
        {
            get => _LeftDisplacement;
            set => SetProperty(ref _LeftDisplacement, value);
        }

        #endregion

        private List<double> Results = new List<double>();

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            Solution();
            Task.Run(() => Draw());
        }
        
        private void Solution()
        {
            double x = X, u = U, unew, xnew;
            double tau = 1e-5;
            double tb = 0.0, te = 10.0;
            while (te > tb)
            {
                unew = u + tau * (-Math.Pow(B, 2) * x + Math.Pow(A, 2) / Math.Pow(x, 3));
                xnew  = x + tau * u;
                Results.Add(xnew);
                tb += tau;
                u = unew;
                x = xnew;
            }
        }

        void Draw()
        {
            double max = Results.Max();
            double segment = 0.9*myCanvas.ActualWidth / max;
            double start = X;
            Task.Run(() =>
            {
                foreach (var result in Results)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Canvas.SetLeft(ellipse, start + segment * result);
                        ResultBox.Text = result.ToString();
                    });
                    
                }
            });
        }


        private void MyCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            var height = myCanvas.ActualHeight;
            var width = myCanvas.ActualWidth;
            Line horizontalLine = new Line
            {
                X1 = 0,
                X2 = width,
                Stroke = Brushes.Yellow,
                Y1 = height/2,
                Y2 = height/2,
                StrokeThickness = 1
            };
            ellipse = new Ellipse();
            ellipse.Width = 40;
            ellipse.Height = ellipse.Width;
            ellipse.Fill = Brushes.Red;
            myCanvas.Children.Add(horizontalLine);
            myCanvas.Children.Add(ellipse);
            LeftDisplacement = width / 2;
            Canvas.SetLeft(ellipse, LeftDisplacement);
            Canvas.SetTop(ellipse, (height-ellipse.Height)/2);
        }

       


        /// <summary>
        /// sets a property. If values aren't equal, a new value is replaced
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">a reference of a private field of a property</param>
        /// <param name="value">the current value </param>
        /// <param name="propertyName">a name of a selected property [optional]</param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// an event that says us about changes 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
