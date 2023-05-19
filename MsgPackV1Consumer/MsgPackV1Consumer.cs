using MessagePack;
using MessagePack.Resolvers;

namespace MsgPackV1;

public static class MsgPackV1Consumer
{
    public static byte[] Serialize<T>(T value) => MessagePackSerializer.Serialize(value, StandardResolver.Instance);

    public static T Deserialize<T>(byte[] value) => MessagePackSerializer.Deserialize<T>(value, StandardResolver.Instance);
}

