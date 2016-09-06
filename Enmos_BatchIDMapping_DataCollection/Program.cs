using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LINQtoCSV;
using HadoopUtil;
using Dapper;
using System.Data.Odbc;

namespace Enmos_BatchIDMapping_DataCollection
{
    class Program
    {
        static string site01ConStr = ConfigurationManager.ConnectionStrings["ENMOS01"].ConnectionString;
        static string site02ConStr = ConfigurationManager.ConnectionStrings["ENMOS02"].ConnectionString;
        static void Main(string[] args)
        {
            List<BatchIDMapping> data = getDataFromDB("001").Union(getDataFromDB("002")).ToList();
            ConvertDataToCsv(data);
        }

        #region Sql Command

        static string getSqlCmd(string site)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(" SELECT '{0}' Site,BatchId, [Value] AS BatchNo, [Time] FROM Monitoring.BatchParams AS A ", site));
            sb.Append(" WHERE ([Index] = 0) AND ([Value] LIKE 'MD%') AND ([Time] BETWEEN '2016-07-14' AND GETDATE() - 1)");
            return sb.ToString();

        }

        #endregion

        #region Retrieve data from database

        static List<BatchIDMapping> getDataFromDB(string site)
        {
            string connStr = site == "001" ? site01ConStr : site02ConStr;
            var cn = new SqlConnection(connStr);
            cn.Open();
            string sqlcmd = getSqlCmd(site);
            return cn.Query<BatchIDMapping>(sqlcmd).ToList();
        }


        #endregion

        #region PutDataToHDFS

        static void PutDataToHDFS(string srcPath, string srcFileName, string trgtFilePath)
        {
            HdfsUtil data = new HdfsUtil(new Uri("http://master01:50070"), "root");
            if (data.IsFileExist(trgtFilePath, srcFileName))
                data.DeleteFile(string.Format("{0}/{1}", trgtFilePath, srcFileName));
            data.PutDataToHdfs(string.Format("{0}\\{1}", srcPath, srcFileName), string.Format("{0}/{1}", trgtFilePath, srcFileName));
            RefreshTblSchema();
        }


        #endregion

        #region Refresh Table Schema

        static void RefreshTblSchema()
        {
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Impala_ODBC"].ConnectionString;
            con.Open();
            string sqlcmd = "REFRESH enmos_batchid_mapping";
            OdbcCommand cmd = new OdbcCommand(sqlcmd, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion

        #region Convert Data To CSV and Put data to hdfs

        /// <summary>
        /// Convert Data To CSV
        /// </summary>
        /// <param name="data"></param>
        static void ConvertDataToCsv(List<BatchIDMapping> data)
        {
            CsvFileDescription outputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ','
                ,FirstLineHasColumnNames = false
                ,EnforceCsvColumnAttribute = true
            };
            CsvContext cc = new CsvContext();
            string filename = "Enmos_BatchId_Mapping.csv";
            cc.Write(data, string.Format("C:\\DataCollection\\{0}", filename), outputFileDescription);
            PutDataToHDFS("C:\\DataCollection", filename, "/DataWarehouse/ENMOS/Enmos_BatchId_Mapping");
        }

        #endregion





    }
}
