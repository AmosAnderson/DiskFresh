using System;
using System.Collections.Generic;
using System.Text;
using Mono.Unix;
using Mono.Unix.Native;

namespace DiskFresh
{
    class DiskAccess
    {
        private int _fileDescriptor;
        private byte[] _buffer;
        private string _fileName;
        private bool _isOpen;

        DiskAccess(string fileName)
        {
            _fileName = fileName;
        }

        public void Open()
        {
            Stdlib.strerror(Stdlib.GetLastError());
        }

        public int Write(byte[] buffer, ulong offset, ulong count)
        {

        }

        public int Read(byte[] buffer, ulong offset, ulong count)
        {
        }
    }
}
