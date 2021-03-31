////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////
using System;
using System.Linq;
using System.Reflection;

namespace Seas0nPass
{
    public static class IoC
    {
        public static T Resolve<T>()
        {
            var contcreteType = (from type in Assembly.GetEntryAssembly().GetTypes()
                        where type.GetInterface(typeof(T).FullName) != null
                        select type).First();

            return (T)Activator.CreateInstance(contcreteType);
        }
    }
}
