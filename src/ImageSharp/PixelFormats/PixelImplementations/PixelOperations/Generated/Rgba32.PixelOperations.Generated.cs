// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

// <auto-generated />

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats.Utils;

namespace SixLabors.ImageSharp.PixelFormats;

/// <content>
/// Provides optimized overrides for bulk operations.
/// </content>
public partial struct Rgba32
{
    /// <summary>
    /// Provides optimized overrides for bulk operations.
    /// </summary>
    internal partial class PixelOperations : PixelOperations<Rgba32>
    {
        /// <inheritdoc />
        public override void FromRgba32(Configuration configuration, ReadOnlySpan<Rgba32> source, Span<Rgba32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            source.CopyTo(destination.Slice(0, source.Length));
        }

        /// <inheritdoc />
        public override void ToRgba32(Configuration configuration, ReadOnlySpan<Rgba32> source, Span<Rgba32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            source.CopyTo(destination.Slice(0, source.Length));
        }
    
        /// <inheritdoc />
        public override void ToArgb32(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<Argb32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Rgba32, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Argb32, byte>(destination);
            PixelConverter.FromRgba32.ToArgb32(sourceBytes, destinationBytes);
        }
        
        /// <inheritdoc />
        public override void FromArgb32(
            Configuration configuration,
            ReadOnlySpan<Argb32> source,
            Span<Rgba32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Argb32, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Rgba32, byte>(destination);
            PixelConverter.FromArgb32.ToRgba32(sourceBytes, destinationBytes);
        }        
    
        /// <inheritdoc />
        public override void ToAbgr32(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<Abgr32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Rgba32, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Abgr32, byte>(destination);
            PixelConverter.FromRgba32.ToAbgr32(sourceBytes, destinationBytes);
        }
        
        /// <inheritdoc />
        public override void FromAbgr32(
            Configuration configuration,
            ReadOnlySpan<Abgr32> source,
            Span<Rgba32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Abgr32, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Rgba32, byte>(destination);
            PixelConverter.FromAbgr32.ToRgba32(sourceBytes, destinationBytes);
        }        
    
        /// <inheritdoc />
        public override void ToBgra32(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<Bgra32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Rgba32, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Bgra32, byte>(destination);
            PixelConverter.FromRgba32.ToBgra32(sourceBytes, destinationBytes);
        }
        
        /// <inheritdoc />
        public override void FromBgra32(
            Configuration configuration,
            ReadOnlySpan<Bgra32> source,
            Span<Rgba32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Bgra32, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Rgba32, byte>(destination);
            PixelConverter.FromBgra32.ToRgba32(sourceBytes, destinationBytes);
        }        
    
        /// <inheritdoc />
        public override void ToRgb24(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<Rgb24> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Rgba32, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Rgb24, byte>(destination);
            PixelConverter.FromRgba32.ToRgb24(sourceBytes, destinationBytes);
        }
        
        /// <inheritdoc />
        public override void FromRgb24(
            Configuration configuration,
            ReadOnlySpan<Rgb24> source,
            Span<Rgba32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Rgb24, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Rgba32, byte>(destination);
            PixelConverter.FromRgb24.ToRgba32(sourceBytes, destinationBytes);
        }        
    
        /// <inheritdoc />
        public override void ToBgr24(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<Bgr24> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Rgba32, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Bgr24, byte>(destination);
            PixelConverter.FromRgba32.ToBgr24(sourceBytes, destinationBytes);
        }
        
        /// <inheritdoc />
        public override void FromBgr24(
            Configuration configuration,
            ReadOnlySpan<Bgr24> source,
            Span<Rgba32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ReadOnlySpan<byte> sourceBytes = MemoryMarshal.Cast<Bgr24, byte>(source);
            Span<byte> destinationBytes = MemoryMarshal.Cast<Rgba32, byte>(destination);
            PixelConverter.FromBgr24.ToRgba32(sourceBytes, destinationBytes);
        }        

        /// <inheritdoc />
        public override void ToL8(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<L8> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ref Rgba32 sourceBase = ref MemoryMarshal.GetReference(source);
            ref L8 destinationBase = ref MemoryMarshal.GetReference(destination);

            for (nuint i = 0; i < (uint)source.Length; i++)
            {
                Unsafe.Add(ref destinationBase, i) = L8.FromRgba32(Unsafe.Add(ref sourceBase, i));
            }
        }

        /// <inheritdoc />
        public override void ToL16(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<L16> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ref Rgba32 sourceBase = ref MemoryMarshal.GetReference(source);
            ref L16 destinationBase = ref MemoryMarshal.GetReference(destination);

            for (nuint i = 0; i < (uint)source.Length; i++)
            {
                Unsafe.Add(ref destinationBase, i) = L16.FromRgba32(Unsafe.Add(ref sourceBase, i));
            }
        }

        /// <inheritdoc />
        public override void ToLa16(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<La16> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ref Rgba32 sourceBase = ref MemoryMarshal.GetReference(source);
            ref La16 destinationBase = ref MemoryMarshal.GetReference(destination);

            for (nuint i = 0; i < (uint)source.Length; i++)
            {
                Unsafe.Add(ref destinationBase, i) = La16.FromRgba32(Unsafe.Add(ref sourceBase, i));
            }
        }

        /// <inheritdoc />
        public override void ToLa32(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<La32> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ref Rgba32 sourceBase = ref MemoryMarshal.GetReference(source);
            ref La32 destinationBase = ref MemoryMarshal.GetReference(destination);

            for (nuint i = 0; i < (uint)source.Length; i++)
            {
                Unsafe.Add(ref destinationBase, i) = La32.FromRgba32(Unsafe.Add(ref sourceBase, i));
            }
        }

        /// <inheritdoc />
        public override void ToRgb48(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<Rgb48> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ref Rgba32 sourceBase = ref MemoryMarshal.GetReference(source);
            ref Rgb48 destinationBase = ref MemoryMarshal.GetReference(destination);

            for (nuint i = 0; i < (uint)source.Length; i++)
            {
                Unsafe.Add(ref destinationBase, i) = Rgb48.FromRgba32(Unsafe.Add(ref sourceBase, i));
            }
        }

        /// <inheritdoc />
        public override void ToRgba64(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<Rgba64> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ref Rgba32 sourceBase = ref MemoryMarshal.GetReference(source);
            ref Rgba64 destinationBase = ref MemoryMarshal.GetReference(destination);

            for (nuint i = 0; i < (uint)source.Length; i++)
            {
                Unsafe.Add(ref destinationBase, i) = Rgba64.FromRgba32(Unsafe.Add(ref sourceBase, i));
            }
        }

        /// <inheritdoc />
        public override void ToBgra5551(
            Configuration configuration,
            ReadOnlySpan<Rgba32> source,
            Span<Bgra5551> destination)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));

            ref Rgba32 sourceBase = ref MemoryMarshal.GetReference(source);
            ref Bgra5551 destinationBase = ref MemoryMarshal.GetReference(destination);

            for (nuint i = 0; i < (uint)source.Length; i++)
            {
                Unsafe.Add(ref destinationBase, i) = Bgra5551.FromRgba32(Unsafe.Add(ref sourceBase, i));
            }
        }

        /// <inheritdoc />
        public override void From<TSourcePixel>(
            Configuration configuration,
            ReadOnlySpan<TSourcePixel> source,
            Span<Rgba32> destination)
        {
            PixelOperations<TSourcePixel>.Instance.ToRgba32(configuration, source, destination.Slice(0, source.Length));
        }
    }
}
