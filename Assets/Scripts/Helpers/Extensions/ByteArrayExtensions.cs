using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
// Original Authors - Wyatt Senalik (kinda, I borrowed most the code from
// https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp)

/// <summary>
/// Extensions to convert thins to byte arrays and back.
/// </summary>
public static class ByteArrayExtensions
{
    /// <summary>
    /// Converts a System.Object to a byte array.
    /// </summary>
    public static byte[] ToByteArray(this object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }
    /// <summary>
    /// Converts a byte[] to a System.Object.
    /// Inteded for use for unpacking a System.Object
    /// that was converted to a byte array using
    /// ByteArrayExtensions.ToByteArray.
    /// </summary>
    public static object ToObject(this byte[] byteArr)
    {
        using MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(byteArr, 0, byteArr.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        object obj = binForm.Deserialize(memStream);
        return obj;
    }
}
