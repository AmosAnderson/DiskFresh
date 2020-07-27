# DiskFresh

## Purpose
I created this to experiment with the idea of reading from and writing to a disk to force it to correct bad sectors. In the past I have been successful using dd or gddrescue to read the contents of a disk and then write it back in a way that the drive would then reallocate the back sectors and make the disk usable again. More recently I've learned about SpinRite and part of its functionality is doing just this. My idea was to find out what it would take to do just this under Linux using a language that I'm familiar with, C#. C/C++ or even Rust would be better options for this because they're faster with Disk I/O and making system calls. 

## State 
This isn't in a state that I would use it for any production purpose. As mentioned above, it's just an experiment. There are other applications proprietary and open source that can do this work and are better at it. 

The code is simple, if messy, and should be easy enough to navigate. I don't intend to do much more with it than what you see. 

## Details
This uses the Mono.Posix.NETStandard package that gives access to the Linux Syscalls from C#. It was originally a Mono library but Microsoft has since broken it out and ported it to .NET Core and made it available through NuGet. It uses the read, lseek, write, fsync, open, and close Linux syscalls. 

