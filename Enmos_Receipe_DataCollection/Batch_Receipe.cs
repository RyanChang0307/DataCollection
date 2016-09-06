using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Enmos_Receipe_DataCollection
{
    public class Batch_Receipe
    {
        private string _Site;
        private string _BatchOrderNo;
        private int _MachineNo;
        private int _DicId;
        private int _CustomerId;  //5
        private float _FabricWeight;
        private float _ReelSpeed;
        private float _PumpSpeed;
        private int _CycleTime;
        private float _NozzleValue; //10
        private float _Flotte;
        private DateTime _ManualStartTime;
        private int _MachineGroupNo;
        private string _DyedRecipeNo;
        private float _NozzlePressure;

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

        [CsvColumn(FieldIndex = 3, CanBeNull = false)]
        public int MachineNo
        {
            get
            {
                return _MachineNo;
            }

            set
            {
                _MachineNo = value;
            }
        }

        [CsvColumn(FieldIndex = 4, CanBeNull = false)]
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

        [CsvColumn(FieldIndex = 5, CanBeNull = true)]
        public int CustomerId
        {
            get
            {
                return _CustomerId;
            }

            set
            {
                _CustomerId = value;
            }
        }

        [CsvColumn(FieldIndex = 6, CanBeNull = true)]
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

        [CsvColumn(FieldIndex = 7, CanBeNull = true)]
        public float ReelSpeed
        {
            get
            {
                return _ReelSpeed;
            }

            set
            {
                _ReelSpeed = value;
            }
        }

        [CsvColumn(FieldIndex = 8, CanBeNull = true)]
        public float PumpSpeed
        {
            get
            {
                return _PumpSpeed;
            }

            set
            {
                _PumpSpeed = value;
            }
        }

        [CsvColumn(FieldIndex = 9, CanBeNull = true)]
        public int CycleTime
        {
            get
            {
                return _CycleTime;
            }

            set
            {
                _CycleTime = value;
            }
        }

        [CsvColumn(FieldIndex = 10, CanBeNull = true)]
        public float NozzleValue
        {
            get
            {
                return _NozzleValue;
            }

            set
            {
                _NozzleValue = value;
            }
        }

        [CsvColumn(FieldIndex = 11, CanBeNull = true)]
        public float Flotte
        {
            get
            {
                return _Flotte;
            }

            set
            {
                _Flotte = value;
            }
        }

        [CsvColumn(FieldIndex = 12, CanBeNull = true, OutputFormat = "yyyy-MM-dd HH:mm:ss ")]
        public DateTime ManualStartTime
        {
            get
            {
                return _ManualStartTime;
            }

            set
            {
                _ManualStartTime = value;
            }
        }
        [CsvColumn(FieldIndex = 13, CanBeNull = true)]
        public int MachineGroupNo
        {
            get
            {
                return _MachineGroupNo;
            }

            set
            {
                _MachineGroupNo = value;
            }
        }

        [CsvColumn(FieldIndex = 14, CanBeNull = false)]
        public string DyedRecipeNo
        {
            get
            {
                return _DyedRecipeNo;
            }

            set
            {
                _DyedRecipeNo = value;
            }
        }
        [CsvColumn(FieldIndex = 15, CanBeNull = true)]
        public float NozzlePressure
        {
            get
            {
                return _NozzlePressure;
            }

            set
            {
                _NozzlePressure = value;
            }
        }


    }
}
