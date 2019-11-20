using System;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
#if NETCOREAPP
            AssemblyLoadContext.Default.Resolving += AssemblyLoadContext_Resolving;
#endif

            const string message = "hi";
            Console.WriteLine($"V2 serialized: {Convert.ToBase64String(MsgPackV2Consumer.Serialize(message))}");
            Console.WriteLine($"V1 serialized: {Convert.ToBase64String(MsgPackV1Consumer.Serialize(message))}");
        }

#if NETCOREAPP
        private static System.Reflection.Assembly AssemblyLoadContext_Resolving(AssemblyLoadContext arg1, System.Reflection.AssemblyName arg2)
        {
            return null;
        }
#endif
    }
}
