using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Sys = Cosmos.System;

namespace FirstOS
{
    public class Kernel : Sys.Kernel
    {
        string pass = "abc123";
        string error = "Unknown command";
        public static string file;
        public bool FS = true;
        string current_path = @"0:\";
        public bool SudoY = true;

        public void deleteFile(string fname)
        {
            if (File.Exists(fname))
            {
                File.Delete(fname);
            }
            else
            {
                Console.WriteLine("File doesn't exist!");
            }
        }

        public void deleteDirectory(string fname)
        {
            if (Directory.Exists(fname))
            {
                Directory.Delete(fname);
            }
            else
            {
                Console.WriteLine("Directory doesn't exist!");
            }
        }

        public void copyFile(string path, string fname, string destination)
        {
            if (File.Exists(fname) && Directory.Exists(destination))
            {
                File.Copy(path + fname, destination);
            }
            else
            {
                Console.WriteLine("File or Directory doesn't exist!");
            }
        }
        public void moveFile(string path, string fname, string destination)
        {
            copyFile(path, fname, destination);
            deleteFile(path + fname);
        }
        private string FileExists(string directory, string filename)
        {
            string exists = null;
            var fileNameToCheck = Path.Combine(directory, filename);
            if (Directory.Exists(directory))
            {
                foreach (var dir in Directory.GetFiles(directory))
                {
                    if (dir == filename)
                    {
                        exists = Path.Combine(directory, dir);
                    }
                }
                if (exists == null)
                {
                    foreach (var dir in Directory.GetDirectories(directory))
                    {
                        var dirNew = Path.Combine(directory, dir);
                        exists = FileExists(dirNew, filename);

                        if (exists != null) break;
                    }
                }
            }
            return exists;
        }
        protected override void BeforeRun()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
        file_system:
            Console.Write("Do you want to enable the file system?(Y/N)");
            var filesys = Console.ReadLine();
            if (filesys == "Y")
            {
                FS = true;
                Console.Write("Initializing...");
                var fs = new Sys.FileSystem.CosmosVFS(); 
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            }
            else if (filesys == "N")
            {
            }
            else
                goto file_system;
            try
            {
                Sys.KeyboardManager.SetKeyLayout(new Sys.ScanMaps.US_Standard()); 
            }
            catch (Exception)
            {
                goto logouterr;
            }
        logouterr:
            Console.Clear();
            Console.WriteLine("                                                  ");
            Console.WriteLine("           Successfully Booted                    ");
            Console.WriteLine("                                                  ");

        loginerr:
            Console.WriteLine("Please enter password to log in! (Type N to shutdown)");
            var sino = Console.ReadLine();
            if (sino == pass)
            {
                Console.Clear();
                Console.Write("Welcome!!!\n");
            }
            else if (sino == "N")
            {
                Stop();
            }
            else { goto loginerr; }

        }

