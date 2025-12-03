using System;
using System.IO;
using System.Text;

namespace Blue_Sky
{
    internal static class O3DReader
    {
        public static O3D ReadO3D(string filepath)
        {
            byte[] o3dBytes;
            try
            {
                o3dBytes = File.ReadAllBytes(filepath);
            }
            catch
            {
                return null;
            }

            O3D o3d = new();

            // Check valid o3d
            if (o3dBytes[0] != 0x84 || o3dBytes[1] != 0x19)
            {
                return null;
            }

            // Check o3d version

            int cursor = 3;

            int version = o3dBytes[2];
            bool is4ByteCount = version > 3;
            bool is4ByteFace = false;

            // If OMSI2 format, check if faces use 4 byte vertex id
            if (is4ByteCount)
            {
                is4ByteFace = (o3dBytes[3] & 1) == 1;
                cursor += 5;
            }

            string[] materials = [];

            while (cursor < o3dBytes.Length)
            {
                if (o3dBytes[cursor] == 0x17)
                {
                    SkipVerts(is4ByteCount, ref o3d, o3dBytes, ref cursor);
                }
                else if (o3dBytes[cursor] == 0x49)
                {
                    SkipFaces(is4ByteCount, is4ByteFace, ref o3d, o3dBytes, ref cursor);
                }
                else if (o3dBytes[cursor] == 0x26)
                {
                    ReadMatls(ref o3d, o3dBytes, ref cursor);
                    break;
                }
                else
                {
                    return null;
                }
            }

            return o3d;
        }

        public static void SkipVerts(bool is4ByteCount, ref O3D o3d, byte[] o3dBytes, ref int cursor)
        {
            // Get vertex count
            cursor++;
            int vertCount = is4ByteCount ? 
                BitConverter.ToInt32(o3dBytes, cursor) : 
                BitConverter.ToInt16(o3dBytes, cursor);
            cursor += is4ByteCount ? 4 : 2;

            // Move cursor by 8 * 4 bytes for each vertex
            cursor += vertCount * 32;
        }

        public static void SkipFaces(bool is4ByteCount, bool is4ByteFace, ref O3D o3d, byte[] o3dBytes, ref int cursor)
        {
            // Get face count
            cursor++;
            int faceCount = is4ByteCount ?
                BitConverter.ToInt32(o3dBytes, cursor) :
                BitConverter.ToInt16(o3dBytes, cursor);
            cursor += is4ByteCount ? 4 : 2;

            // Move cursor by 11 * 4 bytes for each face
            cursor += faceCount * (is4ByteFace ? 14 : 8);
        }

        public static void ReadMatls(ref O3D o3d, byte[] o3dBytes, ref int cursor)
        {
            // Get material count
            cursor++;
            int matlCount = BitConverter.ToInt16(o3dBytes, cursor);
            cursor += 2;

            for (int i = 0; i < matlCount; i++)
            {
                //Move to byte for matl path length
                cursor += 11 * 4;
                int matlPathLen = o3dBytes[cursor];
                cursor++;

                byte[] matlPathChars = new byte[matlPathLen];
                Array.Copy(o3dBytes, cursor, matlPathChars, 0, matlPathLen);
                cursor += matlPathLen;

                string matlPath = Encoding.ASCII.GetString(matlPathChars);
                o3d.AddMaterial(matlPath);
            }
        }
    }
}
