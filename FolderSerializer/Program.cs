using System.IO;
using System.Linq;
using System.Reflection;

namespace FolderSerializer
{
   internal sealed class Program
   {
      private static void Main( string[] args )
      {
         var currDirectory = Directory.GetCurrentDirectory();
         var filePaths = Directory.GetFiles( currDirectory ).ToList();
         filePaths.Remove( Assembly.GetExecutingAssembly().Location );

         int index = 1;
         foreach( var filePath in filePaths )
         {
            var newFileName = index + Path.GetExtension( filePath );
            var newFilePath = Path.Combine( currDirectory, newFileName );
            File.Move( filePath, newFilePath );
            index++;
         }
      }
   }
}
