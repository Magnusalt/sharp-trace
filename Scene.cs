using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SharpTracer
{
  public class Scene
  {
    public Scene(int height, int width)
    {
      var lookFrom = new Vector3(13, 2, 3);
      var lookAt = Vector3.Zero;
      var distToFocus = 10f;
      var aperture = 0.1f;
      _camera = new Camera(lookFrom, lookAt, Vector3.UnitY, 20, (float)width / height, aperture, distToFocus);
      _random = new Random();
      var list = new List<Hitable>
            {
                new Sphere(new Vector3(0, 0, -1), 0.5f, new Lambertian(new Vector3(0.1f, 0.2f, 0.5f))),
                new Sphere(new Vector3(0, -100.5f, -1), 100, new Lambertian(new Vector3(0.8f, 0.8f, 0.0f))),
                new Sphere(new Vector3(1, 0, -1), 0.5f, new Metal(new Vector3(0.8f, 0.6f, 0.2f), 0.0f)),
                new Sphere(new Vector3(-1, 0, -1), 0.5f, new Dielectric(Vector3.Zero, 1.5f)),
                new Sphere(new Vector3(-1, 0, -1), -0.45f, new Dielectric(Vector3.Zero, 1.5f))
            };

      var r = (float)Math.Cos(Math.PI / 4);

      var twoSpheres = new List<Hitable>
            {
                new Sphere(new Vector3(0, 0, -1), r, new Lambertian(Vector3.UnitZ)),
            };

      _world = new HitableList(list);
      _vectorCount = Vector<float>.Count;

      _colors = new Vector3[Averaging];
    }
    private const int Averaging = 100;
    private readonly Camera _camera;
    private readonly Random _random;
    private readonly HitableList _world;
    private readonly int _vectorCount;

    private Vector3[] _colors;

    public Vector3 Render(int height, int width, int i, int j)
    {

      for (int a = 0; a < Averaging; a++)
      {
        var us = (i + (float)_random.NextDouble()) / width;
        var vs = (j + (float)_random.NextDouble()) / height;
        var ray = _camera.GetRay(us, vs);
        _colors[a] = Color(ray, 0);
      }

      var x = _colors.Sum(v => v.X) / Averaging;
      var y = _colors.Sum(v => v.Y) / Averaging;
      var z = _colors.Sum(v => v.Z) / Averaging;
      var color = new Vector3(x, y, z);

      return Vector3.SquareRoot(color);
    }

    private Vector3 Color(Ray r, int depth)
    {
      var hit = _world.Hit(r, 0.001f, float.MaxValue);
      if (hit.Hit)
      {
        if (depth < 50 && hit.Material.Scatter(r, hit, out Vector3 attenuation, out Ray scattered))
        {
          return attenuation * Color(scattered, ++depth);
        }
        else
        {
          return Vector3.Zero;
        }
      }

      var unitDirection = Vector3.Normalize(r.Direction);
      var t = 0.5f * (unitDirection.Y + 1.0f);
      return (1.0f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1.0f);
    }

    private List<Hitable> RandomScene()
    {
      var list = new List<Hitable>();
      list.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(new Vector3(0.5f, 0.5f, 0.5f))));

      for (int a = -11; a < 11; a++)
      {
        for (int b = -11; b < 11; b++)
        {
          var material = _random.NextDouble();
          var center = new Vector3((float)(a + 0.9 * _random.NextDouble()), 0.2f, (float)(b + 0.9 * _random.NextDouble()));
          if ((center - new Vector3(4, 0.2f, 0)).Length() > 0.9f)
          {
            if (material < 0.8)
            {
              list.Add(new Sphere(center, 0.2f, new Lambertian(new Vector3((float)(_random.NextDouble() * _random.NextDouble()), (float)(_random.NextDouble() * _random.NextDouble()), (float)(_random.NextDouble() * _random.NextDouble())))));
            }
            else if (material < 0.95)
            {
              list.Add(new Sphere(center, 0.2f,
                  new Metal(new Vector3((float)(0.5f * (1 + _random.NextDouble())), (float)(0.5f * (1 + _random.NextDouble())), (float)(0.5f * (1 + _random.NextDouble()))), 0.5f * (float)_random.NextDouble())));
            }
            else
            {
              list.Add(new Sphere(center, 0.2f, new Dielectric(Vector3.Zero, 1.5f)));
            }
          }
        }
      }
      list.Add(new Sphere(Vector3.UnitY, 1, new Dielectric(Vector3.Zero, 1.5f)));
      list.Add(new Sphere(new Vector3(-4, 1, 0), 1, new Lambertian(new Vector3(0.4f, 0.2f, 0.1f))));
      list.Add(new Sphere(new Vector3(4, 1, 0), 1, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0)));

      return list;
    }
  }
}
