using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using SkiaSharp;

namespace Catalog.Wpf
{
    sealed class AtlasSprite : IDisposable
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

        public void Dispose()
        {
            Image.Dispose();
        }
    }

    sealed class AtlasBin : IEnumerable<AtlasSprite>, IDisposable
    {
        public SKRectI Bounds { get; }
        public int TotalHeight { get; private set; }

        private readonly List<AtlasSprite> sprites = new();

        public AtlasBin(SKRectI bounds)
        {
            Bounds = bounds;
        }

        public void PushSprite(AtlasSprite sprite)
        {
            sprite.AtlasPosition = new SKPointI(Bounds.Left, TotalHeight);

            TotalHeight += sprite.AtlasSize.Height;

            sprites.Add(sprite);
        }

        public IEnumerator<AtlasSprite> GetEnumerator() => sprites.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            foreach (var sprite in this)
            {
                sprite.Dispose();
            }
        }
    }

    public sealed class SkiaTextureAtlas : IDisposable
    {
        private readonly int spriteWidth;
        private readonly int atlasSize;

        private Dictionary<string, SKRectI> atlasSprites = new();
        private SKBitmap atlas;
        private static bool IsPowerOf2(int n) => (n & (n - 1)) == 0;


        public SkiaTextureAtlas(int spriteWidth = 256, int atlasSize = 8192)
        {
            Contract.Requires<ArgumentException>(
                spriteWidth > 0,
                $"Expected {nameof(spriteWidth)} to be greater than 0"
            );
            Contract.Requires<ArgumentException>(
                IsPowerOf2(spriteWidth),
                $"Expected {nameof(spriteWidth)} to be a power of 2"
            );

            Contract.Requires<ArgumentException>(atlasSize > 0, $"Expected {nameof(atlasSize)} to be greater than 0");
            Contract.Requires<ArgumentException>(
                IsPowerOf2(atlasSize),
                $"Expected {nameof(atlasSize)} to be a power of 2"
            );

            this.spriteWidth = spriteWidth;
            this.atlasSize = atlasSize;

            atlas = new SKBitmap(atlasSize, atlasSize, SKColorType.Rgb888x, SKAlphaType.Opaque);
        }

        public void BuildAtlas(IEnumerable<string> images)
        {
            var sprites = images.AsParallel()
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

            using var canvas = new SKCanvas(atlas);

            using var paint = new SKPaint
            {
                // FilterQuality = SKFilterQuality.High,
            };

            canvas.Clear(SKColors.White);

            foreach (var bin in bins)
            {
                foreach (var sprite in bin)
                {
                    if (atlasSprites.ContainsKey(sprite.ImageKey))
                    {
                        continue;
                    }

                    atlasSprites.Add(sprite.ImageKey, sprite.AtlasBounds);

                    canvas.DrawImage(sprite.Image, sprite.AtlasBounds, paint);
                }
            }

            foreach (var bin in bins)
            {
                bin.Dispose();
            }
        }

        public void DrawSprites(SKCanvas canvas, string[] images, SKRect[] rects)
        {
            // Contract.Requires<ArgumentOutOfRangeException>(images.Length == rects.Length);

            var sprites = images.Select(image => (SKRect)atlasSprites[image]).ToArray();

            var transforms = rects.Zip(sprites)
                .Select(
                    pair =>
                    {
                        var (dest, sprite) = pair;

                        return SKRotationScaleMatrix.Create(
                            dest.Width / sprite.Width,
                            0,
                            dest.Left,
                            dest.Top,
                            0,
                            0
                        );
                    }
                )
                .ToArray();

            using var paint = new SKPaint
            {
                // FilterQuality = SKFilterQuality.High,
            };

            canvas.DrawAtlas(SKImage.FromBitmap(atlas), sprites, transforms, paint);
        }

        public void Dispose()
        {
            atlas.Dispose();
        }
    }
}
