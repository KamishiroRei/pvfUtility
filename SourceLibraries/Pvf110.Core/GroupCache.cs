namespace Pvf110.Core;

/// <summary>
/// body 组解压缓存：容量受限的 LRU 缓存。
/// 组明文解压一次后按字节预算保留，超过预算淘汰最久未访问的组，
/// 避免浏览/搜索/保存把整个解压包永久驻留在内存中（旧实现为无上限 ConcurrentDictionary）。
/// </summary>
sealed class GroupCache
{
    /// <summary>默认缓存字节预算：256 MiB，覆盖大部分热组，同时封顶常驻内存。</summary>
    internal const long DefaultCapacityBytes = 256L << 20;

    private readonly object _gate = new();
    private readonly Dictionary<int, byte[]> _map = new();
    private readonly Dictionary<int, LinkedListNode<int>> _nodes = new();
    private readonly LinkedList<int> _lru = new();
    private readonly long _capacityBytes;
    private long _bytes;

    internal GroupCache(long capacityBytes = DefaultCapacityBytes)
    {
        if (capacityBytes <= 0) throw new ArgumentOutOfRangeException(nameof(capacityBytes));
        _capacityBytes = capacityBytes;
    }

    /// <summary>取缓存或解压（解压在锁外执行，命中/插入在锁内，并发下同组只保留一份）。</summary>
    public byte[] GetOrAdd(int key, Func<int, byte[]> factory)
    {
        lock (_gate)
        {
            if (_map.TryGetValue(key, out byte[]? cached))
            {
                TouchLocked(key);
                return cached;
            }
        }

        byte[] value = factory(key);

        lock (_gate)
        {
            if (_map.TryGetValue(key, out byte[]? winner))
            {
                TouchLocked(key);
                return winner; // 并发下另一线程已完成同组解压
            }
            long size = value.LongLength;
            if (size > _capacityBytes)
                return value; // 单组超出预算：不缓存，直接返回给调用方
            while (_bytes + size > _capacityBytes && _lru.Count > 0)
                EvictOldestLocked();
            _map[key] = value;
            _nodes[key] = _lru.AddLast(key);
            _bytes += size;
            return value;
        }
    }

    private void TouchLocked(int key)
    {
        if (_nodes.Remove(key, out LinkedListNode<int>? node))
        {
            _lru.Remove(node);
            _nodes[key] = _lru.AddLast(key);
        }
    }

    private void EvictOldestLocked()
    {
        LinkedListNode<int> oldest = _lru.First ?? throw new InvalidOperationException("LRU empty");
        _lru.RemoveFirst();
        int key = oldest.Value;
        _nodes.Remove(key);
        _bytes -= _map[key].LongLength;
        _map.Remove(key);
    }
}
