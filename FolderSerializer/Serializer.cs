using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FolderSerializer
{
   internal static class Serializer
   {
      public static List<RenameTask> CreateRenameTasks( string directory, IEnumerable<string> filePaths, int startingNumber, int numDigits, IEnumerable<int> numbersToSkip )
      {
         var renameTasks = new List<RenameTask>();

         var desiredNumDigits = (int)Math.Floor( Math.Log10( filePaths.Count() ) + 1 );
         numDigits = Math.Max( numDigits, desiredNumDigits );

         int index = startingNumber;
         foreach ( var filePath in filePaths )
         {
            while ( numbersToSkip?.Contains( index ) == true )
            {
               index++;
            }

            var indexString = index.ToString( CultureInfo.InvariantCulture );
            var currNumDigits = (int)Math.Floor( Math.Log10( index ) + 1 );
            var newFileName = currNumDigits < numDigits
                              ? new string( '0', numDigits - currNumDigits ) + indexString + Path.GetExtension( filePath )
                              : indexString + Path.GetExtension( filePath );
            var newFilePath = Path.Combine( directory, newFileName );
            renameTasks.Add( new RenameTask( filePath, newFilePath ) );

            index++;
         }

         return renameTasks;
      }

      public static bool ExecuteRenameTasks( List<RenameTask> renameTasks )
      {
         int tasksRemainingSinceLastAttempt = -1;
         while ( renameTasks.Count > 0 )
         {
            foreach ( var task in renameTasks )
            {
               if ( !task.Execute() )
               {
                  break;
               }
            }
            _ = renameTasks.RemoveAll( x => x.Completed );
            renameTasks.Reverse();

            int tasksRenaming = renameTasks.Count;
            if ( tasksRemainingSinceLastAttempt == tasksRenaming )
            {
               return false;
            }

            tasksRemainingSinceLastAttempt = tasksRenaming;
         }

         return true;
      }
   }
}
