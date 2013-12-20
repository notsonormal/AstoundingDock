using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CtrlSoft.Win.UI;

namespace QuickZip.IO
{
    public class ShellProgressDialog : ICustomProgressDialog
    {
        WinProgressDialog _progressDialog;
        public ShellProgressDialog()
        {            
            _progressDialog = new WinProgressDialog(IntPtr.Zero);
        }

        #region ICustomProgressDialog Members
        
        public string Title
        {
            get
            {
                return _progressDialog.Title;
            }
            set
            {
                _progressDialog.Title = value;
            }
        }

        private string header = "";
        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                _progressDialog.SetLine(WinProgressDialog.IPD_Lines.LineOne, value, false);
                header = value;
            }
        }

        private string message = "";
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                _progressDialog.SetLine(WinProgressDialog.IPD_Lines.LineTwo, value, false);
                message = value;
            }
        }

        private string subMessage = "";
        public string SubMessage
        {
            get
            {
                return subMessage;
            }
            set
            {
                _progressDialog.SetLine(WinProgressDialog.IPD_Lines.LineThree, value, false);
                subMessage = value;
            }
        }

        public string Source
        {
            get
            {
                return "";
            }
            set
            {                
            }
        }

        public string Target
        {
            get
            {
                return "";
            }
            set
            {                
            }
        }

        public int TotalItems
        {
            get
            {
                return (int)_progressDialog.Total;
            }
            set
            {
                _progressDialog.Total = (uint)Math.Abs(value);
            }
        }

        public int ItemsCompleted
        {
            get
            {
                return (int)_progressDialog.Complete;
            }
            set
            {
                _progressDialog.Complete = (uint)Math.Abs(value);
            }
        }

        public bool IsCancelEnabled
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }

        public bool IsRestartEnabled
        {
            get
            {
                return false;
            }
            set
            {                
            }
        }

        public bool IsPauseEnabled
        {
            get
            {
                return false;
            }
            set
            {                
            }
        }

        public bool IsResumeEnabled
        {
            get
            {
                return false;
            }
            set
            {                
            }
        }

        private bool _isCanceled;
        public bool IsCanceled
        {
            get { return _isCanceled; }
        }

        private bool _isPaused;
        public bool IsPaused
        {
            get { return _isPaused; }
        }

        public event CancelClickedHandler OnCanceled;

        public event CancelClickedHandler OnPaused;

        public void ShowWindow()
        {
            //_progressDialog.sho
        }

        public void CloseWindow()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
