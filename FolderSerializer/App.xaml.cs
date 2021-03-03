using System.Windows;

namespace FolderSerializer
{
   internal partial class App
   {
      private void OnStartup( object sender, StartupEventArgs e )
      {
         var window = new MainWindow();
         window.ShowDialog();
      }
   }
}
