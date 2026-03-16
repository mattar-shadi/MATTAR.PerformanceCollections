using System;
using Xunit;

namespace MATTAR.PerformanceCollections.Tests;

public unsafe class VanEmdeBoasTests
{
    [Fact]
    public void Insert_SingleElement_MinAndMaxAreSet()
    {
        var v = VanEmdeBoas.Create(8);
        try
        {
            VanEmdeBoas.Insert(v, 42);
            Assert.Equal(42, v->Min);
            Assert.Equal(42, v->Max);
        }
        finally
        {
            VanEmdeBoas.Destroy(v);
        }
    }

    [Fact]
    public void Insert_MultipleElements_MinAndMaxCorrect()
    {
        var v = VanEmdeBoas.Create(8);
        try
        {
            VanEmdeBoas.Insert(v, 10);
            VanEmdeBoas.Insert(v, 5);
            VanEmdeBoas.Insert(v, 200);
            Assert.Equal(5, v->Min);
            Assert.Equal(200, v->Max);
        }
        finally
        {
            VanEmdeBoas.Destroy(v);
        }
    }

    [Fact]
    public void Successor_ReturnsNextElement()
    {
        var v = VanEmdeBoas.Create(8);
        try
        {
            VanEmdeBoas.Insert(v, 3);
            VanEmdeBoas.Insert(v, 7);
            VanEmdeBoas.Insert(v, 15);

            Assert.Equal(7, VanEmdeBoas.Successor(v, 3));
            Assert.Equal(15, VanEmdeBoas.Successor(v, 7));
            Assert.Equal(-1, VanEmdeBoas.Successor(v, 15));
        }
        finally
        {
            VanEmdeBoas.Destroy(v);
        }
    }

    [Fact]
    public void Successor_OnEmptyTree_ReturnsNegativeOne()
    {
        var v = VanEmdeBoas.Create(8);
        try
        {
            Assert.Equal(-1, VanEmdeBoas.Successor(v, 0));
        }
        finally
        {
            VanEmdeBoas.Destroy(v);
        }
    }

    [Fact]
    public void Insert_KeyBelowMin_UpdatesMin()
    {
        var v = VanEmdeBoas.Create(8);
        try
        {
            VanEmdeBoas.Insert(v, 50);
            VanEmdeBoas.Insert(v, 10);
            Assert.Equal(10, v->Min);
        }
        finally
        {
            VanEmdeBoas.Destroy(v);
        }
    }

    [Fact]
    public void Insert_OutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var v = VanEmdeBoas.Create(4); // universe [0, 16)
        try
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => VanEmdeBoas.Insert(v, 16));
            Assert.Throws<ArgumentOutOfRangeException>(() => VanEmdeBoas.Insert(v, -1));
        }
        finally
        {
            VanEmdeBoas.Destroy(v);
        }
    }

    [Fact]
    public void Create_UniverseBitsAboveMax_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => VanEmdeBoas.Create(VanEmdeBoas.MAX_UNIVERSE_BITS + 1));
    }
}
