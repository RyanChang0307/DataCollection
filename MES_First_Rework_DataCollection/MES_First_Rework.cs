using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace MES_First_Rework_DataCollection
{
    public class MES_First_Rework
    {
        private DateTime _TRANSTIME;
        private string _SITE;
        private string _WO;
        private string _LOTID;
        private string _COLORNAME;
        private long _REWORKCOUNT;
        private string _REASONCODE;
        private string _DESCR;
        private long _NEWQTY;
        private string _REWORKTYPE;
        private string _Z_CUST_CNO;

        [CsvColumn(FieldIndex = 1, CanBeNull = false, OutputFormat = "yyyy-MM-dd HH:mm:ss ")]
        public DateTime TRANSTIME
        {
            get
            {
                return _TRANSTIME;
            }

            set
            {
                _TRANSTIME = value;
            }
        }

        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public string SITE
        {
            get
            {
                return _SITE;
            }

            set
            {
                _SITE = value;
            }
        }

        [CsvColumn(FieldIndex = 3, CanBeNull = false)]
        public string WO
        {
            get
            {
                return _WO;
            }

            set
            {
                _WO = value;
            }
        }

        [CsvColumn(FieldIndex = 4, CanBeNull = false)]
        public string LOTID
        {
            get
            {
                return _LOTID;
            }

            set
            {
                _LOTID = value;
            }
        }
        [CsvColumn(FieldIndex = 5, CanBeNull = true)]
        public string COLORNAME
        {
            get
            {
                return _COLORNAME;
            }

            set
            {
                _COLORNAME = value;
            }
        }

        [CsvColumn(FieldIndex = 6, CanBeNull = true)]
        public long REWORKCOUNT
        {
            get
            {
                return _REWORKCOUNT;
            }

            set
            {
                _REWORKCOUNT = value;
            }
        }

        [CsvColumn(FieldIndex = 7, CanBeNull = true)]
        public string REASONCODE
        {
            get
            {
                return _REASONCODE;
            }

            set
            {
                _REASONCODE = value;
            }
        }

        [CsvColumn(FieldIndex = 8, CanBeNull = true)]
        public string DESCR
        {
            get
            {
                return _DESCR;
            }

            set
            {
                _DESCR = value;
            }
        }

        [CsvColumn(FieldIndex = 9, CanBeNull = true)]
        public long NEWQTY
        {
            get
            {
                return _NEWQTY;
            }

            set
            {
                _NEWQTY = value;
            }
        }

        [CsvColumn(FieldIndex = 10, CanBeNull = true)]
        public string REWORKTYPE
        {
            get
            {
                return _REWORKTYPE;
            }

            set
            {
                _REWORKTYPE = value;
            }
        }

        [CsvColumn(FieldIndex = 11, CanBeNull = true)]
        public string Z_CUST_CNO
        {
            get
            {
                return _Z_CUST_CNO;
            }

            set
            {
                _Z_CUST_CNO = value;
            }
        }
    }
}
