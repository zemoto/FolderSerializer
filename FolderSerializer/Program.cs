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

         var serializedFilePaths = Serializer.SerializeFiles( currDirectory, filePaths );

         for ( int i = 0; i < filePaths.Count; i++ )
         {
            File.Move( filePaths[i], serializedFilePaths[i] );
         }
      }
   }
}
