using System.Numerics;

namespace SharpTracer
{
  public class Ray
  {
    public Ray(Vector3 origin, Vector3 direction)
    {
      Origin = origin;
      Direction = direction;
    }

    public Vector3 PointAtParameter(float t)
    {
      return Origin + t * Direction;
    }

    public Vector3 Origin { get; }
    public Vector3 Direction { get; }
  }
}
