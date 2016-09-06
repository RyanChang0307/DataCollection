using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using LINQtoCSV;
using HadoopUtil;
using System.Data.Odbc;

namespace Enmos_Receipe_DataCollection
{
    class Program
    {
        static string site01ConStr = ConfigurationManager.ConnectionStrings["ENMOS01"].ConnectionString;
        static string site02ConStr = ConfigurationManager.ConnectionStrings["ENMOS02"].ConnectionString;
        static void Main(string[] args)
        {
            string date = args.Length == 0 ? string.Format("{0:yyyy-MM-dd}", DateTime.Now) : string.Format("{0:yyyy-MM-dd}", args[0].ToString());
            string month=date.Substring(0, 7);
            List<Batch_Receipe> data = getDataFromDB("001", month).Union(getDataFromDB("002", month)).ToList();
            List<Batch_ReceipeNoEx> mappinglist = DyedRecipeNoSpilt(data.ToList());
            var query = from a in data
                        join b in mappinglist
                        on  new {
                             a.BatchOrderNo
                           , a.DyedRecipeNo } equals 
                          new { b.BatchOrderNo,b.DyedRecipeNo} 
                        select new
                        {
                            a.Site,
                            a.BatchOrderNo,
                            a.MachineNo,
                            a.DicId,
                            a.CustomerId,
                            a.FabricWeight,
                            a.ReelSpeed,
                            a.PumpSpeed,
                            a.CycleTime,
                            a.NozzleValue,
                            a.Flotte,
                            a.ManualStartTime,
                            a.MachineGroupNo,
                            b.DyedRecipeNoList,
                            a.NozzlePressure
                        };
            List<Batch_Receipe> list = new List<Batch_Receipe>();
            foreach( var a in query)
            {
                Batch_Receipe item = new Batch_Receipe();
                item.Site = a.Site;
                item.BatchOrderNo = a.BatchOrderNo;
                item.MachineNo = a.MachineNo;
                item.DicId = a.DicId;
                item.CustomerId = a.CustomerId;
                item.FabricWeight = a.FabricWeight;
                item.ReelSpeed = a.ReelSpeed;
                item.PumpSpeed = a.PumpSpeed;
                item.CycleTime = a.CycleTime;
                item.NozzleValue = a.NozzleValue;
                item.Flotte = a.Flotte;
                item.ManualStartTime = a.ManualStartTime;
                item.MachineGroupNo = a.MachineGroupNo;
                item.DyedRecipeNo = a.DyedRecipeNoList;
                item.NozzlePressure = a.NozzlePressure;
                list.Add(item);
            }
            ConvertDataToCsv(list,string.Format("{0}{1}",month.Substring(0,4),month.Substring(5,2)));



        }

        #region SqlCommand

        static string getSqlCmd(string site,string months)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(" SELECT '{0}' Site, A.BatchOrderNo, A.MachineNo, B.Id AS DICID, A.CustomerId, A.FabricWeight, A.ReelSpeed, A.PumpSpeed, A.CycleTime, A.NozzleValue, A.Flotte",site));
            sb.Append(" ,A.ManualStartTime, A.MachineGroupNo, A.DyedRecipeNo, A.NozzlePressure FROM  Recipe.BatchOrderHeader AS A LEFT OUTER JOIN Machine.Machine AS B ON A.MachineNo = B.Number");
            sb.Append(string.Format(" WHERE substring(Convert(char(30),A.CreatedTime,120),1,7)='{0}' AND (A.MachineNo<>0)", months));
            return sb.ToString();
        }



        #endregion

        #region Retrieve Data From RDBMS

        static List<Batch_Receipe> getDataFromDB(string site,string month)
        {
            string connStr = site == "001" ? site01ConStr : site02ConStr;
            var cn = new SqlConnection(connStr);
            cn.Open();
            string sqlcmd = getSqlCmd(site,month);
            return cn.Query<Batch_Receipe>(sqlcmd).ToList();
        }


        #endregion

        #region ETL

        static List<Batch_ReceipeNoEx> DyedRecipeNoSpilt(List<Batch_Receipe> list)
        {
            List<Batch_ReceipeNoEx> mappinglist = new List<Batch_ReceipeNoEx>();
            foreach(Batch_Receipe item in list)
            {
                string[] arrlist = item.DyedRecipeNo.Split('-');
                for(int i=1;i<arrlist.Length;i++)
                {
                    Batch_ReceipeNoEx mappingItem = new Batch_ReceipeNoEx();
                    mappingItem.BatchOrderNo = item.BatchOrderNo;
                    mappingItem.DyedRecipeNo= item.DyedRecipeNo;
                    mappingItem.DyedRecipeNoList = string.Format("{0}-{1}", arrlist[0].ToString(), arrlist[i].ToString());
                    mappinglist.Add(mappingItem);
                }
            }
            return mappinglist;
        }



        #endregion

        #region Convert data to csv

        static void ConvertDataToCsv(List<Batch_Receipe> data,string month)
        {
            CsvFileDescription outputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ','
                ,FirstLineHasColumnNames = false
                ,EnforceCsvColumnAttribute = true
            };
            CsvContext cc = new CsvContext();
            string filename =string.Format("Enmos_Batch_Order_{0}.csv",month);
            cc.Write(data, string.Format("C:\\DataCollection\\{0}", filename), outputFileDescription);
            PutDataToHDFS("C:\\DataCollection", filename, "/DataWarehouse/ENMOS/Enmos_Receipe_Order");

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
            string sqlcmd = "REFRESH Enmos_Receipe_Order";
            OdbcCommand cmd = new OdbcCommand(sqlcmd, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion










    }
}
