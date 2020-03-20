using System;
using System.IO;
using AddressProcessing.CSV;
using NUnit.Framework;

namespace Csv.Tests
{
    [TestFixture]
    public class CsvFileReaderTests
    {
        private const char Seprator = '\t';
        private const string TestInputFile = @"test_data\csvFileReader_data.csv";

        [Test]
        public void Open_WithValidFileName_OpensFileForReading()
        {
            new CsvFileReader().Open(TestInputFile);
        }

        [Test]
        public void Open_WithNoneExistentFileName_Throws()
        {
            try
            {
                new CsvFileReader().Open(@"none existenta file");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<FileNotFoundException>(e);
            }
        }

        [Test]
        public void ReadLine_ReadsLines()
        {
            var sut = new CsvFileReader();
            sut.Open(TestInputFile);

            var firstLine = sut.ReadLine(Seprator);
            var secondLine = sut.ReadLine(Seprator);

            Assert.AreEqual(firstLine[0], @"Shelby Macias");
            Assert.AreEqual(firstLine[1], @"3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England");


            Assert.AreEqual(secondLine[0], @"Porter Coffey");
            Assert.AreEqual(secondLine[1], @"Ap #827-9064 Sapien. Rd.|Palo Alto|Fl.|HM0G 0YR|Scotland");
        }

        [Test]
        public void Close_ClosesStream()
        {
            var sut = new CsvFileReader();
            sut.Open(TestInputFile);
            sut.Close();

            try
            {
                sut.ReadLine(Seprator);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<ObjectDisposedException>(e);
            }
        }

        [Test]
        public void Dispose_ClosesStream()
        {
            var sut = new CsvFileReader();
            sut.Open(TestInputFile);
            sut.Dispose();

            try
            {
                sut.ReadLine(Seprator);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<ObjectDisposedException>(e);
            }
        }
    }
}