// File System Code for COSMOS OS (authored by Ritika, ref to COSMOS github documentation)
// Configuration for Visual Studio and commit history deleted iniial commits by author
// Collaborator @PratikGarai fixed the commit history issue supposed to be faced by collaborators

using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using System.IO;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;

namespace CosmosKernel1
{
    public class Kernel : Sys.Kernel
    {
        CosmosVFS fs = new Sys.FileSystem.CosmosVFS(); // global initialization of vfs
        protected override void BeforeRun() // -- executes once --
        {
            
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs); //registers vfs at the vfs manager, making it usable
            Console.WriteLine("\nCosmos booted successfully. File System Initialized!");
            
        }

        protected override void Run() // -- executes infinitely --
        {
            Console.Write("\n\nEnter the directory number (DOS format) you want to explore: (preferably 0)");
            var dr = Console.ReadLine();
            string drive = String.Concat(dr,":/");
            var list_dir = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing(drive);

            Console.WriteLine("\nEnter your choice... (1-4): ");
            Console.WriteLine("1. See File System Specifications ");
            Console.WriteLine("2. Create a New File ");
            Console.WriteLine("3. Read Files in a Directory ");
            Console.WriteLine("4. Write to an Existing File ");

            Console.Write("Input: ");
            var choice = Console.ReadLine();
            if (int.TryParse(choice, out int n))
            {
                switch (n)
                {
                    case 1:
                        Console.WriteLine("\n1. Showing File System Specifications ");
                        // --- Showing File System Specifications ---

                        long available = Sys.FileSystem.VFS.VFSManager.GetAvailableFreeSpace(drive);
                        // 0:/ is the id of the drive (DOS drive naming system)
                        Console.WriteLine("\nAvailable Free Space: " + available + " bytes");

                        string fs_type = Sys.FileSystem.VFS.VFSManager.GetFileSystemType(drive);
                        Console.WriteLine("\nFile system type: " + fs_type);

                        
                        Console.WriteLine("\nFiles in the current directory " + drive);
                        foreach (var entry in list_dir)
                        {
                            Console.WriteLine(entry.mName);
                        }

                        break;

                    case 2:
                        Console.WriteLine("\n2. Creating a New File ");
                        // --- Creating a file ---
                        try
                        {
                            Console.WriteLine("Enter a file name along with proper extension");
                            string fsname = Console.ReadLine();
                            string drive1 = String.Concat(dr, ":\\");
                            Console.WriteLine("Creating file in the current directory...");
                            var filepath = String.Concat(drive1,fsname);
                            Sys.FileSystem.VFS.VFSManager.CreateFile(@filepath);
                            Console.WriteLine("File created successfully!");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        break;

                    case 3:
                        Console.WriteLine("\n3. Reading Files in a Directory ");
                        // -- reading files in a directory --
                        Console.WriteLine("\nReading the files in this directory ...");
                        try
                        {
                            // var file = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\hello_from_elia.txt"); // for a single file
                            foreach (var entry in list_dir)
                            {
                                var fstream = entry.GetFileStream();
                                var type = entry.mEntryType;
                                if (type == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                                {
                                    byte[] content = new byte[fstream.Length]; //buffer
                                    fstream.Read(content, 0, (int)fstream.Length); //read operation
                                    Console.WriteLine("File name: " + entry.mName);
                                    Console.WriteLine("File size: " + entry.mSize);
                                    Console.Write("Content: ");
                                    // Console.WriteLine(Encoding.Default.GetString(content)); //alternate command
                                    foreach (char ch in content)
                                    {
                                        Console.Write(ch.ToString());
                                    }
                                    Console.WriteLine();
                                }
                            }
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        break;

                    case 4:
                        Console.WriteLine("\n4. Writing to an Existing File ");
                        // --- Writing to an existing file ---
                        try
                        {
                            Console.WriteLine("Enter a file name existing in the directory, along with proper extension");
                            string fsname = Console.ReadLine();
                            string drive1 = String.Concat(dr, ":\\");
                            var filepath = String.Concat(drive1, fsname);
                            Console.WriteLine("Writing to " + fsname);
                            var file = Sys.FileSystem.VFS.VFSManager.GetFile(@filepath);
                            var filestream = file.GetFileStream();
                            if (filestream.CanWrite)
                            {
                                Console.WriteLine("Enter the contents to write to the file");
                                string strinput = Console.ReadLine();
                                byte[] text = Encoding.ASCII.GetBytes(strinput);
                                filestream.Write(text, 0, text.Length);

                            }
                            Console.WriteLine("Written to file successfully!");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        break;

                    default:
                        Console.WriteLine("Input out of bounds!!");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"{choice} is not a number");
            }
        }
    }
}
