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

         var renameTasks = Serializer.CreateRenameTasks( currDirectory, filePaths, numbersToSkip );

         if ( ExecuteRenameTasks( renameTasks ) )
         {
            Console.WriteLine( "Serialization Success" );
         }
         else
         {
            Console.Error.WriteLine( "Serialization Failed" );
         }
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
