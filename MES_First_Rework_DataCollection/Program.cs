using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using System.Data.Odbc;
using LINQtoCSV;
using HadoopUtil;
using Oracle.ManagedDataAccess.Client;

namespace MES_First_Rework_DataCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            string date = args.Length == 0 ? string.Format("{0:yyyy/MM/dd}", DateTime.Now.AddDays(-1)) : string.Format("{0:yyyy/MM/dd}", args[0].ToString());
             List<MES_First_Rework> list = getDataFromDB(date);
            ConvertDataToCsv(list);


        }

        #region Sql Command

        static string getSqlCmd(string date)
        {
            StringBuilder sb = new StringBuilder();
             sb.Append(" SELECT  TO_DATE(TRANSTIME,'YYYY-MM-DD HH24:MI:SS')  TRANSTIME,SITE,WO,LOTID,COLORNAME,REWORKCOUNT,REASONCODE,DESCR,COALESCE(NEWQTY,0) NEWQTY,REWORKTYPE,Z_CUST_CNO ");
            sb.Append(" FROM EVERESTDM04_2.DMV_CSFR751");
            sb.Append(string.Format(" WHERE SUBSTR(TO_CHAR(SHIFTDATE),1,10) between '2016/07/14' AND '{0}' ", date));
            sb.Append(" AND REASONCODE IN ('317','319') AND REWORKCOUNT=1"); 
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
        static List<MES_First_Rework> getDataFromDB( string date)
        {
            string connStr= ConfigurationManager.ConnectionStrings["MES_DM"].ConnectionString;
            var cn = new OracleConnection(connStr);
            cn.Open();
            string sqlcmd = getSqlCmd( date);
            return cn.Query<MES_First_Rework>(sqlcmd, commandTimeout: 0).ToList();
        }



        #endregion

        #region Convert Data To CSV and Put data to hdfs

        /// <summary>
        /// Convert Data To CSV
        /// </summary>
        /// <param name="data"></param>
        static void ConvertDataToCsv(List<MES_First_Rework> data)
        {
            CsvFileDescription outputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ','
                ,
                FirstLineHasColumnNames = false
                ,
                EnforceCsvColumnAttribute = true
            };
            CsvContext cc = new CsvContext();
            string filename = "MES_FIRST_REWORK.csv";
            cc.Write(data, string.Format("C:\\DataCollection\\{0}", filename), outputFileDescription);
            PutDataToHDFS("C:\\DataCollection", filename, "/DataWarehouse/MES_DM/MES_FIRST_REWORK");
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
            string sqlcmd = "REFRESH MES_FIRST_REWORK";
            OdbcCommand cmd = new OdbcCommand(sqlcmd, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion
    }
}
