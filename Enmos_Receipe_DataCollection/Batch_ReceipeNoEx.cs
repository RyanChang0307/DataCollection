using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enmos_Receipe_DataCollection
{
    public class Batch_ReceipeNoEx
    {
        private string _BatchOrderNo;
        private string _DyedRecipeNoZ;
        private string _DyedRecipeNoList;

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

        public string DyedRecipeNo
        {
            get
            {
                return _DyedRecipeNoZ;
            }

            set
            {
                _DyedRecipeNoZ = value;
            }
        }

        public string DyedRecipeNoList
        {
            get
            {
                return _DyedRecipeNoList;
            }

            set
            {
                _DyedRecipeNoList = value;
            }
        }

    }
}
