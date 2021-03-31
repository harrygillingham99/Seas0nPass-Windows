////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////
using System;

namespace Seas0nPass.CustomEventArgs
{
    public class CreateIpswClickedEventArgs : EventArgs
    {
        public string FileName { get; private set; }

        public CreateIpswClickedEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}
