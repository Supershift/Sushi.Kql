using System.Reflection;

namespace Sushi.Kql.UnitTests
{
    public class DataMapScannerTest
    {
        [Fact]
        public void AssemblyScannerTest()
        {
            Assembly assembly = typeof(TestClass).Assembly;

            var dataMapProvider = new DataMapProvider();

            DataMapScanner.Scan(dataMapProvider, assembly);
            var testClassResult = dataMapProvider.GetMapForType<TestClass>();
            var testRecordResult = dataMapProvider.GetMapForType<TestRecord>();
            var internalClassResult = dataMapProvider.GetMapForType<InternalClass>();

            Assert.NotNull(testClassResult);
            Assert.NotNull(testRecordResult);
            Assert.NotNull(internalClassResult);
        }
    }

    public class TestClass
    {
        private TestClass()
        {
            
        }

        public int Id { get; set; }
    }

    public record TestRecord
    {
        public int Id { get; set; }
    }

    internal class InternalClass
    {
        int Id { get; set; }
    }

    public class TestClassMap : DataMap<TestClass>
    {
        
    }

    internal class TestRecordMap : DataMap<TestRecord>
    {
        
    }

    internal class InternalClassMap : DataMap<InternalClass>
    {

    }
}
