using System.IO;
using System.Text;
using ProtoBuf;

namespace GameProtos
{
    public static class ProtoTool
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T t)
        {
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, t);
            return ms.ToArray();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(byte[] content)
        {
            using var ms = new MemoryStream(content);
            var t = Serializer.Deserialize<T>(ms);
            return t;
        }
    }
}