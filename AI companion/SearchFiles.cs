using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AI_companion
{
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
    public class FindApps
    {
        List<string> diskNames = DiskInfo.GetDiskNames();
        private DirectoryInfo directoryInfo = null;
        static private HashSet<string> systemPaths = new HashSet<string>(
    Enum.GetValues(typeof(SystemDirectories))
    .Cast<SystemDirectories>()
    .Select(dir => Path.GetFullPath(
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
        private static void Search(DirectoryInfo dr, string name)
        {
            try
            {
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
        static public void FindAppByType(string type)
        {
            Regex file = new Regex($@".*{Regex.Escape(type)}$");
            foreach (string diskName in DiskInfo.GetDiskNames())
            {
                Console.WriteLine(diskName);
                DirectoryInfo dr = new DirectoryInfo($@"{diskName}");
                Search(dr, file);
            }
            
        }
        static public void FindAppByName(string name)
        {
            foreach (string diskName in DiskInfo.GetDiskNames())
            {
                Console.WriteLine(diskName);
                DirectoryInfo dr = new DirectoryInfo($@"{diskName}");
                Search(dr, name);
            }

        }

    }
    public class DiskInfo
    {
        static private DriveInfo[] GetDisksAvailable()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            return allDrives;
        }
        static public List<string> GetDiskNames()
        {
            List<string> diskNames = new List<string>();
            DriveInfo[] allDrives = GetDisksAvailable();
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady) diskNames.Add(drive.Name);
            }
            return diskNames;
        }
    }
}