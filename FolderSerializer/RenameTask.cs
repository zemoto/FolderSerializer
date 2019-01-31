using System.IO;

namespace FolderSerializer
{
   internal sealed class RenameTask
   {
      private readonly string _oldFileName;
      private readonly string _newFileName;

      public bool Completed { get; private set; }

      public RenameTask( string oldFileName, string newFileName )
      {
         _oldFileName = oldFileName;
         _newFileName = newFileName;
      }

      public bool Execute()
      {
         if ( Completed )
         {
            return true;
         }

         try
         {
            File.Move( _oldFileName, _newFileName );
            Completed = true;
            return true;
         }
         catch ( IOException )
         {
            return false;
         }
      }
   }
}
