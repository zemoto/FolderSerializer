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
      public int NumDigits
      {
         get => (int)GetValue( NumDigitsProperty );
         set => SetValue( NumDigitsProperty, value );
      }

      public static readonly DependencyProperty StartingNumberProperty = DependencyProperty.Register(
         nameof( StartingNumber ),
         typeof( int ),
         typeof( MainWindow ),
         new PropertyMetadata( 1, OnSpecialCaseParametersChanged ) );
      public int StartingNumber
      {
         get => (int)GetValue( StartingNumberProperty );
         set => SetValue( StartingNumberProperty, value );
      }

      public static readonly DependencyProperty NumbersToSkipStringProperty = DependencyProperty.Register(
         nameof( NumbersToSkipString ),
         typeof( string ),
         typeof( MainWindow ),
         new PropertyMetadata( string.Empty, OnSpecialCaseParametersChanged ) );
      public string NumbersToSkipString
      {
         get => (string)GetValue( NumbersToSkipStringProperty );
         set => SetValue( NumbersToSkipStringProperty, value );
      }

      private static void OnSpecialCaseParametersChanged( DependencyObject d, DependencyPropertyChangedEventArgs e ) => ( (MainWindow)d ).UpdateRenameTasks();

      public ObservableCollection<RenameTask> RenameTasks { get; } = new ObservableCollection<RenameTask>();

      public MainWindow()
      {
         InitializeComponent();
         UpdateRenameTasks();
      }

      private static readonly Regex _notNumberRegex = new( "[^0-9]+" );
      private void OnPreviewTextInput( object sender, TextCompositionEventArgs e )
      {
         var textBox = (TextBox)sender;
         e.Handled = ( textBox.SelectedText.Length == 0 && textBox.Text.Length >= 5 ) || _notNumberRegex.IsMatch( e.Text );
      }

      private void OnTextChanged( object sender, TextChangedEventArgs e )
      {
         var textBox = (TextBox)sender;
         textBox.Text = textBox.Text.TrimStart( '0' );
      }

      private void OnSerializeButtonClicked( object sender, RoutedEventArgs e )
      {
         _ = MessageBox.Show( Serializer.ExecuteRenameTasks( RenameTasks.ToList() ) ? "Serialization Success" : "Serialization Failed" );
         UpdateRenameTasks();
      }

      private void UpdateRenameTasks()
      {
         RenameTasks.Clear();
         var currentDirectory = Directory.GetCurrentDirectory();
         var numbersToSkip = Utils.ParseNumbersToSkip( NumbersToSkipString );
         var filePaths = Utils.GetFilesToSerialize( currentDirectory );

         var minimumNumDigits = (int)Math.Floor( Math.Log10( filePaths.Count + StartingNumber ) + 1 );
         var numDigits = Math.Max( minimumNumDigits, NumDigits );
         foreach ( var task in Serializer.CreateRenameTasks( currentDirectory, filePaths, StartingNumber, numDigits, numbersToSkip ) )
         {
            RenameTasks.Add( task );
         }
      }

      private void OnAddShellExtensionButtonClicked( object sender, RoutedEventArgs e ) => Utils.AddShellExtension();

      private void OnRemoveShellExtensionButtonClicked( object sender, RoutedEventArgs e ) => Utils.RemoveShellExtension();
   }
}