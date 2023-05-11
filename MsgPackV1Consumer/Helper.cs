using MessagePack;
using MessagePack.Resolvers;
using System;

namespace MsgPackV1;

public static class MsgPackV1Consumer
{
    public static byte[] Serialize(object value) => MessagePackSerializer.Serialize(value, ContractlessStandardResolver.Instance);
    public static object Deserialize(Type type, byte[] value) => MessagePackSerializer.NonGeneric.Deserialize(type, value, ContractlessStandardResolver.Instance);
}

