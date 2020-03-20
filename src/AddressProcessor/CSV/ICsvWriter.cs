using System;

namespace AddressProcessing.CSV
{
    public interface ICsvWriter : IDisposable
    {
        void Open(string fileName);
        void WriteLine(char seprator, params string[] columns);
        void Close();
    }
}