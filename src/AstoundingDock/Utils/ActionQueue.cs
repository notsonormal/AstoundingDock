using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics.Contracts;

namespace AstoundingApplications.AstoundingDock.Utils
{
    public delegate void ActionQueueCallback();

    public class ActionQueueItem
    {
        public event EventHandler Completed = delegate { };                
        readonly object _parameter;
        readonly Action<object> _action;
        readonly Action<object, ActionQueueCallback> _actionWithCallback;

        public string Name
        {
            get
            {
                if (_action != null)
                    return _action.Method.Name;
                if (_actionWithCallback != null)
                    return _actionWithCallback.Method.Name;
                return "";
            }
        }

        public ActionQueueItem(Action<object> action, object parameter)
        {
            _action = action;
            _parameter = parameter;
            _actionWithCallback = null;
        }

        public ActionQueueItem(Action<object, ActionQueueCallback> action, object parameter)
        {
            _action = null;
            _parameter = parameter;
            _actionWithCallback = action;
        }

        public void Run()
        {
            if (_action != null)
            {
                _action(_parameter);
                Complete();
            }
            else if (_actionWithCallback != null)
            {
                _actionWithCallback(_parameter, Complete);
            }
            else
            {
                throw new ArgumentNullException("action||actionWithCallback");
            }
        }

        void Complete()
        {
            Completed(this, EventArgs.Empty);
        }
    }

    public class ActionQueue
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        bool _isBusy;
        readonly object _lockObj = new object();
        Queue<ActionQueueItem> _queue;

        public ActionQueue()
        {
            _queue = new Queue<ActionQueueItem>();
        }

        public bool IsBusy
        {
            get
            {
                lock (_lockObj)
                {
                    return _isBusy;
                }
            }
            set
            {
                lock (_lockObj)
                {
                    _isBusy = value;
                }
            }
        }

        public void QueueAction(Action<object> action)
        {
            QueueAction(action, null);
        }

        public void QueueAction(Action<object> action, object parameter)
        {
            QueueAction(new ActionQueueItem(action, parameter));
        }

        public void QueueAction(Action<object, ActionQueueCallback> action)
        {
            QueueAction(action, null);
        }

        public void QueueAction(Action<object, ActionQueueCallback> action, object parameter)
        {
            QueueAction(new ActionQueueItem(action, parameter));
        }

        public void QueueAction(ActionQueueItem action)
        {
            if (Log.IsDebugEnabled)
                Log.DebugFormat("QueueAction {0}", action.Name);

            _queue.Enqueue(action);

            if (!IsBusy)
            {
                RunNextAction();
            }
        }

        void RunNextAction()
        {          
            if (_queue.Count > 0)
            {
                IsBusy = true;

                ActionQueueItem action = _queue.Dequeue();
                if (Log.IsDebugEnabled)
                    Log.DebugFormat("RunNextAction {0}", action.Name);

                action.Completed += OnActionCompleted;
                action.Run();
            }
        }

        void OnActionCompleted(object sender, EventArgs e)
        {           
            ActionQueueItem action = (ActionQueueItem)sender;
            action.Completed -= OnActionCompleted;
            
            if (Log.IsDebugEnabled)
                Log.DebugFormat("OnActionCompleted {0}", action.Name);

            IsBusy = false;
            RunNextAction();
        }
    }
}
