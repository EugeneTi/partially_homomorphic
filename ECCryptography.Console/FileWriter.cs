using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptography.Console
{
    public class FileWriter : IDisposable
    {
        private StreamWriter _streamWriter;
        private FileStream _stream;

        public FileWriter()
        {
            _stream = new FileStream("Output.txt", FileMode.Create);
            _streamWriter = new StreamWriter(_stream);
        }

        public void Write(string text)
        {
           _streamWriter.WriteLine(text);
        }

        public void Flush()
        {
            _streamWriter.Flush();
            _stream.Flush(true);
        }

        public void Dispose()
        {
            _streamWriter.Close();
            _stream.Close();

            _streamWriter.Dispose();
            _stream.Dispose();
        }
    }
}
