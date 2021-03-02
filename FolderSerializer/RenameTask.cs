using System.IO;

namespace FolderSerializer
{
   internal sealed class RenameTask
   {
      private readonly string _oldFilePath;
      private readonly string _newFilePath;

      public string OldFileName => Path.GetFileName( _oldFilePath );
      public string NewFileName => Path.GetFileName( _newFilePath );

      public bool Completed { get; private set; }

      public RenameTask( string oldFilePath, string newFilePath )
      {
         _oldFilePath = oldFilePath;
         _newFilePath = newFilePath;
      }

      public bool Execute()
      {
         if ( Completed )
         {
            return true;
         }

         try
         {
            File.Move( _oldFilePath, _newFilePath );
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
