using System;
using System.IO;

namespace AddressProcessing.CSV
{
    public class CsvFileReader : ICsvReader
    {
        private StreamReader _streamReader;

        public CsvFileReader()
        {
            _streamReader = StreamReader.Null;
        }

        public void Close()
        {
            _streamReader.Close();
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
                _streamReader.Dispose();
            }
        }

        public void Open(string fileName)
        {
            _streamReader = File.OpenText(fileName);
        }

        public string[] ReadLine(char seprator)
        {
            return _streamReader.ReadLine()?.Split(seprator);
        }
    }
}