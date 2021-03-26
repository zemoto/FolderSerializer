using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace FolderSerializer
{
   internal static class Utils
   {
      private const string ShellRegKey = @"Software\Classes\Directory\Background\shell\FolderSerializer";
      private const string ShellCommandRegKey = ShellRegKey + @"\command";

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

      public static void AddShellExtension()
      {
         if ( RegistryKeyExists( ShellRegKey ) )
         {
            MessageBox.Show( "Already added" );
            return;
         }

         using ( var key = Registry.CurrentUser.CreateSubKey( ShellRegKey ) )
         {
            key.SetValue( "", "Serialize Folder" );
            using ( var subkey = Registry.CurrentUser.CreateSubKey( ShellCommandRegKey ) )
            {
               subkey.SetValue( "", $"{Assembly.GetExecutingAssembly().Location} \"%V\"" );
               MessageBox.Show( "Shell Extension added" );
            }
         }
      }

      public static void RemoveShellExtension()
      {
         if ( RegistryKeyExists( ShellRegKey ) )
         {
            Registry.CurrentUser.DeleteSubKeyTree( ShellRegKey );
            MessageBox.Show( "Shell Extension removed" );
            return;
         }

         MessageBox.Show( "Already removed" );
      }

      private static bool RegistryKeyExists( string path )
      {
         using ( var existingKey = Registry.CurrentUser.OpenSubKey( ShellRegKey, false ) )
         {
            return existingKey != null;
         }
      }
   }
}
