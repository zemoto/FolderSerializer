using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderSerializer
{
   internal static class Serializer
   {
      public static List<string> SerializeFiles( string directory, IEnumerable<string> filePaths, IEnumerable<int> numbersToSkip )
      {
         var newFilePaths = new List<string>();

         var numDigits = (int)Math.Floor( Math.Log10( filePaths.Count() ) + 1 );

         int index = 1;
         foreach ( var filePath in filePaths )
         {
            var currNumDigits = (int)Math.Floor( Math.Log10( index ) + 1 );
            var newFileName = new string( '0', numDigits - currNumDigits ) + index + Path.GetExtension( filePath );
            var newFilePath = Path.Combine( directory, newFileName );
            newFilePaths.Add( newFilePath );

            do
            {
               index++;
            }
            while ( numbersToSkip?.Contains( index ) == true );
         }

         return newFilePaths;
      }
   }
}
