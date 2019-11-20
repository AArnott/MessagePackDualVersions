using MessagePack;
using System;

public static class MsgPackV1Consumer
{
    public static byte[] Serialize(string value) => MessagePackSerializer.Serialize(value);
}

