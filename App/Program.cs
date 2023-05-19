using System;
using System.Diagnostics;
using System.IO;
#if NETCOREAPP
using System.Reflection;
using System.Runtime.Loader;
#endif
using MsgPackV1;
using MsgPackV2;

namespace App;

class Program
{
    static void Main(string[] args)
    {
#if NETCOREAPP
        AssemblyLoadContext v1Context = new("MessagePack v1");
        AssemblyLoadContext v2Context = new("MessagePack v2");
        AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).Resolving += (s, e) =>
        {
            if (e.Name.StartsWith("MessagePack", StringComparison.OrdinalIgnoreCase) && e.Version is not null)
            {
                string assemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"v{e.Version.Major}", $"{e.Name}.dll");
                AssemblyLoadContext alc = e.Version.Major switch
                {
                    1 => v1Context,
                    2 => v2Context,
                    _ => throw new InvalidOperationException(),
                };
                return alc.LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        };
#endif
        DoMsgPackStuff();
    }

    private static void DoMsgPackStuff()
    {
        DataObject message = new() { Message = "hi" };

        var v2data = MsgPackV2Consumer.Serialize(message);
        Console.WriteLine($"V2 serialized: {Convert.ToBase64String(v2data)}");
        DataObject v2message = MsgPackV2Consumer.Deserialize<DataObject>(v2data);

        var v1data = MsgPackV1Consumer.Serialize(message);
        Console.WriteLine($"V1 serialized: {Convert.ToBase64String(v1data)}");
        DataObject v1message = MsgPackV1Consumer.Deserialize<DataObject>(v1data);
    }
}
