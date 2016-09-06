using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Hadoop.WebHDFS;

namespace HadoopUtil
{
    public class HdfsUtil
    {
        private WebHDFSClient _con;

        public HdfsUtil(Uri uri,string usrName)
        {
            _con = new WebHDFSClient(uri, usrName);
        }

        public HdfsUtil()
        {
            _con = new WebHDFSClient(new Uri("http://master01:50070"), "root");
            if(_con==null)
                _con = new WebHDFSClient(new Uri("http://master02:50070"), "root");

        }

        /// <summary>
        /// Check file exist or not
        /// </summary>
        /// <param name="path">relative path</param>
        /// <param name="fileName">file name</param>
        /// <returns></returns>
        public bool IsFileExist(string path,string fileName)
        {
            bool isExist = false;
            var dirStatus = _con.GetDirectoryStatus(path).Result;
            IEnumerable<string> fileList = dirStatus.Files.Select(d => d.PathSuffix);
            foreach(string x in fileList)
            {
                if (x == fileName)
                    isExist = true; break;
            }
            return isExist;
        }

        /// <summary>
        /// Check Directory Exist or not
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="dirName">directory name</param>
        /// <returns></returns>
        public bool IsDirectoryExist(string path,string dirName)
        {
            bool isExist = false;
            var dirStatus = _con.GetDirectoryStatus(path).Result;
            IEnumerable<string> fileList = dirStatus.Directories.Select(d => d.PathSuffix);
            foreach (string x in fileList)
            {
                if (x == dirName)
                    isExist = true; break;
            }
            return isExist;

        }

        /// <summary>
        /// Put data into hdfs,if sucess return true,else return false
        /// </summary>
        /// <param name="srcPath">local file path</param>
        /// <param name="trgtPath">hdfs file path</param>
        /// <returns></returns>
        public bool PutDataToHdfs(string srcPath,string trgtPath)
        {
            string sucessPath = string.Empty;
            sucessPath = _con.CreateFile(srcPath, trgtPath).Result;
            return !string.IsNullOrEmpty(sucessPath);
        }
        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="filePath">filePath</param>
        /// <returns></returns>
        public bool DeleteFile(string filePath)
        {
            return _con.DeleteDirectory(filePath).Result;
        }
        /// <summary>
        /// Delete directory
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public bool DeleteDirectory(string dirPath)
        {
            return _con.DeleteDirectory(dirPath, true).Result;

        }
        /// <summary>
        /// Create directory.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public bool CreateDirectory(string dirPath)
        {
            return _con.CreateDirectory(dirPath).Result;
        }

    }
}
