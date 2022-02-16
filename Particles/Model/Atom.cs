using System;
using System.Drawing;

namespace Particles.Model
{
    public class Atom
    {
        public Color CirlceColor { get; set; }
        public Color Color { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public PointF Velocity;
        public float Speed => Velocity.Length();
        private float _radius;
        private float _mass;
        public static float CoeffCollision = 1; //Коэффициент коллизии (столкновения), 1 или -1, направление вперёд или в противоположную сторону
        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                _mass = _radius * _radius;
            }
        }
        public float Mass => _mass;
        public Atom(float radius) //метод, описывающий атом
        {
            Radius = radius;
        }
        //расчёт траектории (отскакивание от стенок)
        public void Move()
        {
            
            if (X <= 0 && Velocity.X < 0) Velocity.X *= -1; // Если координата Х <= 10, то меняем направление движения по оси Х на противоположное
            else if (X >= Field.Width - Radius && Velocity.X > 0) Velocity.X *= -1f;
            if (Y <= 0 && Velocity.Y < 0) Velocity.Y *= -1; // Если координата У <= 10, то меняем направление движения по оси У на противоположное
            else if (Y >= Field.Height - Radius && Velocity.Y > 0) Velocity.Y *= -1;

            X += Velocity.X * Field.DeltaTime;
            Y += Velocity.Y * Field.DeltaTime;
        }
        protected double DistanceSqr(Atom ball)
        {
            return (X - ball.X) * (X - ball.X) + (Y - ball.Y) * (Y - ball.Y);
        }

        protected double Distance(Atom ball)
        {
            return (float)Math.Sqrt(DistanceSqr(ball));
        }

        public bool IsCollision(Atom ball, double distance)
        {
            return DistanceSqr(ball) <= distance * distance; // условие столкновения
        }

        public virtual void CalcCollision(Atom ball)
        {
            if (IsCollision(ball, Radius + ball._radius) && (Velocity.X - ball.Velocity.X) * (X - ball.X) + (Velocity.Y - ball.Velocity.Y) * (Y - ball.Y) < 0)
            {
                //условие столкновения, учитывая dX
                double dx = ball.X - X, dy = ball.Y - Y;
                double alfa = Math.Atan2(dy, dx);
                double angle = Math.Atan2(Velocity.Y, Velocity.X) - alfa, angleBall = Math.Atan2(ball.Velocity.Y, ball.Velocity.X) - alfa,
                    mod = Speed, modBall = ball.Speed;
                double DX = mod * Math.Cos(angle),
                    newDY = mod * Math.Sin(angle),
                    DX1 = modBall * Math.Cos(angleBall),
                    newDY1 = modBall * Math.Sin(angleBall);
                double newDX = (DX * (Mass - ball.Mass) + 2 * ball.Mass * DX1) / (Mass + ball.Mass),
                    newDX1 = (DX1 * (ball.Mass - Mass) + 2 * ball.Mass * DX) / (Mass + ball.Mass);
                angle = Math.Atan2(newDY, newDX) + alfa;
                angleBall = Math.Atan2(newDY1, newDX1) + alfa;
                mod = CoeffCollision * Math.Sqrt(newDX * newDX + newDY * newDY);
                modBall = CoeffCollision * Math.Sqrt(newDX1 * newDX1 + newDY1 * newDY1);
                Velocity.X = (float)(mod * Math.Cos(angle));
                Velocity.Y = (float)(mod * Math.Sin(angle));
                ball.Velocity.X = (float)(modBall * Math.Cos(angleBall));
                ball.Velocity.Y = (float)(modBall * Math.Sin(angleBall));
            }
        }
        public void ApplyForce(PointF force)
        {
            //calc acceleration
            var acc = force.Mul(1 / Mass);
            //clac moving offset
            Velocity.X += acc.X * Field.DeltaTime;
            Velocity.Y += acc.Y * Field.DeltaTime;
        }

    }
}
