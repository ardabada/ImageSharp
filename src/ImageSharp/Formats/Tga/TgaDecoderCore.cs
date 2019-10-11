// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Memory;

namespace SixLabors.ImageSharp.Formats.Tga
{
    internal sealed class TgaDecoderCore
    {
        /// <summary>
        /// The metadata.
        /// </summary>
        private ImageMetadata metadata;

        /// <summary>
        /// The tga specific metadata.
        /// </summary>
        private TgaMetadata tgaMetadata;

        /// <summary>
        /// The file header containing general information about the image.
        /// </summary>
        private TgaFileHeader fileHeader;

        /// <summary>
        /// The global configuration.
        /// </summary>
        private readonly Configuration configuration;

        /// <summary>
        /// Used for allocating memory during processing operations.
        /// </summary>
        private readonly MemoryAllocator memoryAllocator;

        /// <summary>
        /// The stream to decode from.
        /// </summary>
        private Stream currentStream;

        /// <summary>
        /// The bitmap decoder options.
        /// </summary>
        private readonly ITgaDecoderOptions options;

        public TgaDecoderCore(Configuration configuration, ITgaDecoderOptions options)
        {
            this.configuration = configuration;
            this.memoryAllocator = configuration.MemoryAllocator;
            this.options = options;
        }

        /// <summary>
        /// Decodes the image from the specified stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="stream">The stream, where the image should be decoded from. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">
        ///    <para><paramref name="stream"/> is null.</para>
        /// </exception>
        /// <returns>The decoded image.</returns>
        public Image<TPixel> Decode<TPixel>(Stream stream)
            where TPixel : struct, IPixel<TPixel>
        {
            try
            {
                this.ReadFileHeader(stream);
                this.currentStream.Skip(this.fileHeader.IdLength);

                // Parse the color map, if present.
                if (this.fileHeader.ColorMapType != 0 && this.fileHeader.ColorMapType != 1)
                {
                    TgaThrowHelper.ThrowNotSupportedException($"Unknown tga colormap type {this.fileHeader.ColorMapType} found");
                }

                var image = new Image<TPixel>(this.configuration, this.fileHeader.Width, this.fileHeader.Height, this.metadata);
                Buffer2D<TPixel> pixels = image.GetRootFramePixelBuffer();

                byte[] palette = null;
                int colorMapPixelSizeInBytes = 0;
                if (this.fileHeader.ColorMapType is 1)
                {
                    if (this.fileHeader.CMapLength <= 0)
                    {
                        TgaThrowHelper.ThrowImageFormatException("Missing tga color map length");
                    }

                    if (this.fileHeader.CMapDepth <= 0)
                    {
                        TgaThrowHelper.ThrowImageFormatException("Missing tga color map depth");
                    }

                    colorMapPixelSizeInBytes = this.fileHeader.CMapDepth / 8;
                    palette = new byte[this.fileHeader.CMapLength * colorMapPixelSizeInBytes];
                    this.currentStream.Read(palette, this.fileHeader.CMapStart, palette.Length);

                    if (this.fileHeader.ImageType is TgaImageType.RleColorMapped)
                    {
                        this.ReadPalettedRle(this.fileHeader.Width, this.fileHeader.Height, pixels, palette, colorMapPixelSizeInBytes);
                    }
                    else
                    {
                        this.ReadPaletted(this.fileHeader.Width, this.fileHeader.Height, pixels, palette, colorMapPixelSizeInBytes);
                    }

                    return image;
                }

                switch (this.fileHeader.PixelDepth)
                {
                    case 8:
                        if (this.fileHeader.ImageType.IsRunLengthEncoded())
                        {
                            this.ReadRle(this.fileHeader.Width, this.fileHeader.Height, pixels, 1);
                        }
                        else
                        {
                            this.ReadMonoChrome(this.fileHeader.Width, this.fileHeader.Height, pixels);
                        }

                        break;

                    case 15:
                    case 16:
                        if (this.fileHeader.ImageType.IsRunLengthEncoded())
                        {
                            long currentPosition = this.currentStream.Position;
                            this.ReadRle(this.fileHeader.Width, this.fileHeader.Height, pixels, 2);
                        }
                        else
                        {
                            this.ReadBgra16(this.fileHeader.Width, this.fileHeader.Height, pixels);
                        }

                        break;

                    case 24:
                        if (this.fileHeader.ImageType.IsRunLengthEncoded())
                        {
                            this.ReadRle(this.fileHeader.Width, this.fileHeader.Height, pixels, 3);
                        }
                        else
                        {
                            this.ReadBgr24(this.fileHeader.Width, this.fileHeader.Height, pixels);
                        }

                        break;

                    case 32:
                        if (this.fileHeader.ImageType.IsRunLengthEncoded())
                        {
                            this.ReadRle(this.fileHeader.Width, this.fileHeader.Height, pixels, 4);
                        }
                        else
                        {
                            this.ReadBgra32(this.fileHeader.Width, this.fileHeader.Height, pixels);
                        }

                        break;

                    default:
                        TgaThrowHelper.ThrowNotSupportedException("Does not support this kind of tga files.");
                        break;
                }

                return image;
            }
            catch (IndexOutOfRangeException e)
            {
                throw new ImageFormatException("TGA image does not have a valid format.", e);
            }
        }

