using System;

namespace AddressProcessing.CSV
{
    public class CsvReaderWriterException : ApplicationException
    {
        public CsvReaderWriterException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}