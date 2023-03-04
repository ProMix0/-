using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Morphology.Utils
{
    public class FileLinesEnumerable : IEnumerable<string>
    {
        private readonly string _path;

        public FileLinesEnumerable(string path) => 
            _path = path;

        public IEnumerator<string> GetEnumerator() => 
            new Enumerator(new StreamReader(_path, Encoding.UTF8));

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public static IEnumerable<string> Create(string path) => 
            new FileLinesEnumerable(path);

        private class Enumerator : IEnumerator<string>
        {
            private readonly StreamReader _reader;
            private bool _endOfFile;

            public Enumerator(StreamReader reader)
            {
                _reader = reader;
                Current = string.Empty;
            }

            public string Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_endOfFile)
                    return false;

                var line = _reader.ReadLine();
                if (line is null)
                {
                    _endOfFile = true;
                    Current = string.Empty;
                    return false;
                }

                Current = line;
                return true;
            }

            public void Reset()
            {
                _reader.BaseStream.Seek(0, SeekOrigin.Begin);
                _endOfFile = false;
            }

            public void Dispose() => 
                _reader.Dispose();
        }
    }
}