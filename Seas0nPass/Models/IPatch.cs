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
    public interface IPatch
    {
        string PerformPatch();
        string CurrentMessage { get; }
        event EventHandler CurrentMessageChanged;
        int CurrentProgress { get; }
        event EventHandler CurrentProgressChanged;
    }
}
