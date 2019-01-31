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
         int argsIndex = 0;
         IEnumerable<int> numbersToSkip = null;
         var directory = Directory.GetCurrentDirectory();
         while ( argsIndex < args.Count() - 1 )
         {
            if ( args[argsIndex] == "-s" )
            {
               numbersToSkip = args[argsIndex + 1].Split( ',' ).Select( x => int.Parse( x ) );
            }
            if ( args[argsIndex] == "-d" )
            {
               directory = args[argsIndex + 1];
            }
            argsIndex += 2;
         }

         var filePaths = Directory.GetFiles( directory ).ToList();
         filePaths.Remove( Assembly.GetExecutingAssembly().Location );
         var numDigits = (int)Math.Floor( Math.Log10( filePaths.Count ) + 1 );

         var renameTasks = Serializer.CreateRenameTasks( directory, filePaths, numbersToSkip );

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
