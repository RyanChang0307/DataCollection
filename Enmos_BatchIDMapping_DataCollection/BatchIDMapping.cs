using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Enmos_BatchIDMapping_DataCollection
{
    public class BatchIDMapping
    {
        private string _Site;
        private string _BatchId;
        private string _BatchNo;
        private DateTime _Time;

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
        public string BatchId
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
        public string BatchNo
        {
            get
            {
                return _BatchNo;
            }

            set
            {
                _BatchNo = value;
            }
        }

        [CsvColumn(FieldIndex = 4, OutputFormat = "yyyy-MM-dd HH:mm:ss ", CanBeNull = true)]
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
