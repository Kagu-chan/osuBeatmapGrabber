using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using kcUpdater.Structures;
using System.IO;
using kcUpdater.Interfaces;

namespace kcUpdater.Handler
{
    /// <summary>
    /// abstract updatehandler base class
    /// </summary>
    public abstract class UpdaterBase : IUpdateHandler
    {
        
        protected APIResponse _lastResponse;

        public DStartUpdateProcess StartUpdateProcess;
        public DExitUpdateProcess ExitUpdateProcess;

        #region InterfaceMethods
        string IUpdateHandler.GetNewVersionNumber()
        {
            return _lastResponse.Version;
        }

        public abstract void DisplayMessage(Enums.HandlerMessageSeverity severity, string message);

        public virtual bool RetriveProjectInformations(Configuration configuration)
        {
            if (StartUpdateProcess != null) StartUpdateProcess.Invoke();
            DisplayMessage(Enums.HandlerMessageSeverity.Trace, "Get Project Information");

            _lastResponse = Classes.JsonObject.Instance.ReadWebObject<APIResponse>(configuration.Path(Enums.PathType.Project));
            if (_lastResponse.Equals(null) || !_lastResponse.Success)
            {
                DisplayMessage(Enums.HandlerMessageSeverity.Warning, "Failed to load Project Informations");
                if (ExitUpdateProcess != null) ExitUpdateProcess.Invoke();
                return false;
            }
            return true;
        }

        public virtual bool IsUpdateAvailable(Configuration configuration)
        {
            DisplayMessage(Enums.HandlerMessageSeverity.Trace, "Check for Updates");

            bool available = configuration.CurrentVersion != _lastResponse.Version;

            return available;
        }

        public virtual bool IsUpdateRequired(Configuration configuration, bool forceUpdate)
        {
            if (!configuration.AutoUpdate && !forceUpdate)
            {
                DisplayMessage(Enums.HandlerMessageSeverity.Trace, "Automatic updates are disabled");
                if (ExitUpdateProcess != null) ExitUpdateProcess.Invoke();
                return false;
            }

            bool updateRequired = IsUpdateAvailable(configuration);

            DisplayMessage(Enums.HandlerMessageSeverity.Trace, updateRequired ? "Updates available" : "No Updates found");

            if (!updateRequired && ExitUpdateProcess != null) ExitUpdateProcess.Invoke();
            return updateRequired;
        }

        public virtual bool FetchFileList(Configuration configuration)
        {
            DisplayMessage(Enums.HandlerMessageSeverity.Trace, "Retrive versions file list");

            _lastResponse = Classes.JsonObject.Instance.ReadWebObject<APIResponse>(configuration.Path(Enums.PathType.Version, _lastResponse.Version));
            if (_lastResponse.Equals(null) || !_lastResponse.Success)
            {
                DisplayMessage(Enums.HandlerMessageSeverity.Error, "Failed to load file list from server");
                if (ExitUpdateProcess != null) ExitUpdateProcess.Invoke();
                return false;
            }
            return true;
        }

        public virtual bool RehashDownload(Configuration configuration)
        {
            if (Directory.Exists(configuration.UpdateDirectory)) Directory.Delete(configuration.UpdateDirectory, true);
            Directory.CreateDirectory(configuration.UpdateDirectory);
            return true;
        }

        public virtual bool DownloadUpdate(Configuration configuration)
        {
            foreach (KeyValuePair<string, string> item in _lastResponse.Files)
            {
                Uri source = configuration.Path(Enums.PathType.File, _lastResponse.Version, item.Key);
                Classes.WebDownloader.Instance.DownloadFile(source, Path.Combine(configuration.UpdateDirectory, item.Value));
            }
            
            Console.WriteLine(_lastResponse.Files.Count + " files insgesamt, aktuell bei " + Classes.WebDownloader.Instance.FileNumber);
            while (_lastResponse.Files.Count != Classes.WebDownloader.Instance.FileNumber && Classes.WebDownloader.Instance.Downloading) { }
            return true;
        }

        public abstract bool RehashUpdate(Configuration configuration, string configName);

        public abstract bool StopApplication(Configuration configuration);
        #endregion

        #region Delegates
        public delegate void DStartUpdateProcess();
        public delegate void DExitUpdateProcess();
        #endregion

    }
}
