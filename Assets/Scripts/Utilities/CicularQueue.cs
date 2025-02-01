using System.Drawing;
using System;
using TMPro;

public class CircularQueue<T>
{
    private T[] _Data;
    private int Start, _End;
    private int _Capacity;
    private int _Size;

    public int Size => _Size;
    public int Capacity => _Capacity;
    public bool IsFull => _Size == _Capacity;
    public bool IsEmpty => _Size == 0;
    public T Last => IsEmpty ? default! : _Data[_End];

    public CircularQueue(int size)
    {
        _Data = new T[size];
        _Capacity = size;
        Start = _End = -1;
        this._Size = 0;
    }

    public void Enqueue(T newData)
    {
        if (IsFull)
        {
            // Move start forward to overwrite the oldest element
            Start = (Start + 1) % _Capacity;
        }
        else
        {
            if (IsEmpty)
                Start = 0; // Initialize start position
            _Size++;
        }

        // Move end forward (wraps around when reaching capacity)
        _End = (_End + 1) % _Capacity;
        _Data[_End] = newData;
    }

    public T Dequeue()
    {
        if (IsEmpty)
            return default!;

        T oldData = _Data[Start];
        _Size--;

        if (IsEmpty)
            Start = _End = -1;               // Reset when queue is empty
        else
            Start = (Start + 1) % _Capacity; // Move start forward circularly

        return oldData;
    }

    public T this[int index] => _Data[(Start + index) % _Capacity];

    public void Clear()
    {
        Start = _End = -1;
        _Size = 0;
    }

    public void Reverse()
    {
        if (IsEmpty) return;

        for (int i = 0; i < _Size / 2; i++)
        {
            int firstIndex = (Start + i) % _Capacity;
            int lastIndex = (Start + _Size - 1 - i) % _Capacity;

            (_Data[firstIndex], _Data[lastIndex]) = (_Data[lastIndex], _Data[firstIndex]);
        }
    }
}