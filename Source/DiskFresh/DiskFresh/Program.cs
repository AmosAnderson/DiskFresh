using System;
using System.IO;
using Mono.Unix;
using Mono.Unix.Native;

namespace DiskFresh
{
    class Program
    {
        static void Main(string[] args)
        {
            int fileDescriptor;
            string fileName;
            if (args.Length > 0)
            {
                fileName = args[0];
            }
            else
            {
                Console.WriteLine("You must specify a file.");
                return;
            }

            ulong blockSize = 512; // this will normally be 512 in Linux (maybe allow setting it differently later?)
            ulong clusterSize = 8; // number of blocks to read/write (maybe allow settings via command line?)
            ulong bufferSize = clusterSize * blockSize;
            var buffer = new byte[bufferSize];

            Console.WriteLine($"Opening {fileName}");

            fileDescriptor = Syscall.open(fileName, OpenFlags.O_RDWR);

            if (fileDescriptor < 0)
            {
                Stdlib.perror($"Error opening file {fileName}.");
                return;
            }

            long readSize;
            long writeSize;

            do
            {
                unsafe
                {
                    fixed (byte* b = buffer)
                    {

                        readSize = Syscall.read(fileDescriptor, (IntPtr)b, bufferSize);

                        InvertBuffer(buffer, readSize);

                        Syscall.lseek(fileDescriptor, readSize * -1, SeekFlags.SEEK_CUR);

                        writeSize = Syscall.write(fileDescriptor, (IntPtr)b, (ulong)readSize);

                        Syscall.lseek(fileDescriptor, readSize + -1, SeekFlags.SEEK_CUR);

                        readSize = Syscall.read(fileDescriptor, (IntPtr)b, (ulong)readSize);

                        InvertBuffer(buffer, readSize);

                        Syscall.lseek(fileDescriptor, readSize + -1, SeekFlags.SEEK_CUR);

                        writeSize = Syscall.write(fileDescriptor, (IntPtr)b, (ulong)readSize);
                    }

                }

            } while (readSize > 0);

            if (Syscall.fsync(fileDescriptor) < 0)
            {
                Stdlib.perror("Error syncing file buffers.");
                return;
            }


            if (Syscall.close(fileDescriptor) < 0)
            {
                Stdlib.perror($"Error closing file.");
                return;
            }

        }

        private static void InvertBuffer(byte[] buffer, long size)
        {
            long length = size >= buffer.Length ? buffer.Length : size;

            for (int i = 0; i < length; i++)
            {
                buffer[i] = (byte)~buffer[i];
            }
        }
    }
}
