using Dapper;
using HadoopUtil;
using LINQtoCSV;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_WO_Color_Mapping
{
    class Program
    {
        static void Main(string[] args)
        {
            List<WO_Color_Mapping> list = getDataFromDB();
            ConvertDataToCsv(list);
        }

        #region Sql Command

        static string getSqlCmd()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select  GLTRP ORDERDATE,AUFNR WO,Z_PARTON_NAME_ZF COLORNAME ");
            sb.Append(" from  EVEREST04.cst_it_order WHERE GLTRP>='20160714'");
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
        static List<WO_Color_Mapping> getDataFromDB()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MES"].ConnectionString;
            var cn = new OracleConnection(connStr);
            cn.Open();
            string sqlcmd = getSqlCmd();
            return cn.Query<WO_Color_Mapping>(sqlcmd, commandTimeout: 0).ToList();
        }



        #endregion

        #region Convert Data To CSV and Put data to hdfs

        /// <summary>
        /// Convert Data To CSV
        /// </summary>
        /// <param name="data"></param>
        static void ConvertDataToCsv(List<WO_Color_Mapping> data)
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
            string filename = "MES_Wo_Color_Mapping.csv";
            cc.Write(data, string.Format("C:\\DataCollection\\{0}", filename), outputFileDescription);
            PutDataToHDFS("C:\\DataCollection", filename, "/DataWarehouse/MES/MES_Wo_Color_Mapping");
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
            string sqlcmd = "REFRESH MES_Wo_Color_Mapping";
            OdbcCommand cmd = new OdbcCommand(sqlcmd, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion

    }
}
