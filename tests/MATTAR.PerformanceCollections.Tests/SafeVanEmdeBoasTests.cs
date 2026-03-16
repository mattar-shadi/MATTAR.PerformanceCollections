using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MATTAR.PerformanceCollections.Tests;

public class SafeVanEmdeBoasTests
{
    [Fact]
    public void NewTree_IsEmpty()
    {
        using var tree = new SafeVanEmdeBoas(8);
        Assert.True(tree.IsEmpty);
        Assert.Equal(-1, tree.Min);
        Assert.Equal(-1, tree.Max);
    }

    [Fact]
    public void Insert_SingleElement_MinAndMaxAreSet()
    {
        using var tree = new SafeVanEmdeBoas(8);
        tree.Insert(42);
        Assert.Equal(42, tree.Min);
        Assert.Equal(42, tree.Max);
        Assert.False(tree.IsEmpty);
    }

    [Fact]
    public void Contains_InsertedKey_ReturnsTrue()
    {
        using var tree = new SafeVanEmdeBoas(8);
        tree.Insert(10);
        tree.Insert(20);
        Assert.True(tree.Contains(10));
        Assert.True(tree.Contains(20));
        Assert.False(tree.Contains(15));
    }

    [Fact]
    public void Successor_ReturnsNextElement()
    {
        using var tree = new SafeVanEmdeBoas(8);
        tree.Insert(5);
        tree.Insert(10);
        tree.Insert(20);
        Assert.Equal(10, tree.Successor(5));
        Assert.Equal(20, tree.Successor(10));
        Assert.Equal(-1, tree.Successor(20));
    }

    [Fact]
    public void GetEnumerator_ReturnsElementsInAscendingOrder()
    {
        using var tree = new SafeVanEmdeBoas(8);
        int[] keys = { 15, 3, 200, 1, 42 };
        foreach (int k in keys)
            tree.Insert(k);

        List<int> result = tree.ToList();
        List<int> expected = keys.OrderBy(x => x).ToList();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetEnumerator_EmptyTree_ReturnsNoElements()
    {
        using var tree = new SafeVanEmdeBoas(8);
        Assert.Empty(tree);
    }

    [Fact]
    public void Dispose_PreventsSubsequentUse()
    {
        var tree = new SafeVanEmdeBoas(8);
        tree.Dispose();
        Assert.Throws<ObjectDisposedException>(() => tree.Insert(1));
        Assert.Throws<ObjectDisposedException>(() => _ = tree.Min);
        Assert.Throws<ObjectDisposedException>(() => tree.GetEnumerator());
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        var tree = new SafeVanEmdeBoas(8);
        tree.Dispose();
        tree.Dispose(); // second call must be a no-op
    }

    [Fact]
    public void Insert_ManyElements_EnumerationIsComplete()
    {
        using var tree = new SafeVanEmdeBoas(10); // universe [0, 1024)
        var expected = new List<int> { 0, 1, 2, 100, 511, 1023 };
        foreach (int k in expected)
            tree.Insert(k);

        Assert.Equal(expected, tree.ToList());
    }

    [Fact]
    public void Insert_DuplicateKey_DoesNotDuplicateInEnumeration()
    {
        using var tree = new SafeVanEmdeBoas(8);
        tree.Insert(7);
        tree.Insert(7);
        Assert.Single(tree);
    }
}
