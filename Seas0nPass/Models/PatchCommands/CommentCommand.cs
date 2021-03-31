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
    public class CommentCommand : PatchCommand
    {
        public CommentCommand()
            : base("--")
        {
        }

        public override ICommandResult Execute(IDictionary<string, string> vars, params string[] args)
        {
            return Success();
        }
    }
}
