using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace FolderSerializer
{
   internal static class Utils
   {
      private const string ShellRegKey = @"Software\Classes\Directory\Background\shell\FolderSerializer";
      private const string ShellCommandRegKey = ShellRegKey + @"\command";

      public static List<string> GetFilesToSerialize( string dir ) => Directory.EnumerateFiles( dir ).OrderBy( Path.GetFileName, new FileNameComparer() ).ToList();

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

      public static void AddShellExtension()
      {
         if ( ShellExtensionRegistered() )
         {
            _ = MessageBox.Show( "Already added" );
            return;
         }

         using var key = Registry.CurrentUser.CreateSubKey( ShellRegKey );
         key.SetValue( "", "Serialize Folder" );

         using var subkey = Registry.CurrentUser.CreateSubKey( ShellCommandRegKey );
         subkey.SetValue( "", $"{Directory.GetCurrentDirectory()}\\FolderSerializer.exe \"%V\"" );

         _ = MessageBox.Show( "Shell Extension added" );
      }

      public static void RemoveShellExtension()
      {
         if ( ShellExtensionRegistered() )
         {
            Registry.CurrentUser.DeleteSubKeyTree( ShellRegKey );
            _ = MessageBox.Show( "Shell Extension removed" );
            return;
         }

         _ = MessageBox.Show( "Already removed" );
      }

      private static bool ShellExtensionRegistered()
      {
         using var existingKey = Registry.CurrentUser.OpenSubKey( ShellRegKey, false );
         return existingKey != null;
      }
   }
}
