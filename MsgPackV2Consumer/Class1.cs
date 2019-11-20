using MessagePack;
using System;

public static class MsgPackV2Consumer
{
    public static byte[] Serialize(string value) => MessagePackSerializer.Serialize(value);
}
