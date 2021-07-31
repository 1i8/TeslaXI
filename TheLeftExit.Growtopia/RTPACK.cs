using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheLeftExit.Growtopia
{
    public static class RTPACK
    {
        public static Bitmap Decode(String filename)
        {
            using(Stream stream = File.OpenRead(filename))
            {
                return DecodeStream(stream);
            }
        }

        private static Bitmap DecodeStream(Stream stream)
        {
            BinaryReader reader = new(stream);
            String rt = new(reader.ReadChars(6));
            switch (rt)
            {
                case "RTPACK":
                    using (Stream mstream = DecodeRTPACK(stream))
                    {
                        return DecodeStream(mstream);
                    }
                case "RTFONT":
                    throw new NotImplementedException("RTFONT decompression not implemented.");
                case "RTTXTR":
                    return DecodeRTTXTR(stream);
                default:
                    throw new NotImplementedException("What is this?");
            }
        }

        private static Stream DecodeRTPACK(Stream stream) // Must be disposed of within RTPACK.Decode()
        {
            BinaryReader reader = new(stream);
            stream.Seek(10, SeekOrigin.Current);
            bool zlib = reader.ReadBoolean();
            stream.Seek(17, SeekOrigin.Current);

            MemoryStream mstream = new();

            if (zlib)
                using (DeflateStream zstream = new(stream, CompressionMode.Decompress)) zstream.CopyTo(mstream);
            else
                stream.CopyTo(mstream);

            mstream.Position = 0;
            return mstream;
        }

        private static Bitmap DecodeRTTXTR(Stream stream)
        {
            using (BinaryReader reader = new(stream))
            {
                stream.Seek(2, SeekOrigin.Current);

                Int32 Height = reader.ReadInt32();
                Int32 Width = reader.ReadInt32();

                Int32 format = reader.ReadInt32();
                if (format != 0x1401)
                    throw new NotImplementedException("Unsupported RTTXTR format.");

                stream.Seek(8, SeekOrigin.Current);

                bool UsesAlpha = reader.ReadBoolean();

                stream.Seek(95, SeekOrigin.Current);

                Int32[] Bits = new Int32[Width * Height];
                GCHandle BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
                Bitmap buffer = new(Width, Height, Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());

                for (int y = Height - 1; y >= 0; y--)
                    for (int x = 0; x < Width; x++)
                        Bits[x + y * Width] = (reader.ReadByte()) << 16 | (reader.ReadByte() << 8) | (reader.ReadByte()) | (UsesAlpha ? reader.ReadByte() << 24 : -16777216);

                Bitmap res = buffer.Clone(new Rectangle(Point.Empty, buffer.Size), buffer.PixelFormat);

                buffer.Dispose();
                BitsHandle.Free();

                return res;
            }
        }
    }
}
