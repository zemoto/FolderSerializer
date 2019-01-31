using System;
using System.Collections.Generic;
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

         IEnumerable<int> numbersToSkip = null;
         if ( args.Count() == 2 && args[0] == "-s" )
         {
            numbersToSkip = args[1].Split( ',' ).Select( x => int.Parse( x ) );
         }

         var serializedFilePaths = Serializer.SerializeFiles( currDirectory, filePaths, numbersToSkip );

         for ( int i = 0; i < filePaths.Count; i++ )
         {
            File.Move( filePaths[i], serializedFilePaths[i] );
         }
      }
   }
}
