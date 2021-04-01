﻿////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

using System;
using System.IO;
using System.Linq;
using Seas0nPass.Interfaces;

namespace Seas0nPass.Models
{
    public class FreeSpaceModel : IFreeSpaceModel
    {
        private static readonly long RequiredSpace = (long)(1024 * 1024 * 1024 * 3.5); // 3.5GB in bytes

        public bool IsEnoughFreeSpace()
        {
            string systemDriveName = Environment.GetEnvironmentVariable("SystemDrive") + "\\";
            DriveInfo systemDriveInfo = DriveInfo.GetDrives().First(x => x.Name == systemDriveName);
            return systemDriveInfo.AvailableFreeSpace > RequiredSpace;
        }
    }
}
