using System;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public interface ICsvReader : IDisposable
    {
        void Open(string fileName);
        string[] ReadLine(char seprator);
        void Close();
    }
}
