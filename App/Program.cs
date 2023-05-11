using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if NETCOREAPP
using System.Reflection;
using System.Runtime.Loader;
#endif

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
#if NETCOREAPP
            var alc = new MyBindingRedirectAssemblyLoadContext();
            var a = alc.LoadFromAssemblyPath(typeof(Program).Assembly.Location);
            var t = a.GetType(typeof(Program).FullName);
            var t_self = typeof(Program);
            t.GetMethod(nameof(DoMsgPackStuff), BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
#else
            DoMsgPackStuff();
#endif
        }

        private static void DoMsgPackStuff()
        {
            const string message = "hi";
            Console.WriteLine($"V2 serialized: {Convert.ToBase64String(MsgPackV2Consumer.Serialize(message))}");
            Console.WriteLine($"V1 serialized: {Convert.ToBase64String(MsgPackV1Consumer.Serialize(message))}");
        }
    }

#if NETCOREAPP
    internal class MyBindingRedirectAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly string binDirectory;

        internal MyBindingRedirectAssemblyLoadContext()
        {
            this.binDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string pathToAssembly = assemblyName switch
            {
                { Name: "MessagePack", Version: { Major: 1 } } => Path.Combine(this.binDirectory, "v1.9", "MessagePack.dll"),
                { Name: "MessagePack", Version: { Major: 2 } } => Path.Combine(this.binDirectory, "MessagePack.dll"),

                // We don't know where to find the assembly, but we have to load it or it reverts back to the default context.
                // So ask the default context to load it, then reload it with the same path.
                _ => Assembly.Load(assemblyName)?.Location,
            };

            return pathToAssembly != null ? this.LoadFromAssemblyPath(pathToAssembly) : null;
        }
    }
#endif
}
