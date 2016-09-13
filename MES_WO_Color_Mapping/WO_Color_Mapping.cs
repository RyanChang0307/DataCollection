using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace MES_WO_Color_Mapping
{
   public  class WO_Color_Mapping
    {
        private string _OrderDate;
        private string _Wo;
        private string _ColorName;

        [CsvColumn(FieldIndex = 1, CanBeNull = false)]
        public string OrderDate
        {
            get
            {
                return _OrderDate;
            }

            set
            {
                _OrderDate = value;
            }
        }

        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public string Wo
        {
            get
            {
                return _Wo;
            }

            set
            {
                _Wo = value;
            }
        }

        [CsvColumn(FieldIndex = 3, CanBeNull = true)]
        public string ColorName
        {
            get
            {
                return _ColorName;
            }

            set
            {
                _ColorName = value;
            }
        }
    }
}
