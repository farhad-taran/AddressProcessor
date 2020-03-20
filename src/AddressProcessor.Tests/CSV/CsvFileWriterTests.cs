using System;
using System.IO;
using AddressProcessing.CSV;
using NUnit.Framework;

namespace Csv.Tests
{
    [TestFixture]
    public class CsvFileWriterTests
    {
        private const char Seprator = '\t';
        private const string TestInputFile = @"test_data\csvFileWriter_data.csv";

        [Test]
        public void Open_WithValidFileName_OpensFileForReading()
        {
            using (var sut = new CsvFileWriter())
            {
                sut.Open(TestInputFile);
            }
        }

        [Test]
        public void Open_WithNoneExistentFileName_Throws()
        {
            try
            {
                using (var sut = new CsvFileWriter())
                {
                    sut.Open(@"none existenta file");
                }
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<IOException>(e);
            }
        }

        [Test]
        public void WriteLine_WritesLines()
        {
            var expectedFirstLine = new[] { "Shelby Macias", "3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England" };
            var expectedSecondLine = new[] { "Porter Coffey", "Ap #827-9064 Sapien. Rd.|Palo Alto|Fl.|HM0G 0YR|Scotland" };

            using (var sut = new CsvFileWriter())
            {
                sut.Open(TestInputFile);
                sut.WriteLine(Seprator, expectedFirstLine);
                sut.WriteLine(Seprator, expectedSecondLine);
            }

            var csvFileReader = new CsvFileReader();
            csvFileReader.Open(TestInputFile);

            var firstLine = csvFileReader.ReadLine(Seprator);
            var secondLine = csvFileReader.ReadLine(Seprator);

            Assert.AreEqual(firstLine[0], expectedFirstLine[0]);
            Assert.AreEqual(firstLine[1], expectedFirstLine[1]);
            Assert.AreEqual(secondLine[0], expectedSecondLine[0]);
            Assert.AreEqual(secondLine[1], expectedSecondLine[1]);
        }

        [Test]
        public void Close_ClosesStream()
        {
            var sut = new CsvFileWriter();
            sut.Open(TestInputFile);
            sut.Close();

            try
            {
                sut.WriteLine(Seprator);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<ObjectDisposedException>(e);
            }
            finally
            {
                sut.Dispose();
            }
        }

        [Test]
        public void Dispose_ClosesStream()
        {
            var sut = new CsvFileWriter();
            sut.Open(TestInputFile);
            sut.Dispose();

            try
            {
                sut.WriteLine(Seprator);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<ObjectDisposedException>(e);
            }
            finally
            {
                sut.Dispose();
            }
        }
    }
}