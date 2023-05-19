using MessagePack;
using MessagePack.Resolvers;

namespace MsgPackV2;

public static class MsgPackV2Consumer
{
    public static byte[] Serialize<T>(T value) => MessagePackSerializer.Serialize(value, StandardResolver.Options);

    public static T Deserialize<T>(byte[] value) => MessagePackSerializer.Deserialize<T>(value, StandardResolver.Options);
}
