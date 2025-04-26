using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_companion
{
    // Class about disks` names
    public class DiskInfo
    {
        // To get all disks in the device
        static private DriveInfo[] GetDisksAvailable()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            return allDrives;
        }
        // To get all names of all disks in the device like "C:\"
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
