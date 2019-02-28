using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialClient.Models
{
    public class CircleQueue<T>
    {
        private T[] arr;
        private int FrontIndex, RearIndex, Capacity;
        private T FrontItem,RearItem;

        public CircleQueue(int Size)
        {
            if (Size < 1) return;
            arr = new T[Size];
            FrontIndex = RearIndex = 0;
            Capacity = 0;
        }
        public bool IsEmpty()
        {
            return (Capacity == 0);
        }
        public T[] Peek(int Count)
        {


            
            return arr.Skip(FrontIndex).Take(Count).ToArray();
        }
        public T[] Push(T Item)
        {
            if (Capacity==0)
            {
                arr[0] = Item;
                FrontItem =RearItem =Item;
                FrontIndex = RearIndex = 0;
                Capacity++;
            }
            if (arr.Length < RearIndex)
            {
                RearIndex = 0;
            }
            arr[++RearIndex] = Item;
            Capacity++;
            return arr;
        }
        public T[] Push(T[] Items)
        {
            foreach (T item in Items)
            {
                arr[++RearIndex ] = item;
                Capacity++;
            }
            return arr;
        }
        public int Length()
        {
            return Capacity;
        }
        //public T[] GetRear(int Count)
        //{
        //    Capacity = Capacity-Count;
        //    for (int i = 0; i < length; i++)
        //    {

        //    }
        //    if (RearIndex < 0)
        //    {
        //        RearIndex = arr.Length - 1;
        //    }
        //    var Temp = arr.Skip(RearIndex - Count).Take(Count).ToArray<T>();
        //    arr= 
        //    return Temp;
        //}
        //public T GetRear()
        //{
        //    return RearItem;
        //}
        public T GetFront()
        {
           
            FrontItem = arr[FrontIndex++];
            //var temp = arr[FrontIndex];
            //FrontIndex++;
            return FrontItem;
        }
        public T[] GetFront(int Count)
        {
            var Temp= arr.Take(Count).ToArray<T>();

            arr = arr.Skip(Count).ToArray();
            return Temp;
        }


    }
}
