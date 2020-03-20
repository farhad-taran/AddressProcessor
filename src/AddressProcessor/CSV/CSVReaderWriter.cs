using System;
using System.IO;
using System.Linq;

namespace AddressProcessing.CSV
{
    public class CSVReaderWriter : IDisposable
    {
        private const char Seprator = '\t';

        private readonly ICsvReader _csvReader;
        private readonly ICsvWriter _csvWriter;

        /// <summary>
        /// Allows for initialization with csv readers and writers that are not file based, for example
        /// for writing onto csv files hosted on the cloud etc.
        /// </summary>
        /// <param name="csvReader"></param>
        /// <param name="csvWriter"></param>
        public CSVReaderWriter(ICsvReader csvReader, ICsvWriter csvWriter)
        {
            _csvReader = csvReader;
            _csvWriter = csvWriter;
        }

        /// <summary>
        /// Initializes the class with file based csv reader and writer as default 
        /// </summary>
        public CSVReaderWriter() : this(csvReader: new CsvFileReader(), csvWriter: new CsvFileWriter())
        {

        }


        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            switch (mode)
            {
                case Mode.Read:
                    _csvReader.Open(fileName);
                    break;
                case Mode.Write:
                    _csvWriter.Open(fileName);
                    break;
                default:
                    throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public void Write(params string[] columns)
        {
            _csvWriter.WriteLine(Seprator, columns);
        }

        /// <summary>
        /// Only confirms that the line has columns,
        /// In order to get the actual columns use the other method with out params
        /// </summary>
        /// <returns>True if any columns exist are not empty</returns>
        public bool Read(string column1, string column2)
        {
            return Read(out column1, out column2);
        }

        public bool Read(out string column1, out string column2)
        {
            var columns = _csvReader.ReadLine(Seprator);

            column1 = columns?.ElementAtOrDefault(0);
            column2 = columns?.ElementAtOrDefault(1);

            return column1 != null && column2 != null;
        }

        public void Close()
        {
            _csvReader.Close();
            _csvWriter.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _csvReader.Dispose();
                _csvWriter.Dispose();
            }
        }
    }
}
