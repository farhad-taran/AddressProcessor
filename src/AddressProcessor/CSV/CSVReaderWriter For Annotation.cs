using System;
using System.IO;

namespace AddressProcessing.CSV
{
    /*
        1) List three to five key concerns with this implementation that you would discuss with the junior developer. 

        Please leave the rest of this file as it is so we can discuss your concerns during the next stage of the interview process.
        
        ) The streams in this class need to be disposed of, and this class should implement the Idisposable interface, since the instructions are to not break the contract of the class,
        * then a compromise has to be made in the way that the disposable version of CSVReaderWriterForAnnotation would be used and that means even if this class implements the IDisposable interface 
        * the consumer of the class cannot put it in a using statement
        
        *)there is temporal coupling on the open and close methods. meaning the caller has to always call these methods in correct order.
        
        *) bad design decision made by not implementing the IDisposable interface. Implementing the IDisposable interface also gives a good indicator to the client that this class has
        * resources that need to be disposed of and abides to the rules of Designing By Contract. 
        * since the contract of the class cannot be changed as stated in the instructions then the closest best thing to do is to implement the IDisposable interface and
        * make sure that the Close method is called from within the dispose method, this makes sure that in a case where a consumer forgets to call the close method then the garbage collector will make sure that
        * the resources are disposed.
        
        *)the class breaks the single responsibility principle from SOLID, it provides functionality for writing and reading to a file using a Mode flag.
        * the use of primitive types makes the code procedural by forcing us to use if and else statements and to throw exceptions in the default case for the mode flag.
        * the streams are also set to null, these should be set to the NullObject Pattern implementation provided by StreamReader.Null, this would mean that we wont need to check for nulls in the close method;
        * the class also breaks the Open-Close Principle, it is only able to work with Tab delimited files which are not actually CSV but rather TSV, and it is only able to read and write to 2 columns, this means that the class 
        * needs to be opened up and modifications have to be made to it every time the requirements change,
        * we can make this class open to extension and closed to modification by passing or injecting the variants into the class.
        
        *)I can see that the _readerStream.Read and _writerStream.WriteLine are abstracted inside two functions, in the current implementation this seems like extra unnecessary complexity as this does not provide any benefit 
        * and make the code more complicated by delgating the call to another function, breaking the law of demeter, this would have been more beneficial if for example those two operations were to be injected as an Action<string> and a Func<string,string>, this
        * would then allow us to inject different types of mechanisims for reading and writing to not just files but any type of storage, again making the implementation abide by Open-Close, this would also enable us to mock those functions 
        * in unit testing scenarios.
        
        *) I Can see two methods called Read, they both set the values for column one and two, but in different ways, and return a bool indicating whether the operation was successful, the first change I would suggest is to get rid of the version where the values are set by reference,
        * this is dangerous as it leads to Aliasing bugs, I would prefer to use the out praramters in this situation as it would abide better to the principle of Design By Contract, it indicates clearly to the caller that the values passed in will be changed by requiring the out parameters,
        * also having two ways of doing the same thing is bad, we now have two methods that we need to maintain and keep in sync, two possible points of failure.
        * again this method does not abide by Open-Closed principle because what if we need to map more than two column, and from the looks of the mailshot api, this will be required. 
        * I would suggest injecting a mapper into the class that knows how and which columns to map.
        
        *) there is no need to use the Flags attribute on the enums, Flags enumerations are used for masking bit fields and doing bitwise comparisons. They are the correct design to use when multiple enumeration values can be specified at the same time and this is not the case here.
        
        *)the open method assumes that the calling porocess has all the required permissions to open,read or write to files and does not catch any of the exceptions that might be thrown by the File.OpenText and fileInfo.CreateText(), also the fileInfo.CreateText() overwrites the content of an existing file, these might be valid
        * in the current context of the application but it is best to catch exceptions early and return domain specific exceptions where possible because the info in the current unhandled exceptions wont make much sense in an enterprise log aggregation feed or in a layer above the current one.
    */

    public class CSVReaderWriterForAnnotation
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            if (mode == Mode.Read)
            {
                _readerStream = File.OpenText(fileName);
            }
            else if (mode == Mode.Write)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                _writerStream = fileInfo.CreateText();
            }
            else
            {
                throw new Exception("Unknown file mode for " + fileName);
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
                    outPut += "\t";
                }
            }

            WriteLine(outPut);
        }

        public bool Read(string column1, string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();
            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        private void WriteLine(string line)
        {
            _writerStream.WriteLine(line);
        }

        private string ReadLine()
        {
            return _readerStream.ReadLine();
        }

        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }
    }
}