        protected override void Run()
        {
            if (!Directory.Exists(@"0:\RecycleBin"))
            {
                Directory.CreateDirectory(@"0:\RecycleBin");
            }

            Console.WriteLine();
            Console.Write(current_path + "@root: ");
            var input = Console.ReadLine();
            var co = input;
            var vars = "";
            if (input.ToLower().IndexOf(' ') != -1)
            {

                string[] parts = input.Split(' ');
                co = parts[0];
                vars = parts[1];
            }
            try
            {
                switch (co)
                {

                    case "reboot":
                        Cosmos.System.Power.Reboot();
                        break;

                    case "shutdown": 
                        Console.WriteLine("Powering Off...");
                        Stop();
                        break;

                    case "clear":
                        Console.Clear();
                        break;


                    case "about":
                        Console.WriteLine("OS 1.0.0");
                        break;

                    case "chdir":
                        if (FS)
                        {
                            if (vars == "")
                            {
                                current_path = @"0:\";
                            }
                            else if (Directory.Exists(vars))
                            {
                                current_path = current_path + vars;
                            }
                            else
                            {
                                Console.WriteLine("Directory Doesn't Exists");
                            }
                        }
                        else
                        {
                            Console.WriteLine("File System Not Enabled!");
                        }
                        break;
                    case "mkdir":  // Makes new directory
                        if (FS)

                            {
                                filesystem.createDir(current_path + vars);
                            }
                            else
                            {
                                Console.WriteLine("File System Not Enabled!");
                            }
                        break;

                    case "ls": // Displays current location
                        if (FS)
                        {
                            string[] back = filesystem.readFiles(current_path);
                            string[] front = filesystem.readDirectories(current_path);
                            string[] combined = new string[front.Length + back.Length];
                            Array.Copy(front, combined, front.Length);
                            Array.Copy(back, 0, combined, front.Length, back.Length);
                            foreach (var item in combined)
                            {
                                Console.WriteLine(item.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("File System Not Enabled!");
                        }
                        break;

                    case "add": // Adds given numbers
                        string[] inputvarsa = vars.Split('+');
                        Console.WriteLine(Calculator.Add(inputvarsa[0], inputvarsa[1]));
                        break;

                    case "subtract": // Subtracts given numbers
                        string[] inputvarsb = vars.Split('-');
                        Console.WriteLine(Calculator.Subtract(inputvarsb[0], inputvarsb[1]));
                        break;

                    case "multiply": // Multiplys given numbers
                        string[] inputvarsc = vars.Split('*');
                        Console.WriteLine(Calculator.Multiply(inputvarsc[0], inputvarsc[1]));
                        break;

                    case "divide": // Divides given numbers
                        string[] inputvarsd = vars.Split('/');
                        Console.WriteLine(Calculator.Divide(inputvarsd[0], inputvarsd[1]));
                        break;

                    case "power": // Raises given number to other given number
                        string[] inputvarse = vars.Split('^');
                        Console.WriteLine(Calculator.ToPower(inputvarse[0], inputvarse[1]));
                        break;

                    case "gcd": // Gives gcd conversion of given numbers
                        string[] inputvarsf = vars.Split(',');
                        Console.WriteLine(Calculator.GcdCon(inputvarsf[0], inputvarsf[1]));
                        break;

                    case "lcm": // Gives lcm conversion of given numbers
                        string[] inputvarsg = vars.Split(',');
                        Console.WriteLine(Calculator.LcmCon(inputvarsg[0], inputvarsg[1]));
                        break;

                    case "text_editor": //text_editor
                        Console.Clear();
                        File.AppendAllText(@"0:\history", input);
                        text_editor.init(current_path);
                        break;

                    case "open_file": //open a file
                        Console.WriteLine("Enter filename of file to read:");
                        var file = Console.ReadLine();
                        File.AppendAllText(@"0:\history", input + " " + file + "\n");
                        string[] read;
                        read = File.ReadAllLines(current_path + file);
                        foreach (string s in read)
                        {
                            Console.WriteLine(s);
                        }
                        break;


                    case "rmdir": //delete directory
                        if (SudoY)
                        {
                            Console.WriteLine("Enter name of directory to be deleted:");
                            var directory = Console.ReadLine();
                            File.AppendAllText(@"0:\history", input + " " + directory + "\n");
                            deleteDirectory(current_path + directory);
                        }
                        else
                        {
                            Console.WriteLine("I'm sorry, you aren't a sudo user");
                        }
                        break;

                    case "df":
                        if (SudoY)
                        {
                            Console.WriteLine("Enter name of file to be deleted:");
                            var filename = Console.ReadLine();
                            File.AppendAllText(@"0:\history", input + " " + filename + "\n");
                            deleteFile(current_path + filename);
                        }
                        else
                        {
                            Console.WriteLine("I'm sorry, you aren't a sudo user");
                        }
                        break;
                    case "ds":
                        foreach (DriveInfo drive in DriveInfo.GetDrives())
                        {
                            if (drive.IsReady)
                            {
                                Console.WriteLine("Available free space(in Bytes):");
                                Console.WriteLine(drive.TotalFreeSpace + drive.Name);
                            }
                        }
                        break;
                    case "mv":
                        Console.WriteLine("Enter name of file to be moved:");
                        var movefile = Console.ReadLine();
                        Console.WriteLine("Enter path to where file is to be moved:");
                        var movePath = Console.ReadLine();
                        File.AppendAllText(@"0:\history", input + " " + movefile + " " + movePath + "\n");
                        moveFile(current_path, movefile, movePath);
                        break;

                    case "cp":
                        Console.WriteLine("Enter name of file to be copied:");
                        var copyfile = Console.ReadLine();
                        Console.WriteLine("Enter path to where file is to be copied:");
                        var copyPath = "";
                        copyPath = Console.ReadLine();
                        File.AppendAllText(@"0:\history", input + " " + copyfile + " " + copyPath + "\n");
                        copyFile(current_path, copyfile, copyPath);
                        break;

                    case "date":
                        File.AppendAllText(@"0:\history", input + "\n");
                        DateTime now = DateTime.Now;
                        Console.WriteLine(now);
                        break;
                    case "cat":
                        Console.WriteLine("Enter original file name:");
                        string origfname = Console.ReadLine();
                        //File.Create(current_path + fname);
                        Console.WriteLine("Enter filename of file to read:");
                        var fileCon = Console.ReadLine();
                        File.AppendAllText(@"0:\history", input + " " + origfname + " " + fileCon + "\n");
                        string[] readCon;
                        readCon = File.ReadAllLines(current_path + fileCon);
                        foreach (string str in readCon)
                        {
                            File.AppendAllText(current_path + origfname, str);
                        }
                        break;
                    case "search":
                        Console.WriteLine("Enter name of file to search:");
                        string fsearch = Console.ReadLine();
                        File.AppendAllText(@"0:\history", input + " " + fsearch + "\n");
                        var answer = FileExists(@"0:\", fsearch);
                        if (answer == null)
                        {
                            Console.WriteLine("File Doesn't Exist");
                        }
                        else
                        {
                            Console.WriteLine("File Found!!!");
                            Console.WriteLine(answer);
                        }
                        break;
                    case "rm":
                        Console.WriteLine("Enter file to be moved to recycle bin:");
                        var reclFile = Console.ReadLine();
                        File.AppendAllText(@"0:\history", input + " " + reclFile + "\n");
                        moveFile(current_path, reclFile, @"0:\RecycleBin\");
                        break;
                    case "clearbin":
                        File.AppendAllText(@"0:\history", input + "\n");
                        foreach (var fileRecl in Directory.GetFiles(@"0:\RecycleBin\"))
                        {
                            deleteFile(@"0:\RecycleBin\" + fileRecl);
                        }
                        break;
                    case "history":
                        string[] readHis;
                        readHis = File.ReadAllLines(@"0:\history");
                        foreach (string s in readHis)
                        {
                            Console.WriteLine(s);
                        }
                        Console.WriteLine();
                        break;

                    default:
                        Console.WriteLine(error);
                        break;
                }
            }
            catch (Exception e) //BlueScreenOfDeath-like thing I wanted to make noerror false but it bugs
            {

                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Clear();
            spegni:
                Console.Write("   Do you want to reboot or shutdown?(R/S)");
                var risp = Console.ReadLine();
                if (risp == "R" || risp == "r")
                {
                    Sys.Power.Reboot();
                }
                else if (risp == "S" || risp == "s")
                {
                    Stop();
                }
                else
                {
                    goto spegni;
                }

            }
        }
    }
}



