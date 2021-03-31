////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////
using System;

namespace Seas0nPass.Models
{
    [Serializable]
    public class FirmwareVersion
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Md5 { get; set; }
        public string CommandsText { get; set; }
        public string OriginalFileName { get; set; }
        public string PatchedFileName { get; set; }
        public string Folder { get; set; }
        public string DownloadUrl { get; set; }
        public bool NeedTether { get; set; }
        public bool SaveIBec { get; set; }
    }
}
