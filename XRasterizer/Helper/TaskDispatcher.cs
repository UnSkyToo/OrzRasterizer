using System.Collections.Generic;
using System.Threading;

namespace XRasterizer.Helper
{
    public class TaskBase
    {
        public virtual void Execute()
        {
        }
    }

    internal class TaskThread
    {
        private readonly int Index_;
        private readonly TaskDispatcher Master_;
        private readonly ManualResetEvent RunEvent_;
        private readonly ManualResetEvent WaitEvent_;
        private Thread Thread_;
        private bool IsEnd_;

        public TaskThread(int Index, TaskDispatcher Master)
        {
            Index_ = Index;
            Master_ = Master;
            RunEvent_ = new ManualResetEvent(false);
            WaitEvent_ = new ManualResetEvent(false);
            IsEnd_ = false;
        }

        public void Start()
        {
            Thread_ = new Thread(Run);
            Thread_.Start();
        }

        public void Stop()
        {
            IsEnd_ = true;
            SetReady();
            Thread_.Join();
        }

        public void SetReady()
        {
            RunEvent_.Reset();
            WaitEvent_.Set();
        }

        public void Wait()
        {
            RunEvent_.WaitOne();
        }

        public void Run()
        {
            while (!IsEnd_)
            {
                WaitEvent_.WaitOne();
                WaitEvent_.Reset();

                if (IsEnd_)
                {
                    break;
                }

                while (true)
                {
                    var Task = Master_.GetTask(Index_);
                    if (Task == null)
                    {
                        break;
                    }

                    Master_.GetDevice().GetProfiler().Increase((ProfilerElementType)((int)ProfilerElementType.Core1TaskCount + Index_));
                    Task.Execute();
                }

                RunEvent_.Set();
            }
        }
    }

    public class TaskDispatcher
    {
        public bool Enable { get; set; }

        private readonly GraphicsDevice Device_;
        private readonly Stack<TaskBase>[] Tasks_;
        private readonly TaskThread[] TaskHandler_;

        public TaskDispatcher(GraphicsDevice Device, int CoreNum)
        {
            Enable = true;

            Device_ = Device;
            Tasks_ = new Stack<TaskBase>[CoreNum];
            for (var Index = 0; Index < CoreNum; ++Index)
            {
                Tasks_[Index] = new Stack<TaskBase>();
            }

            TaskHandler_ = new TaskThread[CoreNum];
        }

        public GraphicsDevice GetDevice()
        {
            return Device_;
        }

        public void Start()
        {
            if (!Enable)
            {
                return;
            }

            for (var Index = 0; Index < TaskHandler_.Length; ++Index)
            {
                TaskHandler_[Index] = new TaskThread(Index, this);
                TaskHandler_[Index].Start();
            }
        }

        public void Stop()
        {
            if (!Enable)
            {
                return;
            }

            foreach (var Handler in TaskHandler_)
            {
                Handler.Stop();
            }
        }

        public void PushTask(int Index, TaskBase Task)
        {
            Device_.GetProfiler().Increase(ProfilerElementType.TaskCount);
            Tasks_[Index].Push(Task);
        }

        public TaskBase GetTask(int Index)
        {
            TaskBase Task = null;
            if (Tasks_[Index].Count > 0)
            {
                Task = Tasks_[Index].Pop();
            }
            return Task;
        }
        
        public void Wait()
        {
            if (Enable)
            {
                foreach (var Handler in TaskHandler_)
                {
                    Handler.SetReady();
                }
                
                foreach (var Handler in TaskHandler_)
                {
                    Handler.Wait();
                }
            }
            else
            {
                for (var Index = 0; Index < Tasks_.Length; ++Index)
                {
                    while (true)
                    {
                        var Task = GetTask(Index);
                        if (Task == null)
                        {
                            break;
                        }

                        Task.Execute();
                    }
                }
            }
        }
    }
}