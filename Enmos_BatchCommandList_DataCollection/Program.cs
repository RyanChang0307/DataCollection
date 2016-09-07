using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LINQtoCSV;
using HadoopUtil;
using System.Data.Odbc;

namespace Enmos_BatchCommandList_DataCollection
{
    class Program
    {
        static string site01ConStr = ConfigurationManager.ConnectionStrings["ENMOS01"].ConnectionString;
        static string site02ConStr = ConfigurationManager.ConnectionStrings["ENMOS02"].ConnectionString;
        static void Main(string[] args)
        {
            string date = args.Length == 0 ? string.Format("{0:yyyy-MM-dd}", DateTime.Now) : string.Format("{0:yyyy-MM-dd}", args[0].ToString());
            string month = date.Substring(0, 7);
            List<BatchCommandList> list = getDataFromDB("001", month).Union(getDataFromDB("002", month)).ToList();
            ConvertDataToCsv(list, string.Format("{0}{1}", month.Substring(0, 4), month.Substring(5, 2)));
        }


        #region Sql Command

        static string getSqlCmd(string site,string month)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(" SELECT '{0}' Site,C.BatchId,B.DICId,C.StepNumber, B.Desc1, A.StartTime, A.EndTime ", site));
            sb.Append("FROM Monitoring.BatchCommandList AS A LEFT OUTER JOIN DIC.CommandsList AS B ON A.CommandId = B.Id");
            sb.Append(" LEFT OUTER JOIN  Monitoring.BatchProgram AS C ON A.BatchProgramId = C.Id");
            sb.Append(string.Format(" WHERE substring(Convert(char(30),A.StartTime,120),1,7)='{0}'", month));
            return sb.ToString();

        }

        #endregion

        #region Retrieve data from database

        static List<BatchCommandList> getDataFromDB(string site,string month)
        {
            string connStr = site == "001" ? site01ConStr : site02ConStr;
            var cn = new SqlConnection(connStr);
            cn.Open();
            string sqlcmd = getSqlCmd(site, month);
            return cn.Query<BatchCommandList>(sqlcmd).ToList();
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

        #region Convert Data To CSV and Put data to hdfs

        /// <summary>
        /// Convert Data To CSV
        /// </summary>
        /// <param name="data"></param>
        static void ConvertDataToCsv(List<BatchCommandList> data,string month)
        {
            CsvFileDescription outputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ','
                ,FirstLineHasColumnNames = false
                ,EnforceCsvColumnAttribute = true
            };
            CsvContext cc = new CsvContext();
            string filename =string.Format("Enmos_Batch_CommandList_{0}.csv",month);
            cc.Write(data, string.Format("C:\\DataCollection\\{0}", filename), outputFileDescription);
            PutDataToHDFS("C:\\DataCollection", filename, "/DataWarehouse/ENMOS/Enmos_Batch_CommandList");
        }

        #endregion

        #region Refresh Table Schema

        static void RefreshTblSchema()
        {
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Impala_ODBC"].ConnectionString;
            con.Open();
            string sqlcmd = "REFRESH Enmos_Batch_CommandList";
            OdbcCommand cmd = new OdbcCommand(sqlcmd, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion


    }
}
