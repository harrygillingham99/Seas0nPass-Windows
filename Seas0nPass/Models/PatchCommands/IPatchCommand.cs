////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

using System.Collections.Generic;

namespace Seas0nPass.Models.PatchCommands
{
    public interface IPatchCommand
    {
        string Name { get; }
        ICommandResult Execute(IDictionary<string, string> vars, params string[] args); 
    }
}
