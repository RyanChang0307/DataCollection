using Dapper;
using HadoopUtil;
using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enmos_Actl_ChemicalUsage
{
    class Program
    {
        static string site01ConStr = ConfigurationManager.ConnectionStrings["ENMOS01"].ConnectionString;
        static string site02ConStr = ConfigurationManager.ConnectionStrings["ENMOS02"].ConnectionString;
        static void Main(string[] args)
        {
            List<ChemicalUsage> list = getDataFromDB("001");
            ConvertDataToCsv(list, "001");
            List<ChemicalUsage> list2 = getDataFromDB("002");
            ConvertDataToCsv(list2, "002");
            RefreshTblSchema();

        }

        #region Sql Command

        static string getSqlCmd(string site)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT   boh.Id AS BatchOrderId,boh.BatchOrderNo,Machine.GetMachineNameByMachineNo(boh.MachineNo) AS Machine,bod.ProductCode,p.Desc1 AS ProductName, SUM(bod.ProductWeight) AS ProductWeight");
            sb.Append(string.Format(" , boh.FabricWeight, p.TypeID, (Select Code From General.luProductType Where LanguageId = @@langId And [Index] = p.TypeID) AS ProductType,'{0}' SiteID", site));
            sb.Append(" FROM Recipe.BatchOrderHeader boh INNER JOIN Recipe.BatchOrderDetails bod ON bod.BatchOrderHeaderId = boh.Id");
            sb.Append(" INNER JOIN Recipe.Products p ON p.Code = bod.ProductCode INNER JOIN General.luProductType lpt ON lpt.[Index] = p.TypeID  AND LanguageId = @@langId");
            sb.Append(" GROUP BY boh.Id, boh.BatchOrderNo, boh.MachineNo, bod.ProductCode, p.Desc1, boh.FabricWeight, p.TypeID");
            return sb.ToString();

        }

        #endregion

        #region Retrieve data from database

        static List<ChemicalUsage> getDataFromDB(string site)
        {
            string connStr = site == "001" ? site01ConStr : site02ConStr;
            var cn = new SqlConnection(connStr);
            cn.Open();
            string sqlcmd = getSqlCmd(site);
            return cn.Query<ChemicalUsage>(sqlcmd).ToList();
        }


        #endregion

        #region PutDataToHDFS

        static void PutDataToHDFS(string srcPath, string srcFileName, string trgtFilePath)
        {
            HdfsUtil data = new HdfsUtil(new Uri("http://master01:50070"), "root");
            if (data.IsFileExist(trgtFilePath, srcFileName))
                data.DeleteFile(string.Format("{0}/{1}", trgtFilePath, srcFileName));
            data.PutDataToHdfs(string.Format("{0}\\{1}", srcPath, srcFileName), string.Format("{0}/{1}", trgtFilePath, srcFileName));
            //RefreshTblSchema();
        }


        #endregion

        #region Convert Data To CSV and Put data to hdfs

        /// <summary>
        /// Convert Data To CSV
        /// </summary>
        /// <param name="data"></param>
        static void ConvertDataToCsv(List<ChemicalUsage> data, string site)
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
            string filename = string.Format("Enmos_Actl_ChemicalUsage_{0}.csv", site);
            cc.Write(data, string.Format("C:\\DataCollection\\{0}", filename), outputFileDescription);
            PutDataToHDFS("C:\\DataCollection", filename, "/DataWarehouse/ENMOS/Enmos_Actl_ChemicalUsage");
        }

        #endregion

        #region Refresh Table Schema

        static void RefreshTblSchema()
        {
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Impala_ODBC"].ConnectionString;
            con.Open();
            string sqlcmd = "REFRESH Enmos_Actl_ChemicalUsage";
            OdbcCommand cmd = new OdbcCommand(sqlcmd, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion

    }
}
