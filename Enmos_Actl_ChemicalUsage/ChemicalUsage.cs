using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Enmos_Actl_ChemicalUsage
{
  public  class ChemicalUsage
    {
        private string _SiteID;
        private int _BatchOrderID;
        private string _BatchOrderNo;
        private int _Machine;
        private string _ProductCode;
        private string _ProductName;
        private float _ProductWeight;
        private float _FabricWeight;
        private int _TypeID;
        private string _ProductType;

        [CsvColumn(FieldIndex = 1, CanBeNull = false)]
        public int BatchOrderID
        {
            get
            {
                return _BatchOrderID;
            }

            set
            {
                _BatchOrderID = value;
            }
        }

        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public string BatchOrderNo
        {
            get
            {
                return _BatchOrderNo;
            }

            set
            {
                _BatchOrderNo = value;
            }
        }
        [CsvColumn(FieldIndex = 3, CanBeNull = true)]
        public int Machine
        {
            get
            {
                return _Machine;
            }

            set
            {
                _Machine = value;
            }
        }
        [CsvColumn(FieldIndex = 4, CanBeNull = true)]
        public string ProductCode
        {
            get
            {
                return _ProductCode;
            }

            set
            {
                _ProductCode = value;
            }
        }
        [CsvColumn(FieldIndex = 5, CanBeNull = true)]
        public string ProductName
        {
            get
            {
                return _ProductName;
            }

            set
            {
                _ProductName = value;
            }
        }
        [CsvColumn(FieldIndex = 6, CanBeNull = true)]
        public float ProductWeight
        {
            get
            {
                return _ProductWeight;
            }

            set
            {
                _ProductWeight = value;
            }
        }
        [CsvColumn(FieldIndex = 7, CanBeNull = true)]
        public float FabricWeight
        {
            get
            {
                return _FabricWeight;
            }

            set
            {
                _FabricWeight = value;
            }
        }
        [CsvColumn(FieldIndex = 8, CanBeNull = true)]
        public int TypeID
        {
            get
            {
                return _TypeID;
            }

            set
            {
                _TypeID = value;
            }
        }
        [CsvColumn(FieldIndex = 9, CanBeNull = true)]
        public string ProductType
        {
            get
            {
                return _ProductType;
            }

            set
            {
                _ProductType = value;
            }
        }
        [CsvColumn(FieldIndex = 10, CanBeNull = false)]
        public string SiteID
        {
            get
            {
                return _SiteID;
            }

            set
            {
                _SiteID = value;
            }
        }
    }
}
