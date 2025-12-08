using Blue_Sky.Classes;
using System;
using System.IO;
using System.Text;

namespace Blue_Sky.Readers
{
    internal static class O3DReader
    {
        public static void ReadAllO3DTextures(Map map)
        {
            foreach (O3D o3d in map.o3ds)
            {
                ReadO3D(map, o3d);
            }
        }
        private static void ReadO3D(Map map, O3D o3d)
        {
            byte[] o3dBytes;
            try { o3dBytes = File.ReadAllBytes($"{o3d.path}\\model\\{o3d.fileName}"); }
            catch { return; }
            ;

            // Check valid o3d
            if (o3dBytes[0] != 0x84 || o3dBytes[1] != 0x19) return;

            // Check o3d version
            uint cursor = 3;
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
                    SkipVerts(is4ByteCount, o3dBytes, ref cursor);
                }
                else if (o3dBytes[cursor] == 0x49)
                {
                    SkipFaces(is4ByteCount, is4ByteFace, o3dBytes, ref cursor);
                }
                else if (o3dBytes[cursor] == 0x26)
                {
                    ReadMatls(map, o3d, o3dBytes, ref cursor);
                    break;
                }
                else
                {
                    return;
                }
            }
        }

        public static void SkipVerts(bool is4ByteCount, byte[] o3dBytes, ref uint cursor)
        {
            // Get vertex count
            cursor++;
            uint vertCount = is4ByteCount ?
                BitConverter.ToUInt32(o3dBytes, (int)cursor) :
                BitConverter.ToUInt16(o3dBytes, (int)cursor);
            cursor += (uint) (is4ByteCount ? 4 : 2);

            // Move cursor by 8 * 4 bytes for each vertex
            cursor += vertCount * 32;
        }

        public static void SkipFaces(bool is4ByteCount, bool is4ByteFace, byte[] o3dBytes, ref uint cursor)
        {
            // Get face count
            cursor++;
            uint faceCount = is4ByteCount ?
                BitConverter.ToUInt32(o3dBytes, (int)cursor) :
                BitConverter.ToUInt16(o3dBytes, (int)cursor);
            cursor += (uint)(is4ByteCount ? 4 : 2);

            // Move cursor by 11 * 4 bytes for each face
            cursor += (uint)(faceCount * (is4ByteFace ? 14 : 8));
        }

        public static void ReadMatls(Map map, O3D o3d, byte[] o3dBytes, ref uint cursor)
        {
            // Get material count
            cursor++;
            int matlCount = BitConverter.ToInt16(o3dBytes, (int)cursor);
            cursor += 2;

            for (int i = 0; i < matlCount; i++)
            {
                //Move to byte for matl path length
                cursor += 11 * 4;
                uint matlPathLen = o3dBytes[cursor];
                cursor++;

                byte[] matlPathChars = new byte[matlPathLen];
                Array.Copy(o3dBytes, cursor, matlPathChars, 0, matlPathLen);
                cursor += matlPathLen;

                string fileName = Encoding.ASCII.GetString(matlPathChars);
                string matlPath = $"{o3d.path}\\texture";
                Texture newMatl = new Texture(fileName, matlPath, "o3d Material");

                if (!File.Exists($"{matlPath}\\{fileName}")) newMatl.isMissing = true;

                o3d.AddMaterial(newMatl);
                o3d.owner.AddTexture(newMatl);
                map.AddTexture(newMatl);
            }
        }
    }
}
