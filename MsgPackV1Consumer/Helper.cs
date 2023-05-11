using MessagePack;
using System;

namespace MsgPackV1;

public static class MsgPackV1Consumer
{
    public static byte[] Serialize(string value) => MessagePackSerializer.Serialize(value);
}

