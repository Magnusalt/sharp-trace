using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpTracer
{
  public struct HitRecord
  {
    public HitRecord(bool hit, float t, Vector3 p, Vector3 normal, Material material)
    {
      Hit = hit;
      T = t;
      P = p;
      Normal = normal;
      Material = material;
    }
    public bool Hit { get; set; }
    public float T { get; set; }
    public Vector3 P { get; set; }
    public Vector3 Normal { get; set; }
    public Material Material { get; set; }

  }
  public abstract class Hitable
  {
    public abstract HitRecord Hit(Ray ray, float tMin, float tMax);
  }

  public class Sphere : Hitable
  {
    public Sphere(Vector3 center, float radius, Material material)
    {
      Center = center;
      Radius = radius;
      Material = material;
    }

    public Vector3 Center { get; }
    public float Radius { get; }
    public Material Material { get; }
    public override HitRecord Hit(Ray ray, float tMin, float tMax)
    {
      var oc = ray.Origin - Center;
      var a = Vector3.Dot(ray.Direction, ray.Direction);
      var b = Vector3.Dot(oc, ray.Direction);
      var c = Vector3.Dot(oc, oc) - Radius * Radius;
      var discriminant = b * b - a * c;

      if (discriminant > 0)
      {
        HitRecord CreateHitRecord(float t)
        {
          var p = ray.PointAtParameter(t);
          return new HitRecord(true, t, p, (p - Center) / Radius, Material);
        }
        var temp = (float)(-b - Math.Sqrt(discriminant)) / a;
        if (temp < tMax && temp > tMin)
        {
          return CreateHitRecord(temp);
        }
        temp = (float)(-b + Math.Sqrt(discriminant)) / a;
        if (temp < tMax && temp > tMin)
        {
          return CreateHitRecord(temp);
        }
      }
      return new HitRecord();
    }
  }

  public class HitableList : Hitable
  {
    public HitableList(List<Hitable> hitables)
    {
      Hitables = hitables;
    }

    public List<Hitable> Hitables { get; }

    public override HitRecord Hit(Ray ray, float tMin, float tMax)
    {
      var hitRecord = new HitRecord();
      var closestSoFar = tMax;
      foreach (var item in Hitables)
      {
        var itemHit = item.Hit(ray, tMin, closestSoFar);
        if (itemHit.Hit)
        {
          hitRecord.Hit = true;
          closestSoFar = itemHit.T;
          hitRecord = itemHit;
        }
      }

      return hitRecord;
    }
  }
}
