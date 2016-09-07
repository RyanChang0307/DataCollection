using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Enmos_BatchCommandList_DataCollection
{
    public class BatchCommandList
    {
        private string _Site;
        private int _BatchId;
        private int _DicId;
        private int _StepNumber;
        private string _Desc1;
        private DateTime _StartTime;
        private DateTime _EndTime;

        [CsvColumn(FieldIndex = 1, CanBeNull = false)]
        public string Site
        {
            get
            {
                return _Site;
            }

            set
            {
                _Site = value;
            }
        }

        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public int BatchId
        {
            get
            {
                return _BatchId;
            }

            set
            {
                _BatchId = value;
            }
        }

        [CsvColumn(FieldIndex = 3, CanBeNull = false)]
        public int DicId
        {
            get
            {
                return _DicId;
            }

            set
            {
                _DicId = value;
            }
        }

        [CsvColumn(FieldIndex = 4, CanBeNull = false)]
        public int StepNumber
        {
            get
            {
                return _StepNumber;
            }

            set
            {
                _StepNumber = value;
            }
        }

        [CsvColumn(FieldIndex = 5, CanBeNull = false)]
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

        [CsvColumn(FieldIndex = 6, OutputFormat = "yyyy-MM-dd HH:mm:ss ", CanBeNull = true)]
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }

            set
            {
                _StartTime = value;
            }
        }

        [CsvColumn(FieldIndex = 7, OutputFormat = "yyyy-MM-dd HH:mm:ss ", CanBeNull = true)]
        public DateTime EndTime
        {
            get
            {
                return _EndTime;
            }

            set
            {
                _EndTime = value;
            }
        }


    }
}
