using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FolderSerializer
{
   internal static class Utils
   {
      public static List<string> GetFilesToSerialize( string directory )
      {
         var filePaths = Directory.GetFiles( directory ).OrderBy( x => Path.GetFileName( x ), StringComparer.OrdinalIgnoreCase ).ToList();
         filePaths.Remove( Assembly.GetExecutingAssembly().Location );

         return filePaths;
      }

      public static IEnumerable<int> ParseNumbersToSkip( string text )
      {
         var parsedNumbers = new List<int>();
         if ( string.IsNullOrEmpty( text ) )
         {
            return parsedNumbers;
         }

         int parsedInt;
         text = text.Trim( ',', ' ' );
         if ( text.Contains( ',' ) )
         {
            foreach ( var value in text.Split( ',' ) )
            {
               if ( int.TryParse( value, out parsedInt ) )
               {
                  parsedNumbers.Add( parsedInt );
               }
            }
         }
         else if ( int.TryParse( text, out parsedInt ) )
         {
            parsedNumbers.Add( parsedInt );
         }

         return parsedNumbers;
      }
   }
}
