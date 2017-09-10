using System;
using System.Collections.Generic;
using System.IO;
using AddressProcessing.CSV;
using FluentAssertions;
using NUnit.Framework;

namespace AddressProcessing.Tests.CSV
{
    /*
     * you can see why having two responsibilities in the same class can complicate things, this is even reflective in the tests.
     * I have to use two files to test, in this case I dont have to clean up the write file because the way it was implemented was to overwrite an existing file,
     * which might have been the original intention and I have not been told otherwise.
     */

    [TestFixture]
    public class CsvReaderWriterTests
    {
        private const string TestInputFile = @"test_data\CSVReaderWriterTests_contacts.csv";
        private const string CsvWritesTestFile = @"test_data\CSV_Writes.csv";

        readonly List<Tuple<string, string>> _expectedColumns = new List<Tuple<string, string>>
        {
            new Tuple<string,string>("Shelby Macias","3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England"),
            new Tuple<string,string>("Porter Coffey","Ap #827-9064 Sapien. Rd.|Palo Alto|Fl.|HM0G 0YR|Scotland"),
            new Tuple<string,string>("Noelani Ward","637-911 Mi Rd.|Monrovia|MB|M5M 6SC|Scotland"),
            new Tuple<string,string>("Lillian Cotton","Ap #210-1906 Integer Ave|Caguas|SXW|GG2 3OZ|Bosnia and Herzegovina")
        };

        [Test]
        public void Disposed_ClosesReaderStream()
        {
            ObjectDisposedException expectedException = null;
            try
            {
                using (var sut = new CsvReaderWriter())
                {
                    sut.Open(TestInputFile, CsvReaderWriter.Mode.Read);

                    sut.Dispose();

                    string columnOne, columnTwo;

                    var success = sut.Read(out columnOne, out columnTwo);
                    success.Should().BeFalse();
                }
            }
            catch (ObjectDisposedException ode)
            {
                expectedException = ode;
            }

            expectedException.Should().NotBeNull("should not be able to reuse streams if object has been disposed.");
        }

        [Test]
        public void Disposed_ClosesWriterStream()
        {
            ObjectDisposedException expectedException = null;
            try
            {
                using (var sut = new CsvReaderWriter())
                {
                    sut.Open(CsvWritesTestFile, CsvReaderWriter.Mode.Write);
                    sut.Dispose();
                    sut.Write();
                }
            }
            catch (ObjectDisposedException ode)
            {
                expectedException = ode;
            }

            expectedException.Should().NotBeNull("should not be able to reuse streams if object has been disposed.");
        }

        [Test]
        public void Read_With_Out_Params_Reads_First_Line()
        {
            using (var sut = new CsvReaderWriter())
            {
                sut.Open(TestInputFile, CsvReaderWriter.Mode.Read);

                string columnOne, columnTwo;

                var success = sut.Read(out columnOne, out columnTwo);

                success.Should().BeTrue();
                columnOne.Should().BeEquivalentTo("Shelby Macias");
                columnTwo.Should().BeEquivalentTo("3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England");
            }
        }

        [Test]
        public void Read_With_Out_Params_Reads_Multiple_Lines()
        {
            using (var sut = new CsvReaderWriter())
            {
                sut.Open(TestInputFile, CsvReaderWriter.Mode.Read);

                _expectedColumns.ForEach(t =>
                {
                    string columnOne, columnTwo;

                    var success = sut.Read(out columnOne, out columnTwo);

                    success.Should().BeTrue();
                    columnOne.Should().BeEquivalentTo(t.Item1);
                    columnTwo.Should().BeEquivalentTo(t.Item2);
                });
            }
        }

        [Test]
        public void Open_For_Any_Exception_Throws_Domain_Exception()
        {
            Exception expectedException = null;
            try
            {
                using (var sut = new CsvReaderWriter())
                {
                    sut.Open("None Existent file to cause exception", CsvReaderWriter.Mode.Read);
                }
            }
            catch (Exception e)
            {
                expectedException = e;
            }

            expectedException.Should().NotBeNull();
            expectedException.Should().BeOfType<CsvReaderWriterException>();
            expectedException?.InnerException.Should().BeOfType<FileNotFoundException>();
        }

        [Test]
        public void Write_Can_Write_Multiple_Lines()
        {
            using (var sut = new CsvReaderWriter())
            {
                sut.Open(CsvWritesTestFile, CsvReaderWriter.Mode.Write);

                _expectedColumns.ForEach(t =>
                {
                    sut.Write(t.Item1, t.Item2);
                });
            }

            using (var sut = new CsvReaderWriter())
            {
                sut.Open(CsvWritesTestFile, CsvReaderWriter.Mode.Read);

                _expectedColumns.ForEach(t =>
                {
                    string columnOne, columnTwo;

                    var success = sut.Read(out columnOne, out columnTwo);

                    success.Should().BeTrue();
                    columnOne.Should().BeEquivalentTo(t.Item1);
                    columnTwo.Should().BeEquivalentTo(t.Item2);
                });
            }
        }
    }
}
