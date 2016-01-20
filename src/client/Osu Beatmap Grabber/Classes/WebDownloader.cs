using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace kcUpdater.Classes
{
    public class WebDownloader
    {
        private static readonly Lazy<WebDownloader> lazy = new Lazy<WebDownloader>(() => new WebDownloader());

        /// <summary>
        /// Returns the Instance of this class
        /// </summary>
        public static WebDownloader Instance { get { return lazy.Value; } }

        public bool Downloading;
        public int FileNumber = 0;
        public int CurrentBytes;
        public int CurrentDownloadLength;

        public DBeginDownload BeginDownload;
        public DUpdateDownload UpdateDownload;
        public DEndDownload EndDownload;

        private string _target;
        private HttpWebRequest _request;
        private HttpWebResponse _response;
        private byte[] _dataBuffer;
        private FileStream _fileStream;

        /// <summary>
        /// Make the constructor private!
        /// </summary>
        private WebDownloader()
        {
            Downloading = false;
        }

        public void DownloadFile(Uri source, string target)
        {
            while (Downloading) { } //NOP - wait for current download to finnish

            string dir = Path.GetDirectoryName(target);
            while (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

            Downloading = true;

            _target = target;
            FileNumber++;
            CurrentBytes = 0;

            _request = (HttpWebRequest)HttpWebRequest.Create(source);
            _request.BeginGetResponse(new AsyncCallback(ResponseReceived), Instance);
        }

        private void ResponseReceived(IAsyncResult async)
        {
            try
            {
                _response = (HttpWebResponse)_request.EndGetResponse(async);
            } catch (Exception ex)
            {
                throw ex; // TODO: Find out which exception throws here - literaly a WebException - catch and throw own exception
            }

            CurrentDownloadLength = (int)_response.ContentLength;
            if (BeginDownload != null) BeginDownload.Invoke(_target);

            Array.Resize(ref _dataBuffer, CurrentDownloadLength);

            _fileStream = new FileStream(_target, FileMode.Create);
            _response.GetResponseStream().BeginRead(_dataBuffer, 0, CurrentDownloadLength, new AsyncCallback(OnDataRead), Instance);
        }

        private void OnDataRead(IAsyncResult async)
        {
            int nBytes = _response.GetResponseStream().EndRead(async);

            _fileStream.Write(_dataBuffer, 0, nBytes);
            if (nBytes > 0)
            {
                CurrentBytes += nBytes;
                if (UpdateDownload != null) UpdateDownload.Invoke();

                _response.GetResponseStream().BeginRead(_dataBuffer, 0, CurrentDownloadLength, new AsyncCallback(OnDataRead), Instance);
            } else
            {
                _fileStream.Close();
                _fileStream.Dispose();
                if (EndDownload != null) EndDownload.Invoke();
                Downloading = false;
            }
        }

        public delegate void DBeginDownload(string targetFile);
        public delegate void DUpdateDownload();
        public delegate void DEndDownload();
    }
}
