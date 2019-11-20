using System;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            const string message = "hi";
            Console.WriteLine($"V2 serialized: {Convert.ToBase64String(MsgPackV2Consumer.Serialize(message))}");
            Console.WriteLine($"V1 serialized: {Convert.ToBase64String(MsgPackV1Consumer.Serialize(message))}");
        }
    }
}
