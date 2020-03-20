using System.IO;

namespace AddressProcessing.CSV
{
    public class CsvFileWriter : ICsvWriter
    {
        private StreamWriter _streamWriter = StreamWriter.Null;

        public void Close()
        {
            _streamWriter.Close();
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }

        public void Open(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            _streamWriter = fileInfo.CreateText();
        }

        public void WriteLine(char seprator, params string[] columns)
        {
            _streamWriter.WriteLine(string.Join(seprator.ToString(), columns));
        }
    }
}