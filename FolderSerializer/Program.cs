using System;
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
         var numDigits = (int)Math.Floor( Math.Log10( filePaths.Count ) + 1 );

         int index = 1;
         foreach( var filePath in filePaths )
         {
            var currNumDigits = (int)Math.Floor( Math.Log10( index ) + 1 );
            var newFileName = new string( '0', numDigits - currNumDigits ) + index + Path.GetExtension( filePath );
            var newFilePath = Path.Combine( currDirectory, newFileName );
            File.Move( filePath, newFilePath );
            index++;
         }
      }
   }
}
