using System.Drawing;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Windows;
using System.Threading.Tasks;

namespace SharpTracer
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    private BitmapImage _bitmap;
    public MainWindowViewModel()
    {
    }

    public Task RenderImage()
    {
      var scene = new Scene(Height, Width);
      var bitmap = new Bitmap(Width, Height);

      for (var j = 1; j <= Height; j++)
      {
        for (var i = 0; i < Width; i++)
        {
          var pixelColor = scene.Render(Height, Width, i, j);
          var ir = (byte)(pixelColor.X * 255.99);
          var ig = (byte)(pixelColor.Y * 255.99);
          var ib = (byte)(pixelColor.Z * 255.99);

          var c = Color.FromArgb(ir, ig, ib);

          bitmap.SetPixel(i, Height - j, c);
        }
      }

      var bmpImage = new BitmapImage();
      var memStream = new MemoryStream();

      bitmap.Save(memStream, ImageFormat.Bmp);
      bmpImage.BeginInit();
      bmpImage.StreamSource = memStream;
      bmpImage.EndInit();

      Bitmap = bmpImage;
      return Task.CompletedTask;
    }

    public int Height => 100;
    public int Width => 200;

    public BitmapImage Bitmap
    {
      get => _bitmap;
      set
      {
        _bitmap = value;
        OnPropertyChanged("Bitmap");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
