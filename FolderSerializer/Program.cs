using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderSerializer
{
   internal sealed class Program
   {
      private static void Main( string[] args )
      {
         int index = 0;
         IEnumerable<int> numbersToSkip = null;
         var directory = Directory.GetCurrentDirectory();
         while ( index < args.Count() - 1 )
         {
            if ( args[index] == "-s" )
            {
               numbersToSkip = args[index + 1].Split( ',' ).Select( x => int.Parse( x ) );
            }
            else if ( args[index] == "-d" )
            {
               directory = args[index + 1];
            }
            index += 2;
         }

         if ( Serializer.SerializeFilesInDirectory( directory, numbersToSkip ) )
         {
            Console.WriteLine( "Serialization Success" );
         }
         else
         {
            Console.Error.WriteLine( "Serialization Failed" );
         }
      }
   }
}
