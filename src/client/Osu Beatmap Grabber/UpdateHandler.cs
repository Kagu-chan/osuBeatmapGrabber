using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using kcUpdater;
using kcUpdater.Enums;

using System.IO;
using kcUpdater.Structures;

namespace Osu_Beatmap_Grabber
{
    public delegate void LastMessageChanged(object sender, LastMessageEventArgs e);
    public delegate void UpdatingChanged(object sender, UpdatingEventArgs e);
    public delegate void ProgressChanged(object sender, ProgressEventArgs e);

    public class LastMessageEventArgs : EventArgs
    {
        private string _message;

        public LastMessageEventArgs(string message)
        {
            _message = message;
        }

        public string Message { get { return _message; } }
    }

    public class ProgressEventArgs : EventArgs
    {
        private double _percent;

        public ProgressEventArgs(double percent)
        {
            _percent = percent;
        }

        public double Percent { get { return _percent; } }
    }

    public class UpdatingEventArgs : EventArgs {}

    class UpdateHandler : kcUpdater.Handler.UpdaterBase
    {
        private string _lastMessage = string.Empty;
        public string LastMessage { get { return _lastMessage; } }

        private bool _updating = false;
        public bool Updating { get { return _updating; } }

        private double _percent = 0;
        public double Percent {  get { return _percent; } }

        public string UpdateDirectory = "";
        public string TargetDirectory = "";

        public event LastMessageChanged OnLastMessageChanged;
        public event UpdatingChanged OnUpdatingChanged;
        public event ProgressChanged OnProgressChanged;

        public HandlerMessageSeverity PrintSeverity = HandlerMessageSeverity.Trace;

        protected string LastFile;
        protected int CurrentIndex;
        protected int CurrentBytes;
        
        private void Update()
        {
            if (OnUpdatingChanged != null)
            {
                OnUpdatingChanged.Invoke(this, new UpdatingEventArgs());
            }
        }

        public UpdateHandler()
        {
            StartUpdateProcess = OnStartUpdateProcess;
            ExitUpdateProcess = OnExitUpdateProcess;

            kcUpdater.Classes.WebDownloader.Instance.BeginDownload = OnBeginDownload;
            kcUpdater.Classes.WebDownloader.Instance.UpdateDownload = OnUpdateDownload;
            kcUpdater.Classes.WebDownloader.Instance.EndDownload = OnEndDownload;
        }

        public override void DisplayMessage(HandlerMessageSeverity severity, string message)
        {
            _lastMessage = message;
            if (OnLastMessageChanged != null)
            {
                OnLastMessageChanged.Invoke(this, new LastMessageEventArgs(message));
            }
        }

        protected void OnStartUpdateProcess()
        {
            DisplayMessage(HandlerMessageSeverity.Trace, "Start update process");
            _updating = true;
            Update();
        }

        protected void OnExitUpdateProcess()
        {
            DisplayMessage(HandlerMessageSeverity.Trace, "Finnished update process");
            _updating = false;
            Update();
        }

        protected void UpdateProgress()
        {
            double progress = (double) (CurrentIndex - 1) / _lastResponse.Files.Count;
            double range = 1.0 / _lastResponse.Files.Count;

            double currentPerc = (double) kcUpdater.Classes.WebDownloader.Instance.CurrentBytes / CurrentBytes;
            double addition = range * currentPerc;

            _percent = progress + addition;
            if (OnProgressChanged != null) OnProgressChanged.Invoke(this, new ProgressEventArgs(_percent));
        }

        protected void OnBeginDownload(string file)
        {
            LastFile = Path.GetFileName(file);
            DisplayMessage(HandlerMessageSeverity.Trace, "Start downloading file - " + LastFile);

            CurrentIndex = kcUpdater.Classes.WebDownloader.Instance.FileNumber;
            CurrentBytes = kcUpdater.Classes.WebDownloader.Instance.CurrentDownloadLength;
            UpdateProgress();
        }

        protected void OnUpdateDownload()
        {
            DisplayMessage(HandlerMessageSeverity.Trace, "Download - " + LastFile);
            UpdateProgress();
        }

        protected void OnEndDownload()
        {
            DisplayMessage(HandlerMessageSeverity.Trace, "Finnished download - " + LastFile);
            UpdateProgress();
        }

        public override bool RehashUpdate(Configuration configuration, string configName)
        {
            foreach (string item in Directory.GetFiles(TargetDirectory))
            {
                if (item.Equals(configName)) continue;

                try
                {
                    File.Delete(item);
                } catch (Exception) { }
            }
            foreach (string item in Directory.GetDirectories(TargetDirectory))
            {
                string d = Path.GetFileName(item);
                if (configuration.ProtectedDirectories.Contains(d)) continue;
                if (item.Equals(configuration.UpdateDirectory)) continue;

                try
                {
                    Directory.Delete(item, true);
                }
                catch (Exception) { }
            }

            string[] files = Directory.GetFiles(UpdateDirectory);
            foreach (var item in files)
            {
                string newPath = Path.Combine(TargetDirectory, Path.GetFileName(item));
                try
                {
                    File.Copy(item, newPath);
                } catch (Exception) { }
            }
            try
            {
                Directory.Delete(UpdateDirectory, true);
            } catch (Exception) { }
            return true;
        }

        public override bool StopApplication(Configuration configuration)
        {
            if (ExitUpdateProcess != null) ExitUpdateProcess.Invoke();
            return true;
        }
        
    }
}
