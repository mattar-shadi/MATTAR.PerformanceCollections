using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A safe, managed wrapper around the unsafe <see cref="VanEmdeBoas"/> tree that
/// implements <see cref="IEnumerable{T}"/> so callers never need to write unsafe code.
/// </summary>
public sealed unsafe class SafeVanEmdeBoas : IEnumerable<int>, IDisposable
{
    private VanEmdeBoas* _root;
    private bool _disposed;

    /// <summary>
    /// Initialises a new Van Emde Boas tree that can hold keys in [0, 2^<paramref name="universeBits"/>).
    /// </summary>
    /// <param name="universeBits">
    /// Number of bits in the universe size (2 – 30).
    /// Keys must satisfy 0 ≤ key &lt; 2^universeBits.
    /// </param>
    public SafeVanEmdeBoas(int universeBits = 20)
    {
        _root = VanEmdeBoas.Create(universeBits);
    }

    /// <summary>Minimum element in the tree, or -1 if the tree is empty.</summary>
    public int Min
    {
        get
        {
            ThrowIfDisposed();
            return _root->Min;
        }
    }

    /// <summary>Maximum element in the tree, or -1 if the tree is empty.</summary>
    public int Max
    {
        get
        {
            ThrowIfDisposed();
            return _root->Max;
        }
    }

    /// <summary>Returns <see langword="true"/> if the tree contains no elements.</summary>
    public bool IsEmpty
    {
        get
        {
            ThrowIfDisposed();
            return _root->Min == -1;
        }
    }

    /// <summary>Inserts <paramref name="key"/> into the tree.</summary>
    public void Insert(int key)
    {
        ThrowIfDisposed();
        VanEmdeBoas.Insert(_root, key);
    }

    /// <summary>
    /// Returns the smallest key strictly greater than <paramref name="x"/>,
    /// or -1 if no such key exists.
    /// </summary>
    public int Successor(int x)
    {
        ThrowIfDisposed();
        return VanEmdeBoas.Successor(_root, x);
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="key"/> is present in the tree.</summary>
    public bool Contains(int key)
    {
        ThrowIfDisposed();
        if (_root->Min == -1) return false;
        if (_root->Min == key || _root->Max == key) return true;
        // Successor of (key - 1) equals key iff key is present.
        return key > 0 && VanEmdeBoas.Successor(_root, key - 1) == key;
    }

    /// <inheritdoc/>
    public IEnumerator<int> GetEnumerator()
    {
        ThrowIfDisposed();
        return CollectElements().GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private List<int> CollectElements()
    {
        var list = new List<int>();
        if (_root->Min == -1) return list;

        int current = _root->Min;
        while (current != -1)
        {
            list.Add(current);
            current = VanEmdeBoas.Successor(_root, current);
        }
        return list;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            VanEmdeBoas.Destroy(_root);
            _root = null;
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SafeVanEmdeBoas));
    }
}