        private void ReadPaletted<TPixel>(int width, int height, Buffer2D<TPixel> pixels, byte[] palette, int colorMapPixelSizeInBytes)
            where TPixel : struct, IPixel<TPixel>
        {
            using (IManagedByteBuffer row = this.memoryAllocator.AllocateManagedByteBuffer(width, AllocationOptions.Clean))
            {
                TPixel color = default;
                Span<byte> rowSpan = row.GetSpan();

                for (int y = 0; y < height; y++)
                {
                    this.currentStream.Read(row);
                    Span<TPixel> pixelRow = pixels.GetRowSpan(height - y - 1);
                    switch (colorMapPixelSizeInBytes)
                    {
                        case 2:
                            for (int x = 0; x < width; x++)
                            {
                                int colorIndex = rowSpan[x];
                                color.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref palette[colorIndex * colorMapPixelSizeInBytes]));
                                pixelRow[x] = color;
                            }

                            break;

                        case 3:
                            for (int x = 0; x < width; x++)
                            {
                                int colorIndex = rowSpan[x];
                                color.FromBgr24(Unsafe.As<byte, Bgr24>(ref palette[colorIndex * colorMapPixelSizeInBytes]));
                                pixelRow[x] = color;
                            }

                            break;

                        case 4:
                            for (int x = 0; x < width; x++)
                            {
                                int colorIndex = rowSpan[x];
                                color.FromBgra32(Unsafe.As<byte, Bgra32>(ref palette[colorIndex * colorMapPixelSizeInBytes]));
                                pixelRow[x] = color;
                            }

                            break;
                    }
                }
            }
        }

        private void ReadPalettedRle<TPixel>(int width, int height, Buffer2D<TPixel> pixels, byte[] palette, int colorMapPixelSizeInBytes)
            where TPixel : struct, IPixel<TPixel>
        {
            int bytesPerPixel = 1;
            using (IMemoryOwner<byte> buffer = this.memoryAllocator.Allocate<byte>(width * height * bytesPerPixel, AllocationOptions.Clean))
            {
                TPixel color = default;
                Span<byte> bufferSpan = buffer.GetSpan();
                this.UncompressRle(width, height, bufferSpan, bytesPerPixel: 1);

                for (int y = 0; y < height; y++)
                {
                    Span<TPixel> pixelRow = pixels.GetRowSpan(this.fileHeader.Height - y - 1);
                    int rowStartIdx = y * width * bytesPerPixel;
                    for (int x = 0; x < width; x++)
                    {
                        int idx = rowStartIdx + x;
                        switch (colorMapPixelSizeInBytes)
                        {
                            case 1:
                                color.FromGray8(Unsafe.As<byte, Gray8>(ref palette[bufferSpan[idx] * colorMapPixelSizeInBytes]));
                                break;
                            case 2:
                                color.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref palette[bufferSpan[idx] * colorMapPixelSizeInBytes]));
                                break;
                            case 3:
                                color.FromBgr24(Unsafe.As<byte, Bgr24>(ref palette[bufferSpan[idx] * colorMapPixelSizeInBytes]));
                                break;
                            case 4:
                                color.FromBgra32(Unsafe.As<byte, Bgra32>(ref palette[bufferSpan[idx] * colorMapPixelSizeInBytes]));
                                break;
                        }

                        pixelRow[x] = color;
                    }
                }
            }
        }

        private void ReadMonoChrome<TPixel>(int width, int height, Buffer2D<TPixel> pixels)
            where TPixel : struct, IPixel<TPixel>
        {
            using (IManagedByteBuffer row = this.memoryAllocator.AllocatePaddedPixelRowBuffer(width, 1, 0))
            {
                for (int y = 0; y < height; y++)
                {
                    this.currentStream.Read(row);
                    Span<TPixel> pixelSpan = pixels.GetRowSpan(height - y - 1);
                    PixelOperations<TPixel>.Instance.FromGray8Bytes(
                        this.configuration,
                        row.GetSpan(),
                        pixelSpan,
                        width);
                }
            }
        }

        private void ReadBgra16<TPixel>(int width, int height, Buffer2D<TPixel> pixels)
            where TPixel : struct, IPixel<TPixel>
        {
            using (IManagedByteBuffer row = this.memoryAllocator.AllocatePaddedPixelRowBuffer(width, 2, 0))
            using (IMemoryOwner<Bgra5551> bgraRow = this.memoryAllocator.Allocate<Bgra5551>(width))
            {
                Span<Bgra5551> bgraRowSpan = bgraRow.GetSpan();
                long currentPosition = this.currentStream.Position;
                for (int y = 0; y < this.fileHeader.Height; y++)
                {
                    this.currentStream.Read(row);
                    Span<TPixel> pixelSpan = pixels.GetRowSpan(height - y - 1);
                    PixelOperations<TPixel>.Instance.FromBgra5551Bytes(
                        this.configuration,
                        row.GetSpan(),
                        pixelSpan,
                        width);
                }

                // We need to set each alpha component value to fully opaque.
                this.MakeOpaque(pixels, currentPosition, row, bgraRowSpan);
            }
        }

        private void ReadBgr24<TPixel>(int width, int height, Buffer2D<TPixel> pixels)
            where TPixel : struct, IPixel<TPixel>
        {
            using (IManagedByteBuffer row = this.memoryAllocator.AllocatePaddedPixelRowBuffer(width, 3, 0))
            {
                for (int y = 0; y < height; y++)
                {
                    this.currentStream.Read(row);
                    Span<TPixel> pixelSpan = pixels.GetRowSpan(height - y - 1);
                    PixelOperations<TPixel>.Instance.FromBgr24Bytes(
                        this.configuration,
                        row.GetSpan(),
                        pixelSpan,
                        width);
                }
            }
        }

        private void ReadBgra32<TPixel>(int width, int height, Buffer2D<TPixel> pixels)
            where TPixel : struct, IPixel<TPixel>
        {
            using (IManagedByteBuffer row = this.memoryAllocator.AllocatePaddedPixelRowBuffer(width, 4, 0))
            {
                for (int y = 0; y < height; y++)
                {
                    this.currentStream.Read(row);
                    Span<TPixel> pixelSpan = pixels.GetRowSpan(height - y - 1);
                    PixelOperations<TPixel>.Instance.FromBgra32Bytes(
                        this.configuration,
                        row.GetSpan(),
                        pixelSpan,
                        width);
                }
            }
        }

        private void ReadRle<TPixel>(int width, int height, Buffer2D<TPixel> pixels, int bytesPerPixel)
            where TPixel : struct, IPixel<TPixel>
        {
            TPixel color = default;
            using (IMemoryOwner<byte> buffer = this.memoryAllocator.Allocate<byte>(width * height * bytesPerPixel, AllocationOptions.Clean))
            {
                Span<byte> bufferSpan = buffer.GetSpan();
                this.UncompressRle(width, height, bufferSpan, bytesPerPixel);
                for (int y = 0; y < height; y++)
                {
                    Span<TPixel> pixelRow = pixels.GetRowSpan(this.fileHeader.Height - y - 1);
                    int rowStartIdx = y * width * bytesPerPixel;
                    for (int x = 0; x < width; x++)
                    {
                        int idx = rowStartIdx + (x * bytesPerPixel);
                        switch (bytesPerPixel)
                        {
                            case 1:
                                color.FromGray8(Unsafe.As<byte, Gray8>(ref bufferSpan[idx]));
                                break;
                            case 2:
                                // Set bit 16 to 1, to treat it as opaque for Bgra5551.
                                bufferSpan[idx + 1] = (byte)(bufferSpan[idx + 1] | 128);
                                color.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref bufferSpan[idx]));
                                break;
                            case 3:
                                color.FromBgr24(Unsafe.As<byte, Bgr24>(ref bufferSpan[idx]));
                                break;
                            case 4:
                                color.FromBgra32(Unsafe.As<byte, Bgra32>(ref bufferSpan[idx]));
                                break;
                        }

                        pixelRow[x] = color;
                    }
                }
            }
        }

        private void UncompressRle(int width, int height, Span<byte> buffer, int bytesPerPixel)
        {
            int uncompressedPixels = 0;
            var pixel = new byte[bytesPerPixel];
            int totalPixels = width * height;
            while (uncompressedPixels < totalPixels)
            {
                byte runLengthByte = (byte)this.currentStream.ReadByte();

                // The high bit of a run length packet is set to 1.
                int highBit = runLengthByte >> 7;
                if (highBit == 1)
                {
                    int runLength = runLengthByte & 127;
                    this.currentStream.Read(pixel, 0, bytesPerPixel);
                    int bufferIdx = uncompressedPixels * bytesPerPixel;
                    for (int i = 0; i < runLength + 1; i++, uncompressedPixels++)
                    {
                        pixel.AsSpan().CopyTo(buffer.Slice(bufferIdx));
                        bufferIdx += bytesPerPixel;
                    }
                }
                else
                {
                    // Non-run-length encoded packet.
                    int runLength = runLengthByte;
                    int bufferIdx = uncompressedPixels * bytesPerPixel;
                    for (int i = 0; i < runLength + 1; i++, uncompressedPixels++)
                    {
                        this.currentStream.Read(pixel, 0, bytesPerPixel);
                        pixel.AsSpan().CopyTo(buffer.Slice(bufferIdx));
                        bufferIdx += bytesPerPixel;
                    }
                }
            }
        }

        private void MakeOpaque<TPixel>(Buffer2D<TPixel> pixels, long currentPosition, IManagedByteBuffer row, Span<Bgra5551> bgraRowSpan)
            where TPixel : struct, IPixel<TPixel>
        {
            // Reset our stream for a second pass.
            this.currentStream.Position = currentPosition;
            for (int y = 0; y < this.fileHeader.Height; y++)
            {
                this.currentStream.Read(row);
                PixelOperations<Bgra5551>.Instance.FromBgra5551Bytes(
                    this.configuration,
                    row.GetSpan(),
                    bgraRowSpan,
                    this.fileHeader.Width);
                Span<TPixel> pixelSpan = pixels.GetRowSpan(this.fileHeader.Height - y - 1);

                for (int x = 0; x < this.fileHeader.Width; x++)
                {
                    Bgra5551 bgra = bgraRowSpan[x];
                    bgra.PackedValue = (ushort)(bgra.PackedValue | (1 << 15));
                    ref TPixel pixel = ref pixelSpan[x];
                    pixel.FromBgra5551(bgra);
                }
            }
        }

        /// <summary>
        /// Reads the raw image information from the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        public IImageInfo Identify(Stream stream)
        {
            this.ReadFileHeader(stream);
            return new ImageInfo(
                new PixelTypeInfo(this.fileHeader.PixelDepth),
                this.fileHeader.Width,
                this.fileHeader.Height,
                this.metadata);
        }

        /// <summary>
        /// Reads the tga file header from the stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        private void ReadFileHeader(Stream stream)
        {
            this.currentStream = stream;

#if NETCOREAPP2_1
            Span<byte> buffer = stackalloc byte[TgaFileHeader.Size];
#else
            var buffer = new byte[TgaFileHeader.Size];
#endif
            this.currentStream.Read(buffer, 0, TgaFileHeader.Size);
            this.fileHeader = TgaFileHeader.Parse(buffer);
            this.metadata = new ImageMetadata();
            this.tgaMetadata = this.metadata.GetFormatMetadata(TgaFormat.Instance);
            this.tgaMetadata.BitsPerPixel = (TgaBitsPerPixel)this.fileHeader.PixelDepth;
        }
    }
}
