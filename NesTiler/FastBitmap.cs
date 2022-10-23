﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.NesTiler
{
    public class FastBitmap
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Color[] colors;
        private static Dictionary<string, SKBitmap> imagesCache = new Dictionary<string, SKBitmap>();

        private FastBitmap(SKBitmap skBitmap, int verticalOffset = 0, int height = 0)
        {
            Width = skBitmap.Width;
            Height = height <= 0 ? skBitmap.Height - verticalOffset : height;
            var pixels = skBitmap.Pixels;
            colors = skBitmap.Pixels.Skip(verticalOffset * Width).Take(Width * Height).Select(p => Color.FromArgb(p.Alpha, p.Red, p.Green, p.Blue)).ToArray();
        }

        public static FastBitmap Decode(string filename, int verticalOffset = 0, int height = 0)
        {
            using var image = SKBitmap.Decode(filename);
            if (image == null) return null;
            imagesCache[filename] = image;
            return new FastBitmap(image, verticalOffset, height);
        }

        public Color GetPixelColor(int x, int y)
        {
            return colors[y * Width + x];
        }

        public void SetPixelColor(int x, int y, Color color)
        {
            colors[y * Width + x] = color;
        }

        public byte[] Encode(SKEncodedImageFormat format, int v)
        {
            using var skImage = new SKBitmap(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var color = colors[y * Width + x];
                    var skColor = new SKColor(color.R, color.G, color.B);
                    skImage.SetPixel(x, y, skColor);
                }
            }
            return skImage.Encode(format, v).ToArray();
        }
    }
}
