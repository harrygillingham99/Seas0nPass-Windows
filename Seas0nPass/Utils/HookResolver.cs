﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Seas0nPass.Utils
{
    public class HookResolver
    {
        Dictionary<string, Assembly> _loaded;

        public HookResolver()
        {
            _loaded = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = "Seas0nPass.Resources." + args.Name.Split(',')[0] + ".dll";
            Assembly asm;
            lock (_loaded)
            {
                if (!_loaded.TryGetValue(name, out asm))
                {
                    using (Stream io = GetType().Assembly.GetManifestResourceStream(name))
                    {
                        byte[] bytes = new BinaryReader(io).ReadBytes((int)io.Length);
                        asm = Assembly.Load(bytes);
                        _loaded.Add(name, asm);
                    }
                }
            }
            return asm;
        }
    }
}
