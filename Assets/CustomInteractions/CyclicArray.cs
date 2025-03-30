using System;
using System.Collections.Generic;

public class CyclicArray<T>
{
    private T[] array;
    private int capacity;
    private int head;
    private int tail;
    private int size;

    public CyclicArray(int capacity)
    {
        this.capacity = capacity;
        this.array = new T[capacity];
        this.head = 0;
        this.tail = 0;
        this.size = 0;
    }

    public void Add(T item)
    {
        array[tail] = item;
        if (size == capacity)
        {
            // Overwrite the oldest element
            head = (head + 1) % capacity;
        }
        else
        {
            size++;
        }
        tail = (tail + 1) % capacity;
    }

    public T Get(int index)
    {
        if (index < 0 || index >= size)
        {
            throw new IndexOutOfRangeException();
        }
        return array[(head + index) % capacity];
    }

    public T GetNewest() 
    {
        return Get(0);
    }

    public List<T> GetAllElementInOrder()
    {
        List<T> elements = new List<T>();
        for (int i = 0; i < size; i++)
        {
            elements.Add(array[(head + i) % capacity]);
        }
        return elements;
    }

    public int Size => size;
    public int Capacity => capacity;
}