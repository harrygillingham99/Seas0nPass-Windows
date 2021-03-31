////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

namespace Seas0nPass.Interfaces
{
    public class TunesInfo
    {
        public string RequiredVersion { get; set; }
        public string InstalledVersion { get; set; }
        public bool IsCompatible { get; set; }
    }
}
