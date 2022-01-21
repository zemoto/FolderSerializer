using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FolderSerializer
{
   internal sealed class FileNameComparer : IComparer<string>
   {
      [DllImport( "shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
      private static extern int StrCmpLogicalW( string x, string y );

      public int Compare( string a, string b )
      {
         a = TrimLeadingZerosFromContiguousNumbers( a );
         b = TrimLeadingZerosFromContiguousNumbers( b );

         return StrCmpLogicalW( a, b );
      }

      private static string TrimLeadingZerosFromContiguousNumbers( string dirtyString )
      {
         bool numberTrimmed = false;
         string cleanString = "";
         for ( int i = 0; i < dirtyString.Length; i++ )
         {
            var character = dirtyString[i];
            if ( char.IsDigit( character ) )
            {
               if ( !numberTrimmed )
               {
                  if ( character == '0' && i < dirtyString.Length - 1 && char.IsDigit( dirtyString[i + 1] ) )
                  {
                     continue;
                  }
                  else
                  {
                     numberTrimmed = true;
                  }
               }
            }
            else
            {
               numberTrimmed = false;
            }

            cleanString += character;
         }

         return cleanString;
      }
   }
}
