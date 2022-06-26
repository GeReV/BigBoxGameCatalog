using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace Catalog.Wpf
{
    internal sealed class AtlasSprite : IDisposable
    {
        public SKSizeI AtlasSize { get; }
        public SKPointI AtlasPosition { get; set; }
        public SKRectI AtlasBounds => SKRectI.Create(AtlasPosition, AtlasSize);
        public SKImageInfo ImageInfo => Image.Info;
        public SKImage Image { get; }

        public string ImageKey { get; }

        public AtlasSprite(string path, int targetWidth)
        {
            ImageKey = path;
            Image = SKImage.FromBitmap(SKBitmap.Decode(path));

            var ratio = targetWidth / (float)ImageInfo.Width;
            AtlasSize = new SKSizeI(targetWidth, (int)(ImageInfo.Height * ratio));
        }

        #region IDisposable Members

        public void Dispose()
        {
            Image.Dispose();
        }

        #endregion
    }

    internal sealed class AtlasBin : IEnumerable<AtlasSprite>, IDisposable
    {
        private readonly List<AtlasSprite> sprites = new();
        public SKRectI Bounds { get; }
        public int TotalHeight { get; private set; }

        public AtlasBin(SKRectI bounds)
        {
            Bounds = bounds;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (var sprite in this)
            {
                sprite.Dispose();
            }
        }

        #endregion

        #region IEnumerable<AtlasSprite> Members

        public IEnumerator<AtlasSprite> GetEnumerator()
        {
            return sprites.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void PushSprite(AtlasSprite sprite)
        {
            sprite.AtlasPosition = new SKPointI(Bounds.Left, TotalHeight);

            TotalHeight += sprite.AtlasSize.Height;

            sprites.Add(sprite);
        }
    }

    public sealed class SkiaTextureAtlas : IDisposable
    {
        private readonly int atlasSize;

        private readonly Dictionary<string, SKRectI> atlasSprites = new();
        private readonly int spriteWidth;
        private SKImage? atlas;

        private int currentAtlasHash;

        public SkiaTextureAtlas(int spriteWidth = 256, int atlasSize = 8192)
        {
            // Contract.Requires<ArgumentException>(
            //     spriteWidth > 0,
            //     $"Expected {nameof(spriteWidth)} to be greater than 0"
            // );
            // Contract.Requires<ArgumentException>(
            //     IsPowerOf2(spriteWidth),
            //     $"Expected {nameof(spriteWidth)} to be a power of 2"
            // );
            //
            // Contract.Requires<ArgumentException>(atlasSize > 0, $"Expected {nameof(atlasSize)} to be greater than 0");
            // Contract.Requires<ArgumentException>(
            //     IsPowerOf2(atlasSize),
            //     $"Expected {nameof(atlasSize)} to be a power of 2"
            // );

            this.spriteWidth = spriteWidth;
            this.atlasSize = atlasSize;
        }

        #region IDisposable Members

        public void Dispose()
        {
            atlas?.Dispose();
        }

        #endregion

        private static bool IsPowerOf2(int n)
        {
            return (n & (n - 1)) == 0;
        }

        private static int HashImages(List<string> images)
        {
            var hash = new HashCode();

            images.Sort();

            foreach (var image in images)
            {
                hash.Add(image);
            }

            return hash.ToHashCode();
        }

        public void BuildAtlas(GRContext grContext, ICollection<string> images)
        {
            if (images.Count == 0)
            {
                return;
            }

            var newHash = HashImages(images.ToList());

            // Same atlas requested, no need to rebuild.
            if (currentAtlasHash == newHash)
            {
                return;
            }

            currentAtlasHash = newHash;

            atlas?.Dispose();

            var surface = SKSurface.Create(
                grContext,
                true,
                new SKImageInfo(atlasSize, atlasSize, SKColorType.Rgba8888)
            );

            var sprites = images
                .Select(path => new AtlasSprite(path, spriteWidth))
                .OrderByDescending(info => info.ImageInfo.Height)
                .ToList();

            var bins = Enumerable.Range(0, atlasSize / spriteWidth)
                .Select(i => new AtlasBin(SKRectI.Create(i * spriteWidth, 0, spriteWidth, atlasSize)))
                .ToArray();

            foreach (var sprite in sprites)
            {
                var targetBin =
                    bins.FirstOrDefault(bin => bin.TotalHeight + sprite.AtlasSize.Height <= atlasSize);

                if (targetBin is null)
                {
                    // TODO: Add another atlas.
                    throw new Exception("Could not find bin");
                }

                targetBin.PushSprite(sprite);
            }

            surface.Canvas.Clear(SKColors.White);

            foreach (var bin in bins)
            {
                foreach (var sprite in bin)
                {
                    atlasSprites.TryAdd(sprite.ImageKey, sprite.AtlasBounds);

                    surface.Canvas.DrawImage(sprite.Image, sprite.AtlasBounds);
                }
            }

            atlas = surface.Snapshot();

            surface.Dispose();

            foreach (var bin in bins)
            {
                bin.Dispose();
            }
        }

        public void DrawSprites(SKCanvas canvas, IEnumerable<string> images, SKRect[] rects)
        {
            // Contract.Requires<ArgumentOutOfRangeException>(images.Length == rects.Length);

            var sprites = images.Select(image => (SKRect)atlasSprites[image]).ToArray();

            for (var i = 0; i < sprites.Length; i++)
            {
                var dest = rects[i].AspectFit(sprites[i].Size);

                canvas.DrawImage(atlas, sprites[i], dest);
            }
        }

        public void DrawSprite(SKCanvas canvas, string image, SKRect rect)
        {
            var sprite = (SKRect)atlasSprites[image];

            var dest = rect.AspectFit(sprite.Size);

            canvas.DrawImage(atlas, sprite, dest);
        }
    }
}
