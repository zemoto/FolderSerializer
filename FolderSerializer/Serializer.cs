using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FolderSerializer
{
   internal static class Serializer
   {
      public static bool SerializeFilesInDirectory( string directory, IEnumerable<int> numbersToSkip )
      {
         var filePaths = Directory.GetFiles( directory ).ToList();
         filePaths.Remove( Assembly.GetExecutingAssembly().Location );

         var renameTasks = CreateRenameTasks( directory, filePaths, numbersToSkip );

         return ExecuteRenameTasks( renameTasks );
      }

      private static List<RenameTask> CreateRenameTasks( string directory, IEnumerable<string> filePaths, IEnumerable<int> numbersToSkip )
      {
         var renameTasks = new List<RenameTask>();

         var numDigits = (int)Math.Floor( Math.Log10( filePaths.Count() ) + 1 );

         int index = 1;
         foreach ( var filePath in filePaths )
         {
            var currNumDigits = (int)Math.Floor( Math.Log10( index ) + 1 );
            var newFileName = new string( '0', numDigits - currNumDigits ) + index + Path.GetExtension( filePath );
            var newFilePath = Path.Combine( directory, newFileName );
            renameTasks.Add( new RenameTask( filePath, newFilePath ) );

            do
            {
               index++;
            }
            while ( numbersToSkip?.Contains( index ) == true );
         }

         return renameTasks;
      }

      private static bool ExecuteRenameTasks( List<RenameTask> renameTasks )
      {
         int tasksRemainingSinceLastAttempt = -1;
         while ( renameTasks.Any() )
         {
            foreach ( var task in renameTasks )
            {
               if ( !task.Execute() )
               {
                  break;
               }
            }
            renameTasks.RemoveAll( x => x.Completed );
            renameTasks.Reverse();

            int tasksRenaming = renameTasks.Count();
            if ( tasksRemainingSinceLastAttempt == tasksRenaming )
            {
               return false;
            }
            else
            {
               tasksRemainingSinceLastAttempt = tasksRenaming;
            }
         }

         return true;
      }
   }
}
