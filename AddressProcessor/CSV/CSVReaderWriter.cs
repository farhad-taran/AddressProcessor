using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.

        because backwards compatiability must be maintained, then this means that the contract of the class cannot change, and
        therefor it limits us in terms of the amount of refactoring that we can do. I would prefer a solution without temporal coupling and a seprate class for
        reading and writing, also I would pass in the splitting strategy in and not couple the method signature to a limited number of strings, this implementation will have to change once,
        the other methods in the IMailShot interface need implementing because they require extra columns and different mappings, instead of string params we could pass in a polymorphic object which can
        decide on its own how to map columns into its attributes. that is a more open closed approach and decouples this class from any implementation details of its consumers.

        it makes much more sense to read the whole file upfront, this way the stream closes after all the lines have been read.
    */


    public class CsvReaderWriter : IDisposable
    {
        private StreamReader _readerStream = StreamReader.Null;
        private StreamWriter _writerStream = StreamWriter.Null;
        public enum Mode { Read = 1, Write = 2 };

        private char delimeter = '\t';

        
        public void Open(string fileName, Mode mode)
        {
            try
            {
                if (mode == Mode.Read)
                {
                    _readerStream = File.OpenText(fileName);
                }
                else if (mode == Mode.Write)
                {
                    var fileInfo = new FileInfo(fileName);
                    _writerStream = fileInfo.CreateText();
                }
            }
            catch (Exception e)
            {
                throw new CsvReaderWriterException($"Failed to initialize {nameof(CsvReaderWriter)}", e);
            }
        }

        public void Write(params string[] columns)
        {
            string outPut = "";

            for (int i = 0; i < columns.Length; i++)
            {
                outPut += columns[i];
                if ((columns.Length - 1) != i)
                {
                    outPut += delimeter;
                }
            }

            WriteLine(outPut);
        }

        public bool Read(out string column1, out string column2)
        {
            column1 = string.Empty;
            column2 = string.Empty;

            string line = _readerStream.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] columns = line.Split(delimeter);
                if (columns.Length == 0)
                {
                    return false;
                }
                column1 = columns[0];
                column2 = columns[1];

                return true;
            }

            return false;
        }

        private void WriteLine(string line)
        {
            _writerStream.WriteLine(line);
        }

        public void Close()
        {
            _writerStream.Close();
            _readerStream.Close();
        }

        //there are costs associate with disposing an object,
        // so it is best to only dispose an object once.
        private bool _disposed;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                this.Close();
            }

            _disposed = true;
        }
    }
}
