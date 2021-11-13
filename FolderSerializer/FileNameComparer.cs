using System;
using System.Collections.Generic;
using System.Linq;

namespace FolderSerializer
{
   internal sealed class FileNameComparer : IComparer<string>
   {
      public int Compare( string a, string b )
      {
         if ( a.Any( char.IsDigit ) && b.Any( char.IsDigit ) )
         {
            var intA = int.Parse( a.Where( char.IsDigit ).ToArray() );
            var intB = int.Parse( b.Where( char.IsDigit ).ToArray() );

            return intA.CompareTo( intB );
         }
         else
         {
            return StringComparer.OrdinalIgnoreCase.Compare( a, b );
         }
      }
   }
}
