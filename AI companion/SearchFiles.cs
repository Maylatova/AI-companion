using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AI_companion;
namespace AI_companion
{
    // The enumeration of system`s directories
    public enum SystemDirectories
    {
        WindowsDirectory,        // C:\Windows
        ProgramFiles,            // C:\Program Files
        ProgramFilesx86,         // C:\Program Files (x86)
        System32,                // C:\Windows\System32
        TempDirectory,           // C:\Users\{user}\AppData\Local\Temp
        HomeDirectory,           // /home/{user}
        BinDirectory,            // /bin
        SbinDirectory,           // /sbin
        VarDirectory,            // /var
        EtcDirectory,            // /etc
        TmpDirectory             // /tmp
    }
    // Class that has methods to find files by their names or extensions
    public class FindFiles
    {
        List<string> diskNames = DiskInfo.GetDiskNames(); // all disks in the current device
        private DirectoryInfo directoryInfo = null; // the info about the certain direct

        static private HashSet<string> systemPaths = new HashSet<string>( // the hashset that checks if the direct is either system or not
    Enum.GetValues(typeof(SystemDirectories))
    .Cast<SystemDirectories>()
    .Select(dir => Path.GetFullPath( // The LINQ request that provides if the direct is either system or not
        dir switch
        {
            SystemDirectories.WindowsDirectory => @"C:\Windows",
            SystemDirectories.ProgramFiles => @"C:\Program Files",
            SystemDirectories.ProgramFilesx86 => @"C:\Program Files (x86)",
            SystemDirectories.System32 => @"C:\Windows\System32",
            SystemDirectories.TempDirectory => Path.GetTempPath(),
            SystemDirectories.HomeDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "home"),
            SystemDirectories.BinDirectory => "/bin",
            SystemDirectories.SbinDirectory => "/sbin",
            SystemDirectories.VarDirectory => "/var",
            SystemDirectories.EtcDirectory => "/etc",
            SystemDirectories.TmpDirectory => "/tmp",
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        }
    ))
);
        // Search by name
        private static void Search(DirectoryInfo dr, string name)
        {
            try
            {
                // Check all files in the current direct
                FileInfo[] fi = dr.GetFiles();
                foreach (FileInfo info in fi)
                {
                    if (string.Equals(Path.GetFileNameWithoutExtension(info.Name), name, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine(info.FullName); 
                    }
                }
            }
            catch (UnauthorizedAccessException) { return; }
            catch (DirectoryNotFoundException) { return; }

            try
            {
                DirectoryInfo[] dirs = dr.GetDirectories();
                // Check next direct if it is not system
                foreach (DirectoryInfo directoryInfo in dirs)
                {
                    if (!systemPaths.Contains(Path.GetFullPath(directoryInfo.FullName)))
                    {
                        Search(directoryInfo, name);
                    }
                }
            }
            catch (UnauthorizedAccessException) { return; }
        }
        private static void Search(DirectoryInfo dr, Regex file)
        {
            try
            {
                // Check all files in the current direct
                FileInfo[] fi = dr.GetFiles();
                foreach (FileInfo info in fi)
                {
                    if (file.IsMatch(info.Name))
                    {
                        Console.WriteLine(info.FullName);
                    }
                }
            }
            catch (UnauthorizedAccessException) { return; }
            catch (DirectoryNotFoundException) { return; }

            try
            {
                DirectoryInfo[] dirs = dr.GetDirectories();
                // Check next direct if it is not system
                foreach (DirectoryInfo directoryInfo in dirs)
                {
                    if (!systemPaths.Contains(Path.GetFullPath(directoryInfo.FullName)))
                    {
                        Search(directoryInfo, file);
                    }
                }
            }
            catch (UnauthorizedAccessException) { return; }
        }
        //public delegate void FindFile(string name);
        static public void FindFilesByType(string type)
        {
            Regex file = new Regex($@".*{Regex.Escape(type)}$"); // pattern that only provides an extension of a file
            // Check for all disks in the current device
            foreach (string diskName in DiskInfo.GetDiskNames())
            {
                Console.WriteLine(diskName);
                DirectoryInfo dr = new DirectoryInfo($@"{diskName}");
                Search(dr, file);
            }
            
        }
        static public void FindFilesByName(string name)
        {
            // Check for all disks in the current device
            foreach (string diskName in DiskInfo.GetDiskNames())
            {
                Console.WriteLine(diskName);
                DirectoryInfo dr = new DirectoryInfo($@"{diskName}");
                Search(dr, name);
            }

        }

    }
}