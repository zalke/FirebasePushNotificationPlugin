using Android.Gms.Tasks;
using System;
using System.Threading.Tasks;

namespace Plugin.FirebasePushNotification
{
    public static class TaskExtensions
    {
        public static Task<Java.Lang.Object> ToAwaitableTask(this Android.Gms.Tasks.Task task)
        {
            var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();
            var taskCompleteListener = new TaskCompleteListener(taskCompletionSource);
            task.AddOnCompleteListener(taskCompleteListener);

            return taskCompletionSource.Task;
        }

        private class TaskCompleteListener : Java.Lang.Object, IOnCompleteListener
        {
            private readonly TaskCompletionSource<Java.Lang.Object> taskCompletionSource;

            public TaskCompleteListener(TaskCompletionSource<Java.Lang.Object> taskCompletionSource)
            {
                this.taskCompletionSource = taskCompletionSource;
            }

            public void OnComplete(Android.Gms.Tasks.Task task)
            {
                if (task.IsCanceled)
                {
                    taskCompletionSource.SetCanceled();
                }
                else if (task.IsSuccessful)
                {
                    taskCompletionSource.SetResult(task.Result);
                }
                else
                {
                    taskCompletionSource.SetException(task.Exception);
                }
            }
        }


        public static Task<bool> ToAwaitableTaskVoid(this Android.Gms.Tasks.Task task)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var taskCompleteListener = new TaskCompleteListenerVoid(taskCompletionSource);
            task.AddOnCompleteListener(taskCompleteListener);

            return taskCompletionSource.Task;
        }

        private class TaskCompleteListenerVoid : Java.Lang.Object, IOnCompleteListener
        {
            private readonly TaskCompletionSource<bool> taskCompletionSource;

            public TaskCompleteListenerVoid(TaskCompletionSource<bool> taskCompletionSource)
            {
                this.taskCompletionSource = taskCompletionSource;
            }

            public void OnComplete(Android.Gms.Tasks.Task task)
            {
                if (task.IsCanceled)
                {
                    taskCompletionSource.SetCanceled();
                }
                else if (task.IsSuccessful)
                {
                    taskCompletionSource.SetResult(true);
                }
                else
                {
                    taskCompletionSource.SetException(task.Exception);
                }
            }
        }
    }
}
