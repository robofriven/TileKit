using System;
using System.IO;
using System.Text;
using TileKit.Core.Map;

namespace TileKit.Core.Serialization
{
    /// <summary>Very basic binary serialization for TileLayer/TileMap snapshots.</summary>
    public static class TileSerde
    {
        public static byte[] SaveLayer(TileLayer layer)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms, Encoding.UTF8, true);
            bw.Write(layer.Width);
            bw.Write(layer.Height);
            for (int y = 0; y < layer.Height; y++)
                for (int x = 0; x < layer.Width; x++)
                    bw.Write(layer.Get(new Grid.TilePos(x, y)));
            bw.Flush();
            return ms.ToArray();
        }

        public static TileLayer LoadLayer(ReadOnlySpan<byte> data)
        {
            using var ms = new MemoryStream(data.ToArray());
            using var br = new BinaryReader(ms, Encoding.UTF8, true);
            int w = br.ReadInt32();
            int h = br.ReadInt32();
            var layer = new TileLayer(w, h);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    layer.Set(new Grid.TilePos(x, y), br.ReadByte());
            return layer;
        }
    }
}