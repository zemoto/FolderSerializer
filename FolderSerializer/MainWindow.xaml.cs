using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FolderSerializer
{
   internal partial class MainWindow
   {
      public static readonly DependencyProperty NumDigitsProperty = DependencyProperty.Register( 
         nameof( NumDigits ), 
         typeof( int ), 
         typeof( MainWindow ), 
         new PropertyMetadata( 1, OnParameterChanged ) );

      private int _minimumNumDigits;
      public int NumDigits
      {
         get { return (int)GetValue( NumDigitsProperty ); }
         set { SetValue( NumDigitsProperty, value ); }
      }

      public static readonly DependencyProperty StartingNumberProperty = DependencyProperty.Register(
         nameof( StartingNumber ),
         typeof( int ),
         typeof( MainWindow ),
         new PropertyMetadata( 1, OnParameterChanged ) );
      public int StartingNumber
      {
         get { return (int)GetValue( StartingNumberProperty ); }
         set { SetValue( StartingNumberProperty, value ); }
      }

      public static readonly DependencyProperty NumbersToSkipStringProperty = DependencyProperty.Register(
         nameof( NumbersToSkipString ),
         typeof( string ),
         typeof( MainWindow ),
         new PropertyMetadata( string.Empty, OnParameterChanged ) );
      public string NumbersToSkipString
      {
         get { return (string)GetValue( NumbersToSkipStringProperty ); }
         set { SetValue( NumbersToSkipStringProperty, value ); }
      }

      private static void OnParameterChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
      {
         ( (MainWindow)d ).UpdateRenameTasks();
      }
      private void UpdateRenameTasks()
      {
         RenameTasks.Clear();
         if ( string.IsNullOrEmpty( SelectedDirectory ) )
         {
            return;
         }

         var numbersToSkip = ParseNumbersToSkipString();
         var filePaths = Directory.GetFiles( SelectedDirectory ).OrderBy( x => Path.GetFileNameWithoutExtension( x ) ).ToList();
         var numDigits = Math.Max( _minimumNumDigits, NumDigits );
         var renameTasks = Serializer.CreateRenameTasks( SelectedDirectory, filePaths, StartingNumber, numDigits, numbersToSkip );

         foreach ( var task in renameTasks )
         {
            RenameTasks.Add( task );
         }
      }

      private IEnumerable<int> ParseNumbersToSkipString()
      {
         var text = NumbersToSkipString;

         var parsedNumbers = new List<int>();
         if ( string.IsNullOrEmpty( text ) )
         {
            return parsedNumbers;
         }

         int parsedInt;
         text = text.Trim( ',', ' ' );
         if ( text.Contains( ',' ) )
         {
            foreach( var value in text.Split( ',' ) )
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

      public static readonly DependencyProperty SelectedDirectoryProperty = DependencyProperty.Register(
         nameof( SelectedDirectory ),
         typeof( string ),
         typeof( MainWindow ) );
      public string SelectedDirectory
      {
         get { return (string)GetValue( SelectedDirectoryProperty ); }
         set { SetValue( SelectedDirectoryProperty, value ); }
      }

      public ObservableCollection<RenameTask> RenameTasks { get; } = new ObservableCollection<RenameTask>();

      public MainWindow() => InitializeComponent();

      private static readonly Regex _notNumberRegex = new Regex( "[^0-9]+" );
      private void OnPreviewTextInput( object sender, TextCompositionEventArgs e )
      {
         var textBox = (TextBox)sender;
         if ( textBox.SelectedText.Length == 0 && textBox.Text.Length >= 5 )
         {
            e.Handled = true;
         }
         else
         {
            e.Handled = _notNumberRegex.IsMatch( e.Text );
            if ( e.Handled )
            {
               SystemSounds.Beep.Play();
            }
         }
      }

      private void OnTextChanged( object sender, TextChangedEventArgs e )
      {
         var textBox = (TextBox)sender;
         textBox.Text = textBox.Text.TrimStart( '0' );
      }

      private void OnSelectDirectoryButtonClicked( object sender, RoutedEventArgs e )
      {
         var selectFolderDialog = new CommonOpenFileDialog()
         {
            IsFolderPicker = true,
            Multiselect = false,
            InitialDirectory = Directory.GetCurrentDirectory(),
         };

         if ( selectFolderDialog.ShowDialog() == CommonFileDialogResult.Ok )
         {
            SelectedDirectory = selectFolderDialog.FileName;

            var filePaths = Directory.GetFiles( SelectedDirectory ).ToList();
            _minimumNumDigits = (int)Math.Floor( Math.Log10( filePaths.Count() ) + 1 );

            if ( NumDigits < _minimumNumDigits )
            {
               NumDigits = _minimumNumDigits;
            }
            else
            {
               UpdateRenameTasks();
            }

            SerializeButton.IsEnabled = true;
         }
      }

      private void OnSerializeButtonClicked( object sender, RoutedEventArgs e )
      {
         var filePaths = Directory.GetFiles( SelectedDirectory ).OrderBy( x => Path.GetFileNameWithoutExtension( x ) ).ToList();

         if ( Serializer.ExecuteRenameTasks( RenameTasks.ToList() ) )
         {
            MessageBox.Show( "Serialization Success" );
         }
         else
         {
            MessageBox.Show( "Serialization Failed" );
         }

         UpdateRenameTasks();
      }
   }
}
