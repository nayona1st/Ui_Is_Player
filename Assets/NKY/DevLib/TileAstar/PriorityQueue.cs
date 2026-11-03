using System;
using System.Collections.Generic;

namespace DevLib.TileAstar
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public List<T> heap = new List<T>();
        public int Count => heap.Count;
        
        public void Clear() => heap?.Clear();

        public T Contains(T t)
        {
            int idx = heap.IndexOf(t);
            if (idx < 0) return default(T);
            return heap[idx];
        }

        public void Push(T data)
        {
            heap.Add(data);
            HeapifyUp(heap.Count - 1);
        }

        // 배열(이진트리)에서 특정 원소의 부모를 구하려면 (n-1)/2
        // 부모에서 자식을 구하려면 n*2+1 (왼쪽자신) 
        private void HeapifyUp(int index) 
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[index].CompareTo(heap[parentIndex]) < 0)
                {
                    break;
                }
                (heap[index], heap[parentIndex]) = (heap[parentIndex], heap[index]);
                index = parentIndex;

            }
        }
        
        public void DecreaseKey(T item)
        {
            int index = heap.IndexOf(item);
            if (index < 0) return;
            
            HeapifyUp(index); // 해당 인덱스부터 위로 올리면서 검사한다
        }
        
        public T Pop()
        {
            T ret = heap[0]; // 제일 부모 (제일 작은) 원소를 꺼내고
            
            int lastIdx = heap.Count - 1;
            heap[0] = heap[lastIdx]; // 마지막 원소 앞으로 가져오고
            heap.RemoveAt(lastIdx); // 마지막은 제거
            lastIdx--;

            int now = 0;

            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now; // 다음에다 나를 넣어두고
                if (left <= lastIdx && heap[next].CompareTo(heap[left]) < 0)
                    next = left; // 왼쪽이 작다면 다음은 왼쪽
                if (right <= lastIdx && heap[next].CompareTo(heap[right]) < 0)
                    next = right; // 오른쪽이 작다면 다음은 오른쪽
                
                if (next == now)
                    break;
                
                (heap[now], heap[next]) = (heap[next], heap[now]); // 튜플 교환
                now = next;
            }
            
            return ret;
        }

        public T Peek() // 헬퍼함수 (젤 높은애가 누구인지 반환)
            => heap.Count == 0 ? default(T) : heap[0];
    }
}