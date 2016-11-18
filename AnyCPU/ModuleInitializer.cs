// This file can be modified in any way, with two exceptions. 1) The name of
// this class must be "ModuleInitializer". 2) This class must have a public or
// internal parameterless "Run" method that returns void. In addition to these
// modifications, this file may also be moved to any location, as long as it
// remains a part of its current project.
using System;
using System.IO;
using System.Reflection;

namespace AnyCPU
{
    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            lock (typeof(ModuleInitializer))
            {
                System.AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            }
        }

        /// Will attempt to load missing assembly from either x86 or x64 subdir
        internal static System.Reflection.Assembly Resolver(object sender, System.ResolveEventArgs args)
        {
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";

            string platform = "x86";
            if (Environment.Is64BitProcess)
            {
                platform = "x64";
            }

            string resource_path = "AnyCPU.Resources." + platform + "." + assemblyName;
            /*
                * Cannot make this work!!!!
            Assembly current_assembly = typeof(CppStackLoader).Assembly;

            byte[] byte_array = null;
            using (Stream assembly = current_assembly.GetManifestResourceStream(resource_path))
            {
                byte_array = new byte[(int)assembly.Length];
                assembly.Read(byte_array, 0, (int)assembly.Length);

                return Assembly.Load(byte_array);
            }
            */

            string file_path = System.Environment.GetEnvironmentVariable("TEMP") + "\\" + assemblyName;
            ;
            if (System.IO.File.Exists(file_path))
            {
                System.IO.File.Delete(file_path);
            }

            using (var dll_resource = System.Reflection.Assembly.GetAssembly(typeof(ModuleInitializer)).GetManifestResourceStream(resource_path))
            {
                if(dll_resource == null)
                {
                    return null; 
                }

                using (System.IO.FileStream file_stream = new FileStream(file_path, FileMode.Create))
                {
                    byte[] byte_array = new byte[(int)dll_resource.Length];
                    dll_resource.Read(byte_array, 0, (int)dll_resource.Length);

                    file_stream.Write(byte_array, 0, byte_array.Length);
                    return Assembly.Load(byte_array);
                }
            }

            Assembly assembly = Assembly.LoadFile(file_path);
            //File.Delete(file_path);
            return assembly;
        }
    }
}