using System;
using System.Numerics;

namespace SharpTracer
{
  public class Camera
    {
        private readonly Random _random;
        public Camera(Vector3 lookFrom, Vector3 lookAt, Vector3 vUp, float verticalFieldOfView, float aspect, float aperture, float focusDistance)
        {
            _random = new Random();
            LensRadius = aperture / 2.0f;
            var theta = verticalFieldOfView * Math.PI / 180;
            var halfHeight = (float) Math.Tan(theta / 2);
            var halfWidth = aspect * halfHeight;
            Origin = lookFrom;
            W = Vector3.Normalize(lookFrom - lookAt);
            U = Vector3.Normalize(Vector3.Cross(vUp, W));
            V = Vector3.Cross(W, U);

            LowerLeft = Origin - halfWidth * focusDistance * U - halfHeight * focusDistance * V - focusDistance * W;
            Vertical = 2 * halfHeight * V * focusDistance;
            Horizontal = 2 * halfWidth * U * focusDistance;
        }
        public Vector3 Origin { get; set; }
        public Vector3 LowerLeft { get; set; }
        public Vector3 Horizontal { get; set; }
        public Vector3 Vertical { get; set; }
        public float LensRadius { get; set; }

        public Vector3 U { get; set; }
        public Vector3 V { get; set; }
        public Vector3 W { get; set; }
        public Ray GetRay(float s, float t)
        {
            var rd = LensRadius * RandomInUnitDisk();
            var offset = U * rd.X + V * rd.Y;
            return new Ray(Origin + offset, LowerLeft + s * Horizontal + t * Vertical - Origin - offset);
        }

        private Vector3 RandomInUnitDisk()
        {
            Vector3 p;
            do
            {
                var x = (float)_random.NextDouble();
                var y = (float)_random.NextDouble();

                p = (2.0f * new Vector3(x, y, 0)) - new Vector3(1, 1, 0);
            } while (Vector3.Dot(p, p) >= 1.0f);
            return p;
        }
    }
}
