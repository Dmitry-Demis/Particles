using System;
using System.Drawing;
namespace Particles.Model
{
    public class LennardJonesAtom : Atom
    {
        private float epsilon; 
        public LennardJonesAtom(float radius, float epsilon) : base(radius)
        {
            this.epsilon = epsilon;
        }
        public override void CalcCollision(Atom other)
        {
            //calc distance between atoms
            var dist = Distance(other);
            if (dist > Radius * 5) //no interaction
                return;
            //calc force
            var k = Radius / dist;
            var f = 4 * epsilon * (float)(Math.Pow(k, 12) - Math.Pow(k, 6));//module of force (LennardJones)
            //to avoid infinite forces -> imitate elastic impact
            if (f > 30 || float.IsNaN(f))
            {
                base.CalcCollision(other);
                return;
            }
            //force vector
            var force = new PointF(X - other.X, Y - other.Y).Normalized().Mul(f);
            //apply force
            ApplyForce(force);
            other.ApplyForce(force.Mul(-1));
        }
    }
}
