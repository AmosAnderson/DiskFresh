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
        private string _fileName;
        private bool _isOpen;

        public DiskAccess(string fileName)
        {
            _fileName = fileName;
        }

        ~DiskAccess()
        {
            if (_isOpen)
            {
                Close();
            }
        }

        public void Open()
        {
            if (_isOpen)
                throw new Exception("Cannot open file, it is already open.");

            int fd = Syscall.open(_fileName, OpenFlags.O_RDWR);

            if (fd < 0)
            {
                UnixMarshal.ThrowExceptionForLastError();
            }
            else
            {
                _isOpen = true;
                _fileDescriptor = fd;
            }
        }

        public void Close()
        {
            int result;

            if (_isOpen)
            {
                result = Syscall.close(_fileDescriptor);

                if (result < 0)
                {
                    UnixMarshal.ThrowExceptionForLastError();
                }
                else
                {
                    _isOpen = false;
                }
            }
        }

        public long Write(byte[] buffer, ulong count)
        {
            long bytesWritten;

            unsafe
            {
                fixed (byte* b = buffer)
                {
                    bytesWritten = Syscall.write(_fileDescriptor, b, count);
                }
            }

            if (bytesWritten < 0)
            {
                UnixMarshal.ThrowExceptionForLastError();
            }

            return bytesWritten;

        }

        public long Read(byte[] buffer, ulong count)
        {
            long bytesRead; 

            unsafe
            {
                fixed (byte* b = buffer)
                {
                    bytesRead = Syscall.read(_fileDescriptor, b, count);
                }
            }

            if (bytesRead < 0)
            {
                UnixMarshal.ThrowExceptionForLastError();
            }

            return bytesRead;
        }

        public long Seek(long offset)
        {
            long resultingOffset;

            resultingOffset = Syscall.lseek(_fileDescriptor, offset, SeekFlags.SEEK_SET);

            if (resultingOffset < 0)
            {
                UnixMarshal.ThrowExceptionForLastError();
            }

            return resultingOffset;
        }

        public void Flush()
        {
            int result;

            result = Syscall.fsync(_fileDescriptor);

            if (result < 0)
            {
                UnixMarshal.ThrowExceptionForLastError();
            }
        }
    }
}
