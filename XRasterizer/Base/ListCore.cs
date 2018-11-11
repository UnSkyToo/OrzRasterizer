using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace XRasterizer.Base
{
    public class ListCore<T>
    {
        private readonly List<T> MainList_;
        private readonly List<T>[] CoreList_;

        public int Count => MainList_.Count;

        public T this[int Index]
        {
            get => MainList_[Index];
            set => MainList_[Index] = value;
        }

        public ListCore(int CoreNum)
        {
            MainList_ = new List<T>();
            CoreList_ = new List<T>[CoreNum];
            for (var Index = 0; Index < CoreNum; ++Index)
            {
                CoreList_[Index] = new List<T>();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            foreach (var List in CoreList_)
            {
                List.Clear();
            }
            MainList_.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int CoreIndex, T Value)
        {
            CoreList_[CoreIndex].Add(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(int CoreIndex, IEnumerable<T> Value)
        {
            CoreList_[CoreIndex].AddRange(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T Value)
        {
            MainList_.Add(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<T> Value)
        {
            MainList_.AddRange(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FlushToMain()
        {
            MainList_.Clear();
            foreach (var List in CoreList_)
            {
                MainList_.AddRange(List);
                List.Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> GetMainList()
        {
            return MainList_;
        }
    }
}