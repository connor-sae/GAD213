using System;
using UnityEngine;

public struct Stack<T> where T : class
{
    private T[] values;
    private int topPointer;

    public Stack(int StackSize)
    {
        topPointer = 0;
        values = new T[StackSize];
    }

    public bool IsEmpty()
    {
        return topPointer <= 0;
    }

    public bool IsFull()
    {
        return topPointer >= values.Length;
    }

    public void Push(T item)
    {
        if(IsFull())
        {
            Debug.LogWarning("tried to Push to a full Stack: try running Stack.IsFull() \n Returning Null");
            return;
        }
        values[topPointer] = item;
        topPointer ++;
    }

    public T Pop()
    {
        if(IsEmpty())
        {
            Debug.LogWarning("tried to pull from an empty Stack: try running Stack.IsEmpty() \n Returning Null");
            return null;
        }
        else
        {
            topPointer --;
            return values[topPointer];
        }
    }

    public T Remove(T item)
    {
        T found = null;
        
        for(int i = 0; i < topPointer; i ++)
        {
            if(found == null) // if not found try find
            {
                if(values[i] != null && values[i].Equals(item))
                {
                    found = values[i];
                }

            }
            else // if the item was already found, push down the stack
            {
                values[i-1] = values[1];
            }
        }

        if(found != null)
            topPointer -- ;

        return found;
    }
}
