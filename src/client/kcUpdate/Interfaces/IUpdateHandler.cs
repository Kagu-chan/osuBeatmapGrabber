using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kcUpdater.Interfaces
{
    /// <summary>
    /// generic interface for updatehandler classes
    /// </summary>
    public interface IUpdateHandler
    {
        /// <summary>
        /// get the newest version number
        /// </summary>
        /// <returns></returns>
        string GetNewVersionNumber();

        /// <summary>
        /// display a message about the update
        /// </summary>
        /// <param name="severity">how important is the message?</param>
        /// <param name="message">message</param>
        void DisplayMessage(Enums.HandlerMessageSeverity severity, string message);

        /// <summary>
        /// retrive information about the project from API server
        /// </summary>
        /// <param name="configuration">the configuration object</param>
        /// <returns></returns>
        bool RetriveProjectInformations(Structures.Configuration configuration);

        /// <summary>
        /// check if an update is available
        /// </summary>
        /// <param name="configuration">the configuration object</param>
        /// <returns></returns>
        bool IsUpdateAvailable(Structures.Configuration configuration);

        /// <summary>
        /// check if an update is required
        /// </summary>
        /// <param name="configuration">the configuration object</param>
        /// <param name="forceUpdate">indicates weather the AutoUpdate value should get ignored or not</param>
        /// <returns></returns>
        bool IsUpdateRequired(Structures.Configuration configuration, bool forceUpdate);
        
        /// <summary>
        /// fetch the list of new files from API server
        /// </summary>
        /// <param name="configuration">the configuration object</param>
        /// <returns></returns>
        bool FetchFileList(Structures.Configuration configuration);

        /// <summary>
        /// prepare the download of files
        /// </summary>
        /// <param name="configuration">the configuration object</param>
        /// <returns></returns>
        bool RehashDownload(Structures.Configuration configuration);

        /// <summary>
        /// download the files from the update
        /// </summary>
        /// <param name="configuration">the configuration object</param>
        /// <returns></returns>
        bool DownloadUpdate(Structures.Configuration configuration);

        /// <summary>
        /// rehash the update installation
        /// </summary>
        /// <param name="configuration">the configuration object</param>
        /// <param name="configName">filename of configuration file</param>
        /// <returns></returns>
        bool RehashUpdate(Structures.Configuration configuration, string configName);

        /// <summary>
        /// stops the application for the updating process
        /// </summary>
        /// <param name="configuration">the configuration object</param>
        /// <returns></returns>
        bool StopApplication(Structures.Configuration configuration);
    }
}
