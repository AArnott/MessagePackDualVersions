using MessagePack;
using MessagePack.Resolvers;
using System;

public static class MsgPackV2Consumer
{
    public static byte[] Serialize(object value) => MessagePackSerializer.Serialize(value, ContractlessStandardResolver.Options);
    public static object Deserialize(Type type, byte[] value) => MessagePackSerializer.Deserialize(type, value, ContractlessStandardResolver.Options);
}
