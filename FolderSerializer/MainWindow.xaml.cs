using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
         new PropertyMetadata( 1, OnSpecialCaseParametersChanged ) );

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
         new PropertyMetadata( 1, OnSpecialCaseParametersChanged ) );
      public int StartingNumber
      {
         get { return (int)GetValue( StartingNumberProperty ); }
         set { SetValue( StartingNumberProperty, value ); }
      }

      public static readonly DependencyProperty NumbersToSkipStringProperty = DependencyProperty.Register(
         nameof( NumbersToSkipString ),
         typeof( string ),
         typeof( MainWindow ),
         new PropertyMetadata( string.Empty, OnSpecialCaseParametersChanged ) );
      public string NumbersToSkipString
      {
         get { return (string)GetValue( NumbersToSkipStringProperty ); }
         set { SetValue( NumbersToSkipStringProperty, value ); }
      }

      private static void OnSpecialCaseParametersChanged( DependencyObject d, DependencyPropertyChangedEventArgs e ) => ( (MainWindow)d ).UpdateRenameTasks();

      public ObservableCollection<RenameTask> RenameTasks { get; } = new ObservableCollection<RenameTask>();

      public MainWindow()
      {
         InitializeComponent();
         InitializeRenameTasks();
      }

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
         }
      }

      private void OnTextChanged( object sender, TextChangedEventArgs e )
      {
         var textBox = (TextBox)sender;
         textBox.Text = textBox.Text.TrimStart( '0' );
      }

      private void OnSerializeButtonClicked( object sender, RoutedEventArgs e )
      {
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

      private void InitializeRenameTasks()
      {
         var filePaths = Utils.GetFilesToSerialize( Directory.GetCurrentDirectory() );
         _minimumNumDigits = (int)Math.Floor( Math.Log10( filePaths.Count() ) + 1 );

         if ( NumDigits < _minimumNumDigits )
         {
            NumDigits = _minimumNumDigits;
         }
         else
         {
            UpdateRenameTasks();
         }
      }

      private void UpdateRenameTasks()
      {
         RenameTasks.Clear();
         var currentDirectory = Directory.GetCurrentDirectory();
         var numbersToSkip = Utils.ParseNumbersToSkip( NumbersToSkipString );
         var filePaths = Utils.GetFilesToSerialize( currentDirectory );

         var numDigits = Math.Max( _minimumNumDigits, NumDigits );
         var renameTasks = Serializer.CreateRenameTasks( currentDirectory, filePaths, StartingNumber, numDigits, numbersToSkip );

         foreach ( var task in renameTasks )
         {
            RenameTasks.Add( task );
         }
      }
   }
}