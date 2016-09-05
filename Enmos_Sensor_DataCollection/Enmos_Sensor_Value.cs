using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Enmos_Sensor_DataCollection
{
    public class Enmos_Sensor_Value
    {
        private string _SITE;
        private string _DICId;
        private string _SRCTYPE;
        private int _Index;
        private string _Desc1;
        private float _Value;
        private DateTime _Time;

        [CsvColumn(FieldIndex = 1,CanBeNull =false)]
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

        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public string DICId
        {
            get
            {
                return _DICId;
            }

            set
            {
                _DICId = value;
            }
        }

        [CsvColumn(FieldIndex = 3, CanBeNull = false)]
        public string SRCTYPE
        {
            get
            {
                return _SRCTYPE;
            }

            set
            {
                _SRCTYPE = value;
            }
        }

        [CsvColumn(FieldIndex = 4, CanBeNull = false)]
        public int Index
        {
            get
            {
                return _Index;
            }

            set
            {
                _Index = value;
            }
        }

        [CsvColumn(FieldIndex = 5, CanBeNull = true)]
        public string Desc1
        {
            get
            {
                return _Desc1;
            }

            set
            {
                _Desc1 = value;
            }
        }

        [CsvColumn(FieldIndex = 6, CanBeNull = true)]
        public float Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;
            }
        }

        [CsvColumn(FieldIndex = 7, OutputFormat = "yyyy-MM-dd HH:mm:ss ", CanBeNull = true)]
        public DateTime Time
        {
            get
            {
                return _Time;
            }

            set
            {
                _Time = value;
            }
        }


    }
}
