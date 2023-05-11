using MsgPackV1;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if NET7_0
using System.Reflection;
using System.Runtime.Loader;
#endif

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            DoMsgPackStuff();
        }

        private static void DoMsgPackStuff()
        {
            const string message = "hi";
            Console.WriteLine($"V2 serialized: {Convert.ToBase64String(MsgPackV2Consumer.Serialize(message))}");
            Console.WriteLine($"V1 serialized: {Convert.ToBase64String(MsgPackV1Consumer.Serialize(message))}");
        }
    }

 #if NET7_0

    internal class MsgPackV1Consumer
    {
        private static MsgPackV1Consumer _instance;
        public static MsgPackV1Consumer Instance
        {
            get
            {
                _instance ??= new MsgPackV1Consumer();
                return _instance;
            }
        }

        private readonly Type _sideLoadedMsgPackV1Consumer;
        private readonly MethodInfo _serializeMethod;

        public MsgPackV1Consumer()
        {
            var assemblyLoadContext = new MyBindingRedirectAssemblyLoadContext();
            var assembly = assemblyLoadContext.LoadFromAssemblyPath(typeof(MsgPackV1.MsgPackV1Consumer).Assembly.Location);
            _sideLoadedMsgPackV1Consumer = assembly.GetType(typeof(MsgPackV1.MsgPackV1Consumer).FullName);
            _serializeMethod = _sideLoadedMsgPackV1Consumer.GetMethod(nameof(Serialize), new Type[] { typeof(string) });
        }

        public static byte[] Serialize(string value) => Instance._serializeMethod.Invoke(null, new object[] { value }) as byte[];
    }

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
