using System;
using System.IO;
using Mono.Unix;
using Mono.Unix.Native;

namespace DiskFresh
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string diskName;

            if (args.Length > 0)
            {
                diskName = args[0];
            }
            else
            {
                Console.WriteLine("You must specify a file.");
                return;
            }

            var disk = new DiskAccess(diskName);

            const ulong blockSize = 512; // this will normally be 512 in Linux (maybe allow setting it differently later?)
            const ulong clusterSize = 8; // number of blocks to read/write (maybe allow settings via command line?)
            const ulong bufferSize = clusterSize * blockSize;

            var buffer = new byte[bufferSize];

            Console.WriteLine($"Opening {diskName}");

            try
            {
                disk.Open();
            }
            catch (Exception E)
            {
                Console.WriteLine($"Unable to open {diskName}. : {E}");
            }

            long readSize;
            long position = 0;

            do
            {
                disk.Seek(position);

                readSize = disk.Read(buffer, bufferSize);

                InvertBuffer(buffer, readSize);

                disk.Seek(position);

                disk.Write(buffer, (ulong)readSize);

                disk.Seek(position);

                disk.Read(buffer, (ulong)readSize);

                InvertBuffer(buffer, readSize);

                disk.Seek(position);

                disk.Write(buffer, (ulong)readSize);

                disk.Flush();

                position += readSize;
            } while (readSize > 0);

            disk.Close();

        }

        private static void InvertBuffer(byte[] buffer, long size)
        {
            long length = size >= buffer.Length ? buffer.Length : size;

            for (var i = 0; i < length; i++)
            {
                buffer[i] = (byte)~buffer[i];
            }
        }
    }
}
