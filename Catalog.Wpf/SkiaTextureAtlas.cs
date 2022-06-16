using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Catalog.Wpf.GlContexts;
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

        private readonly Dictionary<string, SKRectI> atlasSprites = new();
        private SKImage? atlas;

        // private static bool IsPowerOf2(int n) => (n & (n - 1)) == 0;

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

        public void BuildAtlas(GlContext glContext, GRContext grContext, IEnumerable<string> images)
        {
            var texture = glContext.CreateTexture(new SKSizeI(atlasSize, atlasSize));

            var surface = SKSurface.CreateAsRenderTarget(
                grContext,
                new GRBackendTexture(atlasSize, atlasSize, true, texture),
                SKColorType.Rgba8888
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
                    if (atlasSprites.ContainsKey(sprite.ImageKey))
                    {
                        continue;
                    }

                    atlasSprites.Add(sprite.ImageKey, sprite.AtlasBounds);

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

        public void Dispose()
        {
            atlas?.Dispose();
        }
    }
}
