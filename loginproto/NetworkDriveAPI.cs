using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace loginproto
{
    public class NetworkDriveAPI
    {
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2A(ref NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2A(string name, int flags, bool force);

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public int Scope;
            public int Type;
            public int DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        // Map a network drive
        public static int MapNetworkDrive(string driveLetter, string networkPath, string username, string password)
        {
            NetResource netResource = new NetResource
            {
                Scope = 2,
                Type = 1,
                DisplayType = 0,
                Usage = 0,
                LocalName = driveLetter + ":",
                RemoteName = networkPath,
                Provider = null
            };

            int result = WNetAddConnection2A(ref netResource, password, username, 0);

            return result;
        }

        // Unmap a network drive
        public static int UnmapNetworkDrive(string driveLetter)
        {
            int result = WNetCancelConnection2A(driveLetter + ":", 0, true);
            return result;
        }

        // Get the path to My Computer
        public static string GetMyComputerPath()
        {
            // The path to My Computer
            return "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
        }

        // Create a new drive
        public static string CreateNewDrive()
        {
            // Logic to create a new drive goes here
            // For demonstration purposes, let's assume we create a new folder in the root directory of C: drive
            try
            {
                string newDriveLetter = "R"; // For example, you can use any available drive letter

                // Path for the new drive (assuming C: for demonstration)
                string newDrivePath = System.IO.Path.Combine("C:\\", "NewDrive");

                // Create the directory
                System.IO.Directory.CreateDirectory(newDrivePath);

                // Optionally, you might want to use ManagementObject to map this directory as a new drive
                MapNetworkDrive(newDriveLetter, newDrivePath, null, null);

                return newDriveLetter;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating new drive: {ex.Message}");
                return null;
            }
        }
    }
}
