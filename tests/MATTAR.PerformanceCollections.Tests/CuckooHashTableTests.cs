using System;
using Xunit;

namespace MATTAR.PerformanceCollections.Tests;

public unsafe class CuckooHashTableTests
{
    [Fact]
    public void Create_InitialisesEmptyTable()
    {
        var table = CuckooHashTable.Create(16);
        try
        {
            Assert.Equal(0, table->Count);
        }
        finally
        {
            CuckooHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Insert_And_Find_SingleEntry()
    {
        var table = CuckooHashTable.Create(8);
        try
        {
            CuckooHashTable.Insert(table, 1, 100);
            var entry = CuckooHashTable.Find(table, 1);
            Assert.True(entry != null);
            Assert.Equal(1, entry->Key);
            Assert.Equal(100, entry->Value);
        }
        finally
        {
            CuckooHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Find_MissingKey_ReturnsNull()
    {
        var table = CuckooHashTable.Create(8);
        try
        {
            var entry = CuckooHashTable.Find(table, 42);
            Assert.True(entry == null);
        }
        finally
        {
            CuckooHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Insert_DuplicateKey_UpdatesValue()
    {
        var table = CuckooHashTable.Create(8);
        try
        {
            CuckooHashTable.Insert(table, 5, 10);
            CuckooHashTable.Insert(table, 5, 99);
            var entry = CuckooHashTable.Find(table, 5);
            Assert.True(entry != null);
            Assert.Equal(99, entry->Value);
        }
        finally
        {
            CuckooHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Delete_ExistingKey_RemovesEntry()
    {
        var table = CuckooHashTable.Create(8);
        try
        {
            CuckooHashTable.Insert(table, 3, 30);
            bool deleted = CuckooHashTable.Delete(table, 3);
            Assert.True(deleted);
            Assert.True(CuckooHashTable.Find(table, 3) == null);
        }
        finally
        {
            CuckooHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Delete_NonExistingKey_ReturnsFalse()
    {
        var table = CuckooHashTable.Create(8);
        try
        {
            bool deleted = CuckooHashTable.Delete(table, 99);
            Assert.False(deleted);
        }
        finally
        {
            CuckooHashTable.Destroy(table);
        }
    }

    [Fact]
    public void Insert_ManyKeys_AllRetrievable()
    {
        var table = CuckooHashTable.Create(32);
        try
        {
            for (int i = 1; i <= 50; i++)
                CuckooHashTable.Insert(table, i, i * 10);

            for (int i = 1; i <= 50; i++)
            {
                var entry = CuckooHashTable.Find(table, i);
                Assert.True(entry != null);
                Assert.Equal(i * 10, entry->Value);
            }
        }
        finally
        {
            CuckooHashTable.Destroy(table);
        }
    }
}
