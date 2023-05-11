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
            Poco message = new() { Message = "hi" };

            var v2data = MsgPackV2Consumer.Serialize(message);
            Console.WriteLine($"V2 serialized: {Convert.ToBase64String(v2data)}");
            var v2message = MsgPackV2Consumer.Deserialize(typeof(Poco), v2data);

            var v1data = MsgPackV1Consumer.Serialize(message);
            Console.WriteLine($"V1 serialized: {Convert.ToBase64String(v1data)}");
            var v1message = MsgPackV1Consumer.Deserialize(typeof(Poco), v1data);
        }
    }

    public class Poco
    {
        public string Message { get; set; }
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
        private readonly MethodInfo _deserializeMethod;

        public MsgPackV1Consumer()
        {
            var assemblyLoadContext = new MyBindingRedirectAssemblyLoadContext();
            var assembly = assemblyLoadContext.LoadFromAssemblyPath(typeof(MsgPackV1.MsgPackV1Consumer).Assembly.Location);
            _sideLoadedMsgPackV1Consumer = assembly.GetType(typeof(MsgPackV1.MsgPackV1Consumer).FullName);
            _serializeMethod = _sideLoadedMsgPackV1Consumer.GetMethod(nameof(Serialize), new Type[] { typeof(string) });
            _deserializeMethod = _sideLoadedMsgPackV1Consumer.GetMethod(nameof(Deserialize), new Type[] { typeof(Type), typeof(byte[]) });
        }

        public static byte[] Serialize(object value) => Instance._serializeMethod.Invoke(null, new object[] { value }) as byte[];
        public static object Deserialize(Type type, byte[] value) => Instance._deserializeMethod.Invoke(null, new object[] { type, value });
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
