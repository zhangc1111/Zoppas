namespace Zoppas.Model
{
    using System.Collections.ObjectModel;
    using System.Windows.Threading;
    using System;

    /// <summary>
    /// Async observable collection for wpf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private static Dispatcher _Dispatcher;

        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncObservableCollection() : base()
        {
            _Dispatcher = Dispatcher.CurrentDispatcher;
        }
        /// <summary>
        /// Async insert item
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
            if (!_Dispatcher.CheckAccess())
            {
                //_Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate { base.InsertItem(index, item); });
                _Dispatcher.Invoke(() => { base.InsertItem(index, item); });
            }
            else
            {
                base.InsertItem(index, item);
            }
        }
        /// <summary>
        /// Async move item
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (!_Dispatcher.CheckAccess())
            {
                //_Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate { base.MoveItem(oldIndex, newIndex); });
                _Dispatcher.Invoke(() => { base.MoveItem(oldIndex, newIndex); });
            }
            else
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }
        /// <summary>
        /// Async set item
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, T item)
        {
            if (!_Dispatcher.CheckAccess())
            {
                //_Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate { base.SetItem(index, item); });
                _Dispatcher.Invoke(() => { base.SetItem(index, item); });
            }
            else
            {
                base.SetItem(index, item);
            }
        }
        /// <summary>
        /// Async remove item
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            if (!_Dispatcher.CheckAccess())
            {
                //_Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate { base.RemoveItem(index); });
                _Dispatcher.Invoke(() => { base.RemoveItem(index); });
            }
            else
            {
                base.RemoveItem(index);
            }
        }
        /// <summary>
        /// Async clear items
        /// </summary>
        protected override void ClearItems()
        {
            if (!_Dispatcher.CheckAccess())
            {
                //_Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate { base.ClearItems(); });
                _Dispatcher.Invoke(() => { base.ClearItems(); });
            }
            else
            {
                base.ClearItems();
            }
        }
    }
}
