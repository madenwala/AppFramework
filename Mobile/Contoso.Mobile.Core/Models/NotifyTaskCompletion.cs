﻿using Contoso.Mobile.Core.Models;
using Contoso.Mobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.Mobile.Core.Models
{
    public interface INotifyTaskCompletion
    {
        bool IsCompleted { get; }
        bool IsNotCompleted { get; }
        bool IsSuccessfullyCompleted { get; }
        bool IsCanceled { get; }
        bool IsFaulted { get; }
        bool HasResult { get; }
        void Refresh(bool forceRefresh, CancellationToken ct);
    }

    public sealed class NotifyTaskCompletionList : ObservableCollection<INotifyTaskCompletion>
    {
        public NotifyTaskCompletion<TResult> Add<TResult>(Func<CancellationToken, Task<TResult>> func)
        {
            var ntc = new NotifyTaskCompletion<TResult>(func);
            this.Add(ntc);
            return ntc;
        }
        
        public void Refresh(bool forceRefresh, CancellationToken ct)
        {
            foreach (var ntc in this)
                ntc.Refresh(forceRefresh, ct);
        }
    }

    /// <summary>
    /// Task wrapper for async operations that are UI bindable. See https://msdn.microsoft.com/en-us/magazine/dn605875.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class NotifyTaskCompletion<TResult> : BaseModel, IDisposable, INotifyTaskCompletion
    {
        #region Variables

        private TResult _result = default(TResult);
        //private bool _loadedFromCache = false;

        #endregion

        #region Events

        public event EventHandler<TResult> SuccessfullyCompleted;

        #endregion

        #region Properties

        private string Key { get; set; }
        private BaseViewModel VM { get; set; }
        private Func<CancellationToken, Task<TResult>> FuncTask { get; set; }

        private Task<TResult> _Task;
        public Task<TResult> Task
        {
            get { return _Task; }
            private set
            {
                if (this.SetProperty(ref _Task, value))
                {
                    this.NotifyPropertyChanged(() => this.Status);
                    this.NotifyPropertyChanged(() => this.IsCompleted);
                    this.NotifyPropertyChanged(() => this.IsNotCompleted);
                    this.NotifyPropertyChanged(() => this.IsSuccessfullyCompleted);
                    this.NotifyPropertyChanged(() => this.IsCanceled);
                    this.NotifyPropertyChanged(() => this.IsFaulted);
                    this.NotifyPropertyChanged(() => this.Exception);
                    this.NotifyPropertyChanged(() => this.InnerException);
                    this.NotifyPropertyChanged(() => this.ErrorMessage);
                    this.NotifyPropertyChanged(() => this.IsResultNull);
                    this.NotifyPropertyChanged(() => this.Result);
                }
            }
        }

        public TResult Result
        {
            get
            {
                return (this.Task?.Status == TaskStatus.RanToCompletion) ? this.Task.Result : _result;
            }
        }
        public bool HasResult { get { return this.Result != null; } }
        public TaskStatus Status { get { return this.Task?.Status ?? TaskStatus.WaitingForActivation; } }
        public bool IsCompleted { get { return this.Task?.IsCompleted ?? false; } }
        public bool IsNotCompleted { get { return !this.Task?.IsCompleted ?? true; } }
        public bool IsSuccessfullyCompleted { get { return this.Task?.Status == TaskStatus.RanToCompletion; } }
        public bool IsCanceled { get { return this.Task?.IsCanceled ?? false; } }
        public bool IsFaulted { get { return this.Task?.IsFaulted ?? false; } }
        public AggregateException Exception { get { return this.Task?.Exception; } }
        public Exception InnerException { get { return this.Exception?.InnerException; } }
        public string ErrorMessage { get { return this.InnerException?.Message; } }
        public bool IsResultNull { get { return this.Result == null; } }

        #endregion

        #region Constructors

        public NotifyTaskCompletion(Func<CancellationToken, Task<TResult>> task) : this(task, null, null)
        {
        }


        // TODO make public
        private NotifyTaskCompletion(Func<CancellationToken, Task<TResult>> funcTask, BaseViewModel vm, string key)
        {
            if (funcTask == null)
                throw new NullReferenceException(nameof(funcTask));

            this.FuncTask = funcTask;
            this.VM = vm;
            this.Key = key;
        }

        #endregion

        #region Methods

        public void Refresh(bool forceRefresh = true)
        {
            this.Refresh(forceRefresh, CancellationToken.None);
        }

        public void Refresh(bool forceRefresh, CancellationToken ct)
        {
            var _ = this.RefreshAsync(forceRefresh, ct);
        }

        public async Task RefreshAsync(bool forceRefresh, CancellationToken ct)
        {
            //if (!_loadedFromCache && this.VM != null && !string.IsNullOrEmpty(this.Key))
            //{
            //    try
            //    {
            //        _loadedFromCache = true;
            //        _result = await this.VM.LoadFromCacheAsync<TResult>(this.Key);
            //        if (_result != null)
            //        {
            //            this.NotifyPropertyChanged(() => this.Result);
            //            this.NotifyPropertyChanged(() => this.HasResult);
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}

            if (forceRefresh || this.Task == null || this.Task.IsCanceled || this.Task.IsFaulted)
                this.Task = this.FuncTask(ct);

            if (!this.Task.IsCompleted)
                await this.WatchTaskAsync(this.Task);
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }

            this.NotifyPropertyChanged(() => this.Status);
            this.NotifyPropertyChanged(() => this.IsCompleted);
            this.NotifyPropertyChanged(() => this.IsNotCompleted);
            this.NotifyPropertyChanged(() => this.HasResult);

            if (task.IsCanceled)
            {
                this.NotifyPropertyChanged(() => this.IsCanceled);
            }
            else if (task.IsFaulted)
            {
                this.NotifyPropertyChanged(() => this.IsFaulted);
                this.NotifyPropertyChanged(() => this.Exception);
                this.NotifyPropertyChanged(() => this.InnerException);
                this.NotifyPropertyChanged(() => this.ErrorMessage);
            }
            else
            {
                this.NotifyPropertyChanged(() => this.IsSuccessfullyCompleted);
                this.NotifyPropertyChanged(() => this.IsResultNull);
                this.NotifyPropertyChanged(() => this.Result);
                this.SuccessfullyCompleted?.Invoke(this, this.Result);
                //if (this.VM != null && this.Key != null)
                //{
                //    _result = this.Task.Result;
                //    await this.VM.SaveToCacheAsync<TResult>(this.Key, this.Result);
                //}
            }
        }

        public void Dispose()
        {
            this.Key = null;
            this.VM = null;
        }

        #endregion
    }
}