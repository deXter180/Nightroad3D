using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Heap<HeapObject> where HeapObject : IHeapItem<HeapObject>
{
    HeapObject[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new HeapObject[maxHeapSize];
    }

    public void Add(HeapObject item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public HeapObject RemoveFirstItem()
    {
        HeapObject firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public int Count()
    {
        return currentItemCount;
    }

    public bool ContainItem(HeapObject item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public void UpdateItem(HeapObject item)
    {
        SortUp(item);
    }

    private void SortDown(HeapObject item)
    {
        while (true)
        {
            int childIndexLeft = GetChildLeft(item);
            int childIndexRight = GetChildRight(item);
            int swapIndex = 0;
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else return;
            }
            else return; 
        }
    }

    private void SortUp(HeapObject item)
    {
        int parentIndex = GetParentIndex(item);
        while (true)
        {
            HeapObject parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }else
            {
                break;
            }
            parentIndex = GetParentIndex(item);
        }
    }

    private int GetParentIndex(HeapObject item)
    {
        return (item.HeapIndex - 1) / 2;
    }

    private int GetChildLeft(HeapObject item)
    {
        return (item.HeapIndex * 2) + 1;
    }

    private int GetChildRight(HeapObject item)
    {
        return (item.HeapIndex * 2) + 2;
    }

    private void Swap(HeapObject itemA, HeapObject itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

}

    

