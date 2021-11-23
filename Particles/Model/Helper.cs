using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particles.Model
{
    public static class Helper
    {
        public static float Length(this PointF p) => (float)Math.Sqrt(p.X * p.X + p.Y * p.Y);

        public static PointF Mul(this PointF p, float k) => new PointF(p.X * k, p.Y * k);

        public static PointF Add(this PointF p, PointF other) => new PointF(p.X + other.X, p.Y + other.Y);

        public static PointF Normalized(this PointF p)
        {
            var k = p.Length();
            if (Math.Abs(k) <= 0.0001f)
                k = 0.0001f;
            return new PointF(p.X / k, p.Y / k);
        }
    }
}
