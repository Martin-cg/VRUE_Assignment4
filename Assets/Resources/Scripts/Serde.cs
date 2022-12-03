using ExitGames.Client.Photon;
using System.Runtime.InteropServices;
using System;

public static class Serde {
    public static int Serialize<T>(StreamBuffer outStream, T obj) where T : struct => Serialize(outStream, ref obj);

    public static int Serialize<T>(StreamBuffer outStream, ref T obj) where T: struct {
        var size = Marshal.SizeOf<T>();
        var buf = new Span<byte>(outStream.GetBufferAndAdvance(size, out int offset));
        buf = buf[offset..(offset + size)];
        MemoryMarshal.Write(buf, ref obj);
        return size;
    }

    public static T Deserialize<T>(StreamBuffer inStream) where T : struct {
        var size = Marshal.SizeOf<T>();
        var buf = new ReadOnlySpan<byte>(inStream.GetBufferAndAdvance(size, out int offset));
        buf = buf[offset..(offset + size)];
        return MemoryMarshal.Read<T>(buf);
    }
}
