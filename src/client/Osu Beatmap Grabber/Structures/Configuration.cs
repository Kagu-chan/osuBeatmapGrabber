using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace kcUpdater.Structures
{
    /// <summary>
    /// represents the updater configuration itself
    /// </summary>
    public struct Configuration
    {
        /// <summary>
        /// API domain
        /// </summary>
        public Uri Domain;

        /// <summary>
        /// project name
        /// </summary>
        public string Project;

        /// <summary>
        /// Indicates weather the project should update automatically or not
        /// </summary>
        public bool AutoUpdate;

        /// <summary>
        /// the currently installed version
        /// </summary>
        public string CurrentVersion;

        /// <summary>
        /// list of directories the updater should not remove while updating
        /// </summary>
        public IEnumerable<string> ProtectedDirectories;

        /// <summary>
        /// name of the temporary update directory
        /// </summary>
        public string UpdateDirectory;

        /// <summary>
        /// generates the according API uri path for given type of request
        /// </summary>
        /// <param name="t">type of request</param>
        /// <param name="values">arguments</param>
        /// <returns></returns>
        public Uri Path(Enums.PathType t, params string[] values)
        {
            UriBuilder builder = new UriBuilder(Domain);
            StringBuilder query = new StringBuilder("p=" + Project, 3);

            if (t == Enums.PathType.Version || t == Enums.PathType.File) query.Append("&d=" + values[0]);
            if (t == Enums.PathType.File) query.Append("&f=" + values[1]);

            builder.Query = query.ToString();
            return builder.Uri;
        }

        /// <summary>
        /// create the protected directories if not exists
        /// </summary>
        public void CreateProtectedDirectories()
        {
            foreach (string item in ProtectedDirectories)
            {
                if (!Directory.Exists(item)) Directory.CreateDirectory(item);
            }
        }
    }
}
