using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: CircularBuffer.cs
 * Author: Elvin Shen (not really)
 * What the program does: A circulating arrays
 * CREDITS: https://www.youtube.com/watch?v=-lGsuCEWkM0
 */

public class CircularBuffer<T>
{
    private T[] buffer;
    private int bufferSize;

    public CircularBuffer(int bufferSize){
        this.bufferSize = bufferSize;
        buffer = new T[bufferSize];
    }

    public void Add(T item, int index) => buffer[index % bufferSize] = item;
    public T Get(int index) => buffer[index % bufferSize];
    public void Clear() => buffer = new T[bufferSize];
}
