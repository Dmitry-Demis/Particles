using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Particles.Model;
using Point = System.Windows.Point;

namespace Particles
{    
    public partial class LW3_MolecularDynamicsMethod : Window
    {
        public LW3_MolecularDynamicsMethod() => InitializeComponent();
        private static int N;
        private float radius;
        private float startSpeed = 1;
        List<Atom> atoms = new List<Atom>();
        private Random rnd = new Random();
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
                gfx.Clear(Color.FromArgb(30,30,30));
                foreach (var atom in atoms)
                {
                    var pt = new Point(atom.X, atom.Y);
                   
                    this.Dispatcher.Invoke(() =>
                    {
                        
                        gfx?.FillEllipse(new SolidBrush(atom.Color), (float)pt.X, (float)pt.Y, (int)radius, (int)radius);
                        gfx?.DrawEllipse(new Pen(atom.CirlceColor), (float)pt.X - radius / 2, (float)pt.Y - 1.5f * radius,
                            (int)radius * 2, (int)radius * 4);
                        gfx?.DrawEllipse(new Pen(atom.CirlceColor), (float)pt.X - 1.5f * radius, (float)pt.Y - radius / 2,
                            (int)radius * 4, (int)radius * 2);

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
            Field.Width = (int)myCanvas.ActualWidth;
            Field.Height = (int)myCanvas.ActualHeight;
        }
        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            N = Convert.ToInt32(atomCount.Text);
            radius = Convert.ToSingle(atomRadius.Text, CultureInfo.InvariantCulture);
            startSpeed = Convert.ToSingle(startVelocity.Text, CultureInfo.InvariantCulture);
            float r = (float)(radius / 2f);
            float epsilon = 3.0f;
            atoms.Clear();
            for (int i = 0; i < N; i++) //цикл, пока не пройдет все атомы (count)  
            {
                Atom a = new LennardJonesAtom(radius, epsilon); //создание экземпляра конкретного атома класа Atom
                a.X = rnd.Next((int)r, (int)(Field.Width - r)); //Присваивание радномных координат созданным атомам
                a.Y = rnd.Next((int)r, (int)(Field.Height - r)); //Чтобы атом не создавался за пределами PictureBox, отнять r
                a.Velocity = new PointF((float)(rnd.NextDouble() - 0.5) * startSpeed, (float)(rnd.NextDouble() - 0.5) * startSpeed);
                int red = rnd.Next(0, 256);
                int green = rnd.Next(0, 256);
                int blue = rnd.Next(0, 256);
                a.Color = Color.FromArgb(red, green, blue);
                red = rnd.Next(0, 256);
                green = rnd.Next(0, 256);
                blue = rnd.Next(0, 256);
                a.CirlceColor = Color.FromArgb(red, green, blue);
                atoms.Add(a); //добавление созданных атомов в список
            }
            Task.Run(Go);
        }
        private void Go()
        {
            while (true)
            {
                foreach (var a in atoms)
                    a.ApplyForce(new PointF(0, 1f));

                foreach (var a in atoms)
                    a.Move();//для каждого атома из спиcка применяется функция Move
                for (var i = 0; i < atoms.Count; i++)
                for (var j = i + 1; j < atoms.Count; j++)
                {
                    atoms[i].CalcCollision(atoms[j]);
                }
                Render();
            }
        }
    }
}
