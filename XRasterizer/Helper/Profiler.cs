using System.Diagnostics;

namespace XRasterizer.Helper
{
    public enum ProfilerElementType : byte
    {
        LoopTime = 0,
        RenderTime = LoopTime + 1,
        ClearTime = RenderTime + 1,
        VertexShaderTime = ClearTime + 1,
        RasterizeTime = VertexShaderTime + 1,
        FragmentShaderTime = RasterizeTime + 1,
        PrimitiveAssemblyTime = FragmentShaderTime + 1,
        TransformToCVVTime = PrimitiveAssemblyTime + 1,

        VertexCount = TransformToCVVTime + 1,
        Core1TaskCount = VertexCount + 1,
        Core2TaskCount = Core1TaskCount + 1,
        Core3TaskCount = Core2TaskCount + 1,
        Core4TaskCount = Core3TaskCount + 1,
        Core5TaskCount = Core4TaskCount + 1,
        Core6TaskCount = Core5TaskCount + 1,
        Core7TaskCount = Core6TaskCount + 1,
        Core8TaskCount = Core7TaskCount + 1,
        TaskCount = Core8TaskCount + 1,

        Count = TaskCount + 1,
    }

    public class ProfilerElement
    {
        public const int StatisticsCount = 60;

        private readonly ProfilerElementType ElementType_;
        private readonly Stopwatch Watcher_;
        private readonly double[] ElapsedTime_;
        private int Index_;
        private double AverageTime_;
        private int Count_;
        private int MaxCount_;
        
        public ProfilerElement(ProfilerElementType ElementType)
        {
            ElementType_ = ElementType;
            Watcher_ = new Stopwatch();
            ElapsedTime_ = new double[StatisticsCount];
            Index_ = 0;
            AverageTime_ = 0;
            Count_ = 0;
            MaxCount_ = 0;
        }

        public ProfilerElementType GetElementType()
        {
            return ElementType_;
        }

        public void Begin()
        {
            Watcher_.Restart();
        }

        public void End()
        {
            Watcher_.Stop();
            ElapsedTime_[Index_] += Watcher_.Elapsed.TotalMilliseconds;
        }

        public double GetAverageTime()
        {
            return AverageTime_;
        }

        public void Increase(int Count)
        {
            Count_ += Count;
        }

        public int GetCount()
        {
            return Count_;
        }

        public int GetMaxCount()
        {
            return MaxCount_;
        }

        public void Update()
        {
            for (var Index = 0; Index < StatisticsCount; ++Index)
            {
                AverageTime_ += ElapsedTime_[Index];
            }

            AverageTime_ /= (double)StatisticsCount;
            MaxCount_ = Count_ > MaxCount_ ? Count_ : MaxCount_;
            Count_ = 0;

            Index_++;
            if (Index_ >= StatisticsCount)
            {
                Index_ = 0;
            }
            ElapsedTime_[Index_] = 0;
        }
    }

    public class Profiler
    {
        public bool Enable { get; set; }
        
        private readonly ProfilerElement[] Elements_;

        public Profiler()
        {
            Enable = true;

            Elements_ = new ProfilerElement[(int)ProfilerElementType.Count];
            for (var Index = 0; Index < Elements_.Length; ++Index)
            {
                Elements_[Index] = new ProfilerElement((ProfilerElementType)Index);
            }
        }

        public void Begin(ProfilerElementType Type)
        {
            if (Enable)
            {
                Elements_[(int) Type].Begin();
            }
        }

        public void End(ProfilerElementType Type)
        {
            if (Enable)
            {
                Elements_[(int)Type].End();
            }
        }

        public double GetAverageTime(ProfilerElementType Type)
        {
            return Elements_[(int)Type].GetAverageTime();
        }

        public void Increase(ProfilerElementType Type, int Count = 1)
        {
            if (Enable)
            {
                Elements_[(int) Type].Increase(Count);
            }
        }

        public int GetCount(ProfilerElementType Type)
        {
            return Elements_[(int)Type].GetCount();
        }

        public int GetMaxCount(ProfilerElementType Type)
        {
            return Elements_[(int)Type].GetMaxCount();
        }

        public void Update()
        {
            foreach (var Element in Elements_)
            {
                Element.Update();
            }
        }
    }
}