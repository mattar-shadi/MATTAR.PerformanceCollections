using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A safe, managed wrapper around an unmanaged <see cref="VanEmdeBoas"/> instance that
/// exposes its elements as <see cref="IEnumerable{T}">IEnumerable&lt;int&gt;</see>.
/// All unsafe pointer operations are encapsulated internally.
/// </summary>
public sealed unsafe class SafeVanEmdeBoas : IEnumerable<int>, IDisposable
{
    private VanEmdeBoas* _tree;
    private bool _disposed;

    /// <summary>
    /// Initialises a new instance that takes ownership of the given unmanaged tree.
    /// The tree will be destroyed when this object is disposed.
    /// </summary>
    /// <param name="tree">Pointer to an allocated <see cref="VanEmdeBoas"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tree"/> is <see langword="null"/>.</exception>
    public SafeVanEmdeBoas(VanEmdeBoas* tree)
    {
        if (tree == null)
            throw new ArgumentNullException(nameof(tree));
        _tree = tree;
    }

    /// <summary>Returns <see langword="true"/> when the tree contains no elements.</summary>
    /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
    public bool IsEmpty
    {
        get
        {
            ThrowIfDisposed();
            return _tree->Min == -1;
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
    public IEnumerator<int> GetEnumerator()
    {
        ThrowIfDisposed();
        return new SafeVanEmdeBoasEnumerator(_tree);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Destroys the underlying unmanaged tree and frees all associated memory.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            VanEmdeBoas.Destroy(_tree);
            _tree = null;
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SafeVanEmdeBoas));
    }

    // -------------------------------------------------------------------------
    // Enumerator
    // -------------------------------------------------------------------------

    /// <summary>
    /// Enumerator that iterates over every element in a <see cref="VanEmdeBoas"/> tree
    /// in ascending order by repeatedly calling <see cref="VanEmdeBoas.Successor"/>.
    /// </summary>
    public sealed unsafe class SafeVanEmdeBoasEnumerator : IEnumerator<int>
    {
        private readonly VanEmdeBoas* _tree;
        private int _current;
        private bool _started;

        internal SafeVanEmdeBoasEnumerator(VanEmdeBoas* tree)
        {
            _tree = tree;
            _current = -1;
            _started = false;
        }

        /// <inheritdoc/>
        public int Current => _current;

        /// <inheritdoc/>
        object IEnumerator.Current => _current;

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (_tree == null || _tree->Min == -1)
                return false;

            if (!_started)
            {
                _started = true;
                _current = _tree->Min;
                return true;
            }

            int next = VanEmdeBoas.Successor(_tree, _current);
            if (next == -1)
                return false;

            _current = next;
            return true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _current = -1;
            _started = false;
        }

        /// <inheritdoc/>
        public void Dispose() { }
    }
}
