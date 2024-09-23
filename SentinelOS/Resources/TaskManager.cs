using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Resources
{
    public abstract class OSTask
    {
        public int PID { get; private set; }
        public string Name { get; private set; }
        public TaskState State { get; private set; }
        public int MemoryUsage { get; private set; }
        public int ThreadCount { get; private set; }
        public int ParentPID { get; private set; }

        public OSTask(string name, int pid, int parentPID)
        {
            Name = name;
            PID = pid;
            ParentPID = parentPID;
            State = TaskState.Running;
            MemoryUsage = 0;
            ThreadCount = 1;
        }

        public void UpdateState(TaskState newState)
        {
            State = newState;
        }

        public void UpdateResourceUsage(int memoryUsage, int threadCount)
        {
            MemoryUsage = memoryUsage;
            ThreadCount = threadCount;
        }

        public abstract void Execute();

        public override string ToString()
        {
            return $"PID: {PID} | Name: {Name} | State: {State} | Memory Usage: {MemoryUsage} | Thread Count: {ThreadCount} | Parent PID: {ParentPID}";
        }
    }

    public class OSTask<T> : OSTask
    {
        private Action<T> TaskAction { get; set; }
        private T TaskParameter { get; set; }

        public OSTask(string name, int pid, int parentPID, Action<T> taskAction, T taskParameter)
            : base(name, pid, parentPID)
        {
            TaskAction = taskAction;
            TaskParameter = taskParameter;
        }

        public override void Execute()
        {
            TaskAction?.Invoke(TaskParameter);
        }
    }

    public class OSTaskWithoutParams : OSTask
    {
        private Action TaskAction { get; set; }
        public OSTaskWithoutParams(string name, int pid, int parentPID, Action taskAction)
            : base(name, pid, parentPID)
        {
            TaskAction = taskAction;
        }

        public override void Execute()
        {
            TaskAction?.Invoke();
        }
    }

    public enum TaskState
    {
        Running,
        Stopped,
        Suspended
    }

    public enum TaskType
    {
        System,
        User
    }

    public static class TaskManager
    {
        private static List<OSTask> tasks = new List<OSTask>();
        private static int nextPID = 1;

        /// <summary>
        /// Add tasks to the task manager to be executed.
        /// This method is used when the task has no parameters
        /// <para>TaskManager.AddTask("TaskName", 0, () => { Console.WriteLine("Hello World!"); });</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentPID"></param>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        public static OSTask AddTask(string name, int parentPID, Action taskAction)
        {
            var task = new OSTaskWithoutParams(name, nextPID++, parentPID, taskAction);
            tasks.Add(task);
            return task;
        }
        /// <summary>
        /// Add tasks to the task manager to be executed.
        /// This method is used when the task has parameters
        /// <para>TaskManager.AddTask("TaskName", 0, (string message) => { Console.WriteLine(message); }, "Hello World!");</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="parentPID"></param>
        /// <param name="taskAction"></param>
        /// <param name="taskParameter"></param>
        /// <returns></returns>
        public static OSTask<T> AddTask<T>(string name, int parentPID, Action<T> taskAction, T taskParameter)
        {
            var task = new OSTask<T>(name, nextPID++, parentPID, taskAction, taskParameter);
            tasks.Add(task);
            return task;
        }

        public static bool RemoveTask(int pid)
        {
            var task = tasks.Find(t => t.PID == pid);
            if (task != null)
            {
                tasks.Remove(task);
                return true;
            }
            return false;
        }

        public static List<OSTask> ListTasks()
        {
            return new List<OSTask>(tasks);
        }

        public static bool UpdateTaskState(int pid, TaskState newState)
        {
            var task = tasks.Find(t => t.PID == pid);
            if (task != null)
            {
                task.UpdateState(newState);
                return true;
            }
            return false;
        }

        public static void UpdateTaskResourceUsage(int pid, int memoryUsage, int threadCount)
        {
            var task = tasks.Find(t => t.PID == pid);
            if (task != null)
            {
                task.UpdateResourceUsage(memoryUsage, threadCount);
            }
        }

        public static void UpdateAllResourceUsage()
        {
            foreach (var task in tasks)
            {
                // Simule a atualização do uso de recursos
                task.UpdateResourceUsage(new Random().Next(100, 1000), new Random().Next(1, 10));
            }
        }

        public static bool StopTask(int pid)
        {
            return UpdateTaskState(pid, TaskState.Stopped);
        }

        public static bool SuspendTask(int pid)
        {
            return UpdateTaskState(pid, TaskState.Suspended);
        }

        public static bool ResumeTask(int pid)
        {
            return UpdateTaskState(pid, TaskState.Running);
        }

        public static void ExecuteAllTasks()
        {
            foreach (var task in tasks)
            {
                if (task.State == TaskState.Running)
                {
                    task.Execute();
                }
            }
        }
    }

}