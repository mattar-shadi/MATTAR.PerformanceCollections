using System;
using Xunit;

namespace MATTAR.PerformanceCollections.Tests;

public unsafe class PerfectHashTableTests
{
    [Fact]
    public void Create_WithKeysAndValues_FindsAllEntries()
    {
        int[] keys = { 1, 2, 3, 4, 5 };
        int[] values = { 10, 20, 30, 40, 50 };

        var table = PerfectHashTable.Create(keys, values);
        try
        {
            for (int i = 0; i < keys.Length; i++)
            {
                var entry = PerfectHashTable.Find(table, keys[i]);
                Assert.True(entry != null);
                Assert.Equal(keys[i], entry->Key);
                Assert.Equal(values[i], entry->Value);
            }
        }
        finally
        {
            PerfectHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Find_MissingKey_ReturnsNull()
    {
        int[] keys = { 7, 14, 21 };
        int[] values = { 1, 2, 3 };

        var table = PerfectHashTable.Create(keys, values);
        try
        {
            var entry = PerfectHashTable.Find(table, 99);
            Assert.True(entry == null);
        }
        finally
        {
            PerfectHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Create_EmptyKeys_ReturnsNull()
    {
        var table = PerfectHashTable.Create(Array.Empty<int>(), Array.Empty<int>());
        Assert.True(table == null);
    }

    [Fact]
    public void Create_SingleKey_FindsEntry()
    {
        int[] keys = { 42 };
        int[] values = { 999 };

        var table = PerfectHashTable.Create(keys, values);
        try
        {
            var entry = PerfectHashTable.Find(table, 42);
            Assert.True(entry != null);
            Assert.Equal(999, entry->Value);
        }
        finally
        {
            PerfectHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Create_ManyKeys_AllRetrievable()
    {
        const int n = 20;
        int[] keys = new int[n];
        int[] values = new int[n];
        for (int i = 0; i < n; i++)
        {
            keys[i] = (i + 1) * 3;
            values[i] = (i + 1) * 100;
        }

        var table = PerfectHashTable.Create(keys, values);
        try
        {
            for (int i = 0; i < n; i++)
            {
                var entry = PerfectHashTable.Find(table, keys[i]);
                Assert.True(entry != null);
                Assert.Equal(values[i], entry->Value);
            }
        }
        finally
        {
            PerfectHashTable.Destroy(table);
        }
    }
}
