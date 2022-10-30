using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Extensions for <see cref="NetworkWriter"/> and
    /// <see cref="NetworkReader"/> for sending TransformData over the network.
    /// </summary>
    public static class TransformDataReaderWriter
    {
        public static void WriteTransformData(this NetworkWriter writer,
            TransformData transData)
        {
            // Position
            writer.Write(transData.position.x);   // float
            writer.Write(transData.position.y);   // float
            writer.Write(transData.position.z);   // float
            // Rotation
            writer.Write(transData.rotation.x);   // float
            writer.Write(transData.rotation.y);   // float
            writer.Write(transData.rotation.z);   // float
            writer.Write(transData.rotation.w);   // float
            // Scale
            writer.Write(transData.scale.x);      // float
            writer.Write(transData.scale.y);      // float
            writer.Write(transData.scale.z);      // float
        }
        public static TransformData ReadTransformData(this NetworkReader reader)
        {
            // Position
            Vector3 pos = new Vector3();
            pos.x = reader.Read<float>();
            pos.y = reader.Read<float>();
            pos.z = reader.Read<float>();
            // Rotation
            float rot_x = reader.Read<float>();
            float rot_y = reader.Read<float>();
            float rot_z = reader.Read<float>();
            float rot_w = reader.Read<float>();
            Quaternion rot = new Quaternion(rot_x, rot_y, rot_z, rot_w);
            // Scale
            Vector3 scale = new Vector3();
            scale.x = reader.Read<float>();
            scale.y = reader.Read<float>();
            scale.z = reader.Read<float>();

            return new TransformData(pos, rot, scale);
        }
    }
}
