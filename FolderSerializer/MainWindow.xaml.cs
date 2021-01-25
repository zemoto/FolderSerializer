using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace FolderSerializer
{
   public partial class MainWindow
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private void OnButtonClick( object sender, RoutedEventArgs e )
      {
         //TODO 
         // - Starting Number
         // - Digits
         // - Numbers to Skip

         int startingNumber = 1;
         int numDigits = -1;
         IEnumerable<int> numbersToSkip = null;

         var directory = Directory.GetCurrentDirectory();

         var filePaths = Directory.GetFiles( directory ).OrderBy( x => Path.GetFileNameWithoutExtension( x ) ).ToList();
         filePaths.Remove( Assembly.GetExecutingAssembly().Location );

         if ( numDigits == -1 )
         {
            numDigits = (int)Math.Floor( Math.Log10( filePaths.Count() ) + 1 );
         }

         if ( Serializer.SerializeFilesInDirectory( directory, filePaths, startingNumber, numDigits, numbersToSkip ) )
         {
            MessageBox.Show( "Serialization Success" );
         }
         else
         {
            MessageBox.Show( "Serialization Failed" );
         }
      }
   }
}
