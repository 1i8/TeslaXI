using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLeftExit.Growtopia.Decoding
{
    public static class ItemsDAT
    {
        private static readonly String key = "PBG892FXX982ABC*";

        private static String ReadItemString(this BinaryReader reader)
        {
            Int16 length = reader.ReadInt16();
            Byte[] buffer = new Byte[length];
            reader.BaseStream.Read(buffer);
            return Encoding.UTF8.GetString(buffer);
        }

        public static IEnumerable<ItemDefinition> Decode(String filename)
        {
            using (Stream stream = File.OpenRead(filename))
                return DecodeStream(stream);
        }

        private static IEnumerable<ItemDefinition> DecodeStream(Stream stream)
        {
            BinaryReader reader = new(stream);
            Int16 version = reader.ReadInt16();
            Int32 count = reader.ReadInt32();

            for(Int32 i = 0; i < count; i++)
            {
                ItemDefinition item = new();

                item.ItemID = reader.ReadInt32();
                item.EditableType = reader.ReadByte();
                item.ItemCategory = reader.ReadByte();
                item.ActionType = reader.ReadByte();
                item.HitSoundType = reader.ReadByte();

                Int16 length = reader.ReadInt16();
                Byte[] buffer = new Byte[length];
                for (Int32 j = 0; j < length; j++)
                    buffer[j] = (Byte)(reader.ReadByte() ^ (key[(j + item.ItemID) % key.Length]));
                item.Name = Encoding.UTF8.GetString(buffer);

                item.Texture = reader.ReadItemString();
                item.TextureHash = reader.ReadInt32();
                item.ItemKind = reader.ReadByte();
                item.val1 = reader.ReadInt32();
                item.TextureX = reader.ReadByte();
                item.TextureY = reader.ReadByte();
                item.SpreadType = reader.ReadByte();
                item.IsStripeyWallpaper = reader.ReadByte();
                item.CollisionType = reader.ReadByte();
                item.BreakHits = reader.ReadByte();
                item.DropChance = reader.ReadInt32();
                item.ClothingType = reader.ReadByte();
                item.Rarity = reader.ReadInt16();
                item.MaxAmount = reader.ReadByte();
                item.ExtraFile = reader.ReadItemString();
                item.ExtraFileHash = reader.ReadInt32();
                item.AudioVolume = reader.ReadInt32();
                item.PetName = reader.ReadItemString();
                item.PetPrefix = reader.ReadItemString();
                item.PetSuffix = reader.ReadItemString();
                item.PetAbility = reader.ReadItemString();
                item.SeedBase = reader.ReadByte();
                item.SeedOverlay = reader.ReadByte();
                item.TreeBase = reader.ReadByte();
                item.TreeLeaves = reader.ReadByte();
                item.SeedColor = reader.ReadInt32();
                item.SeedOverlayColor = reader.ReadInt32();
                stream.Seek(4, SeekOrigin.Current);
                item.GrowTime = reader.ReadInt32();
                item.val2 = reader.ReadInt16();
                item.IsRayman = reader.ReadInt16();
                item.ExtraOptions = reader.ReadItemString();
                item.Texture2 = reader.ReadItemString();
                item.ExtraOptions2 = reader.ReadItemString();
                stream.Seek(80, SeekOrigin.Current);
                item.PunchOptions = reader.ReadItemString();
                stream.Seek(13 + 4, SeekOrigin.Current);

                yield return item;
            }
        }
    }

    public class ItemDefinition
    {
        public Int32 ItemID { get; set; }
        public Byte EditableType { get; set; }
        public Byte ItemCategory { get; set; }
        public Byte ActionType { get; set; }
        public Byte HitSoundType { get; set; }
        public String Name { get; set; }
        public String Texture { get; set; }
        public Int32 TextureHash { get; set; }
        public Byte ItemKind { get; set; }
        public Int32 val1 { get; set; }
        public Byte TextureX { get; set; }
        public Byte TextureY { get; set; }
        public Byte SpreadType { get; set; }
        public Byte IsStripeyWallpaper { get; set; }
        public Byte CollisionType { get; set; }
        public Byte BreakHits { get; set; }
        public Int32 DropChance { get; set; }
        public Byte ClothingType { get; set; }
        public Int16 Rarity { get; set; }
        public Byte MaxAmount { get; set; }
        public String ExtraFile { get; set; }
        public Int32 ExtraFileHash { get; set; }
        public Int32 AudioVolume { get; set; }
        public String PetName { get; set; }
        public String PetPrefix { get; set; }
        public String PetSuffix { get; set; }
        public String PetAbility { get; set; }
        public Byte SeedBase { get; set; }
        public Byte SeedOverlay { get; set; }
        public Byte TreeBase { get; set; }
        public Byte TreeLeaves { get; set; }
        public Int32 SeedColor { get; set; }
        public Int32 SeedOverlayColor { get; set; }
        public Int32 GrowTime { get; set; }
        public Int16 val2 { get; set; }
        public Int16 IsRayman { get; set; }
        public String ExtraOptions { get; set; }
        public String Texture2 { get; set; }
        public String ExtraOptions2 { get; set; }
        public String PunchOptions { get; set; }
    }
}
