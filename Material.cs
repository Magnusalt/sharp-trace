using System;
using System.Numerics;

namespace SharpTracer
{
  public abstract class Material
  {
    protected Random Random { get; }
    public Material(Vector3 albedo)
    {
      Albedo = albedo;
      Random = new Random();
    }

    protected Vector3 Albedo { get; }

    public abstract bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuation, out Ray scattered);
    protected Vector3 RandomInUnitSphere()
    {
      Vector3 vec;
      do
      {
        vec = 2.0f * new Vector3((float)Random.NextDouble(), (float)Random.NextDouble(), (float)Random.NextDouble());
        vec = vec - Vector3.One;
      } while (vec.LengthSquared() >= 1.0f);
      return vec;
    }
    protected Vector3 Reflect(Vector3 v, Vector3 n)
    {
      var vnDot = Vector3.Dot(v, n);
      return v - 2 * vnDot * n;
    }

    protected bool Refract(Vector3 v, Vector3 n, float niOverNt, out Vector3 refracted)
    {
      var uv = Vector3.Normalize(v);
      var dt = Vector3.Dot(uv, n);
      var discriminant = 1.0f - niOverNt * niOverNt * (1 - dt * dt);
      if (discriminant > 0)
      {
        refracted = niOverNt * (uv - n * dt) - n * (float)Math.Sqrt(discriminant);
        return true;
      }
      refracted = default(Vector3);
      return false;
    }
  }

  public class Lambertian : Material
  {
    public Lambertian(Vector3 albedo) : base(albedo)
    {
    }
    public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
    {
      var target = hitRecord.P + hitRecord.Normal + RandomInUnitSphere();
      scattered = new Ray(hitRecord.P, target - hitRecord.P);
      attenuation = Albedo;
      return true;
    }
  }

  public class Metal : Material
  {
    private readonly float _fuzz;

    public Metal(Vector3 albedo, float fuzz) : base(albedo)
    {
      _fuzz = fuzz < 1 ? fuzz : 1;
    }
    public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
    {
      var reflected = Reflect(Vector3.Normalize(rayIn.Direction), hitRecord.Normal);
      scattered = new Ray(hitRecord.P, reflected + _fuzz * RandomInUnitSphere());
      attenuation = Albedo;
      return Vector3.Dot(scattered.Direction, hitRecord.Normal) > 0;
    }


  }

  public class Dielectric : Material
  {
    private readonly float _refractionIndex;

    public Dielectric(Vector3 albedo, float refractionIndex) : base(albedo)
    {
      _refractionIndex = refractionIndex;
    }

    public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
    {
      Vector3 outwardNormal;
      Vector3 reflected = Reflect(rayIn.Direction, hitRecord.Normal);
      float niOverNt;
      attenuation = Vector3.One;
      float reflectProbability;
      float cosine;
      if (Vector3.Dot(rayIn.Direction, hitRecord.Normal) > 0)
      {
        outwardNormal = -1 * hitRecord.Normal;
        niOverNt = _refractionIndex;
        cosine = _refractionIndex * Vector3.Dot(rayIn.Direction, hitRecord.Normal) / rayIn.Direction.Length();
      }
      else
      {
        outwardNormal = hitRecord.Normal;
        niOverNt = 1 / _refractionIndex;
        cosine = -1 * Vector3.Dot(rayIn.Direction, hitRecord.Normal) / rayIn.Direction.Length();
      }

      if (Refract(rayIn.Direction, outwardNormal, niOverNt, out Vector3 refracted))
      {
        reflectProbability = Schlick(cosine, _refractionIndex);
      }
      else
      {
        scattered = new Ray(hitRecord.P, reflected);
        reflectProbability = 1.0f;
      }

      if (Random.NextDouble() < reflectProbability)
      {
        scattered = new Ray(hitRecord.P, reflected);
      }
      else
      {
        scattered = new Ray(hitRecord.P, refracted);
      }
      return true;
    }

    private float Schlick(float cosine, float refIndex)
    {
      var r0 = (1 - refIndex) / (1 + refIndex);
      r0 = r0 * r0;
      return (float)(r0 + (1 - r0) * Math.Pow((1 - cosine), 5));
    }
  }
}
