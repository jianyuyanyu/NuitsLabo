﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FixedLengthFileStudy;

internal ref struct ValueByteArrayBuilder(Span<byte> initialBuffer)
{
    private byte[]? _arrayToReturnToPool = null;
    private Span<byte> _bytes = initialBuffer;
    private int _pos = 0;

    public bool IsEmpty => _pos == 0;

    public void Clear() => _pos = 0;

    public ReadOnlySpan<byte> AsSpan() => _bytes.Slice(0, _pos);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(byte c)
    {
        int pos = _pos;
        Span<byte> chars = _bytes;
        if (pos < chars.Length)
        {
            chars[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    public void Append(scoped ReadOnlySpan<byte> value)
    {
        int pos = _pos;
        if (_bytes.Length < pos + value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_bytes.Slice(_pos));
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(byte c)
    {
        Grow(1);
        Append(c);
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _bytes.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        int newCapacity = (int)Math.Max(
            (uint)(_pos + additionalCapacityBeyondPos),
            Math.Min((uint)_bytes.Length * 2, ArrayMaxLength));

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
        // This could also go negative if the actual required length wraps around.
        byte[] poolArray = ArrayPool<byte>.Shared.Rent(newCapacity);

        _bytes.Slice(0, _pos).CopyTo(poolArray);

        byte[]? toReturn = _arrayToReturnToPool;
        _bytes = _arrayToReturnToPool = poolArray;
        if (toReturn != null)
        {
            ArrayPool<byte>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        byte[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
        {
            ArrayPool<byte>.Shared.Return(toReturn);
        }
    }
}