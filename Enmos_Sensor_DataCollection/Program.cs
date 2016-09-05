using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using LINQtoCSV;
using HadoopUtil;
using System.Data.Odbc;

namespace Enmos_Sensor_DataCollection
{
    class Program
    {
        static string site01ConStr = ConfigurationManager.ConnectionStrings["ENMOS01"].ConnectionString;
        static string site02ConStr = ConfigurationManager.ConnectionStrings["ENMOS02"].ConnectionString;

        static void Main(string[] args)
        {
            string date = args.Length == 0 ? string.Format("{0:yyyy/MM/dd}", DateTime.Now.AddDays(-1)) : string.Format("{0:yyyy/MM/dd}", args[0].ToString());
            List<Enmos_Sensor_Value> data = getDataFromDB("001", "AI", date);
            var list = data.Union(getDataFromDB("002", "AI", date)).Union(getDataFromDB("001", "AO", date))
                .Union(getDataFromDB("002", "AO", date)).Union(getDataFromDB("001", "DI", date))
                .Union(getDataFromDB("002", "DI", date)).Union(getDataFromDB("001", "DO", date))
                .Union(getDataFromDB("002", "DO", date));
            ConvertDataToCsv(list.ToList(), string.Format("{0:yyyyMMdd}", Convert.ToDateTime(date)));

        }

        #region Convert Data To CSV and Put data to hdfs

        /// <summary>
        /// Convert Data To CSV
        /// </summary>
        /// <param name="data"></param>
        static void ConvertDataToCsv(List<Enmos_Sensor_Value> data, string date)
        {
            CsvFileDescription outputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ','
                ,FirstLineHasColumnNames = false
                ,EnforceCsvColumnAttribute = true
            };
            CsvContext cc = new CsvContext();
            string filename = string.Format("Enmos_Sensor_Value_{0}.csv", date);
            cc.Write(data, string.Format("C:\\DataCollection\\{0}", filename), outputFileDescription);

            PutDataToHDFS("C:\\DataCollection", filename, "/DataWarehouse/ENMOS/Enmos_Sensor_Value", date);
        }

        #endregion

        #region PutDataToHDFS

        static void PutDataToHDFS(string srcPath, string srcFileName, string trgtFilePath, string date)
        {
            HdfsUtil data = new HdfsUtil(new Uri("http://master02:50070"), "root");
            string path = string.Format("{0}/month={1}", trgtFilePath, date.Substring(0, 6));
            if (!data.IsDirectoryExist(trgtFilePath, string.Format("month={0}", date.Substring(0, 6))))
                data.CreateDirectory(path);
            if (data.IsFileExist(path, string.Format("{0}/{1}", path, srcFileName)))
                data.DeleteFile(string.Format("{0}/{1}", path, srcFileName));
            if (date.Substring(6, 2) == "01")
                UpdateTblSchema(date);
            data.PutDataToHdfs(string.Format("{0}\\{1}", srcPath, srcFileName), string.Format("{0}/{1}", path, srcFileName));
            RefreshTblSchema();
        }


        #endregion

        #region SQL Command
        /// <summary>
        /// get SQL Command
        /// </summary>
        /// <param name="site"></param>
        /// <param name="sensorType"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        static string getSQLCmd(string site, string sensorType, string date)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("SELECT '{0}' SITE,A.DICId, '{1}' AS SRCTYPE, A.[Index], B.Desc1, A.[Value], A.[Time]", site, sensorType));
            switch (sensorType)
            {
                case "AI":
                    sb.Append(" FROM Monitoring.IOAIValues AS A LEFT OUTER JOIN DIC.IOsList AS B ON A.DICId = B.DICId AND A.[Index] = B.[Index] WHERE  (B.TypeId = 0)");
                    break;
                case "AO":
                    sb.Append(" FROM Monitoring.IOAOValues AS A LEFT OUTER JOIN DIC.IOsList AS B ON A.DICId = B.DICId AND A.[Index] = B.[Index] WHERE  (B.TypeId = 1)");
                    break;
                case "DI":
                    sb.Append(" FROM Monitoring.IODIValues AS A LEFT OUTER JOIN DIC.IOsList AS B ON A.DICId = B.DICId AND A.[Index] = B.[Index] WHERE  (B.TypeId = 2)");
                    break;
                case "DO":
                    sb.Append(" FROM Monitoring.IODOValues AS A LEFT OUTER JOIN DIC.IOsList AS B ON A.DICId = B.DICId AND A.[Index] = B.[Index] WHERE  (B.TypeId = 3)");
                    break;

            }
            sb.Append(string.Format(" AND CONVERT(CHAR(10),A.Time,111) ='{0}' ", date));
            return sb.ToString();
        }


        #endregion

        #region Get Data From RDBMS
        /// <summary>
        /// get data from RDBMS
        /// </summary>
        /// <param name="site"></param>
        /// <param name="sensorType"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        static List<Enmos_Sensor_Value> getDataFromDB(string site, string sensorType, string date)
        {
            string connStr = site == "001" ? site01ConStr : site02ConStr;
            var cn = new SqlConnection(connStr);
            cn.Open();
            string sqlcmd = getSQLCmd(site, sensorType, date);
            return cn.Query<Enmos_Sensor_Value>(sqlcmd).ToList();
        }


        #endregion

        #region Modify Table Schema

        static void UpdateTblSchema(string date)
        {
            string month = date.Substring(0, 6);
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Impala_ODBC"].ConnectionString;
            con.Open();
            string sqlcmd = string.Format("ALTER TABLE enmos_sensor_value ADD PARTITION(month='{0}')", month);
            OdbcCommand cmd = new OdbcCommand(sqlcmd, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion

        #region Refresh Table Schema

        static void RefreshTblSchema()
        {
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Impala_ODBC"].ConnectionString;
            con.Open();
            string sqlcmd = "REFRESH enmos_sensor_value";
            OdbcCommand cmd = new OdbcCommand(sqlcmd, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion
    }
}
