using System;
using System.Windows;

namespace SharpTracer
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    [STAThread]
    protected override async void OnStartup(StartupEventArgs e)
    {
      var vm = new MainWindowViewModel();
      var view = new MainWindow
      {
        DataContext = vm
      };

      view.Show();
      base.OnStartup(e);

      await vm.RenderImage();
    }
  }
}
