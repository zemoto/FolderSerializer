using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolderSerializer;
using FluentAssertions;

namespace FolderSerializerTests
{
   [TestClass]
   public class FileNameComparerTests
   {
      private static readonly FileNameComparer _comparer = new FileNameComparer();

      [DataTestMethod]
      [DataRow( "1", "10", -1 )]
      [DataRow( "2", "10", -1 )]
      [DataRow( "pre_1", "pre_10", -1 )]
      [DataRow( "1_post", "10_post", -1 )]
      [DataRow( "10", "20", -1 )]
      [DataRow( "1", "2", -1 )]
      [DataRow( "1", "001", 0 )]
      [DataRow( "pre_1", "pre_001", 0 )]
      [DataRow( "pre_1", "pre_002", -1 )]
      [DataRow( "prefix2With0Numbers_1", "prefix2With0Numbers_002", -1 )]
      [DataRow( "prefix2With0Numbers_04_1", "prefix2With0Numbers_04_002", -1 )]
      [DataRow( "prefix2With0Numbers_04_1", "prefix2With0Numbers_05_002", -1 )]
      [DataRow( "a", "b", -1 )]
      [DataRow( "aba", "b", -1 )]
      [DataRow( "0101", "01001", -1 )]
      public void GivenStrings_ReturnExpectedValue( string a, string b, int result ) => _comparer.Compare( a, b ).Should().Be( result );
   }
}
