using System;
using AddressProcessing.CSV;
using Moq;
using NUnit.Framework;

namespace Csv.Tests
{
    [TestFixture]
    public class CSVReaderWriterTests
    {
        private const char Seprator = '\t';
        private const string FileName = "fakeFileName";
        private CSVReaderWriter _sut;
        private Mock<ICsvReader> _csvReaderMock;
        private Mock<ICsvWriter> _csvWriterMock;

        [SetUp]
        public void SetUp()
        {
            _csvReaderMock = new Mock<ICsvReader>();
            _csvWriterMock = new Mock<ICsvWriter>();
            _sut = new CSVReaderWriter(_csvReaderMock.Object, _csvWriterMock.Object);
        }

        [Test]
        public void Open_WriteMode_OpensWriteMode()
        {
            _sut.Open(FileName, CSVReaderWriter.Mode.Write);

            _csvWriterMock.Verify(m => m.Open(FileName));
        }

        [Test]
        public void Open_ReadMode_OpensReadMode()
        {
            _sut.Open(FileName, CSVReaderWriter.Mode.Read);

            _csvReaderMock.Verify(m => m.Open(FileName));
        }

        [Test]
        public void Open_WrongMode_Throws()
        {
            Assert.Throws<Exception>(() => _sut.Open(FileName, 0));
        }

        [Test]
        public void Write_WithParams_WritesToWriter()
        {
            _sut.Write("1", "2", "3");

            _csvWriterMock.Verify(m => m.WriteLine(Seprator, "1", "2", "3"));
        }

        /// <summary>
        /// characterization test that validates the previous behaviour
        /// </summary>
        [Test]
        public void Read_ReadsFromReader()
        {
            _csvReaderMock.Setup(m => m.ReadLine(Seprator))
                .Returns(new[] { "1", "2" });

            string column1 = null, column2 = null;

            _sut.Read(column1, column2);

            Assert.AreEqual(column1, null);
            Assert.AreEqual(column2, null);
        }

        /// <summary>
        /// characterization test that validates the previous behaviour
        /// </summary>
        [Test]
        public void Read_WhenNoColumns_ReturnsNull()
        {
            _csvReaderMock.Setup(m => m.ReadLine(Seprator))
                .Returns(new string[0]);

            string column1 = null, column2 = null;

            var result = _sut.Read(column1, column2);

            Assert.AreEqual(column1, null);
            Assert.AreEqual(column2, null);
            Assert.False(result);
        }

        [Test]
        public void Read_With_OutParameters_ReadsFromReader()
        {
            _csvReaderMock.Setup(m => m.ReadLine(Seprator))
                .Returns(new[] { "1", "2" });

            string column1, column2;

            var result = _sut.Read(out column1, out column2);

            Assert.AreEqual(column1, "1");
            Assert.AreEqual(column2, "2");
            Assert.True(result);
        }

        [Test]
        public void Read_With_OutParameters_WhenReaderReturnsNull_SetsColumnsToNullAndReturnsFalse()
        {
            _csvReaderMock.Setup(m => m.ReadLine(Seprator))
                .Returns(() => null);

            string column1;
            string column2;

            var result = _sut.Read(out column1, out column2);

            Assert.Null(column1);
            Assert.Null(column2);
            Assert.False(result);
        }

        [Test]
        public void Read_With_OutParameters_WhenNoColumns_ReturnsNull()
        {
            _csvReaderMock.Setup(m => m.ReadLine(Seprator))
                .Returns(new string[0]);

            string column1;
            string column2;

            var result = _sut.Read(out column1, out column2);

            Assert.Null(column1);
            Assert.Null(column2);
            Assert.False(result);
        }

        [Test]
        public void Close_ClosesReaderAndWriter()
        {
            _sut.Close();

            _csvReaderMock.Verify(m => m.Close());
            _csvWriterMock.Verify(m => m.Close());
        }

        [Test]
        public void Dispose_DisposesReaderAndWriter()
        {
            _sut.Dispose();

            _csvReaderMock.Verify(m => m.Dispose());
            _csvWriterMock.Verify(m => m.Dispose());
        }
    }
}
