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

                // ---------------------------------------------------------------------------------
                // Try to delete the assembly file when the AppDomain is finished with it.
                //   System.AppDomain.CurrentDomain.ProcessExit += OnExit
                //   System.AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;

                // Unable to delete file while the assembly is loaded into the domain. The System.AppDomain.ProcessExit event
                // is not guarenteed to be raised, and System.AppDomain.Unload is too early to delete the file.            
                // ---------------------------------------------------------------------------------
            }
        }

        /// Will attempt to load missing assembly from either x86 or x64 subdir
        internal static System.Reflection.Assembly Resolver(object sender, System.ResolveEventArgs args)
        {
            string assembly_name = new AssemblyName(args.Name).Name + ".dll";

            string resource_path = "AnyCPU.Resources." + Platform() + "." + assembly_name;
            string file_path = GetInteropDLLPath(assembly_name);
            using (var dll_resource = System.Reflection.Assembly.GetAssembly(typeof(ModuleInitializer)).GetManifestResourceStream(resource_path))
            {
                if(dll_resource == null)
                {
                    return null; 
                }

                if (System.IO.File.Exists(file_path))
                {
                    System.IO.File.Delete(file_path);
                }

                string directory = System.IO.Path.GetDirectoryName(file_path);
                if(!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                using (System.IO.FileStream file_stream = new FileStream(file_path, FileMode.Create))
                {
                    byte[] byte_array = new byte[(int)dll_resource.Length];
                    dll_resource.Read(byte_array, 0, (int)dll_resource.Length);

                    file_stream.Write(byte_array, 0, byte_array.Length);

                    // Since Managed C++ dlls also contained native code, the assembly must be loaded using a LoadFrom!
                    //return Assembly.Load(byte_array);
                }
            }

            Assembly assembly = Assembly.LoadFrom(file_path);

            // Unable to delete file while the assembly is loaded into the domain. 
            //File.Delete(file_path);

            return assembly;
        }

        internal static string GetInteropDLLPath(string resource_assembly)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(ModuleInitializer));

            string local_directory = System.IO.Path.GetDirectoryName(assembly.Location);
            if(IsWritable(local_directory))
            {
                return local_directory + Platform() + @"\" + resource_assembly;
            }

            string assembly_name = new AssemblyName(assembly.FullName).Name;

            return System.Environment.GetEnvironmentVariable("TEMP") + @"\\" + assembly_name + @"\" + Platform() + @"\" + resource_assembly;
        }

        internal static bool IsWritable(string directory)
        {
            try
            {
                string test_file = System.IO.Path.GetRandomFileName();
                using (System.IO.FileStream fs = System.IO.File.Create(directory + test_file))
                {
                    // do nothing
                }
                System.IO.File.Delete(test_file);
            }
            catch
            {
                return false;
            }
            return true;
        }

        internal static string Platform()
        {
            return (Environment.Is64BitProcess) ? "x64" : "x86";
        }
    }
}