using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickStart
{
    class IniConfig
    {
        private string strButton_Name;
        private string strButton_Visible;
        private string strProgram_Name;
        private string strProgram_Type;
        private string strProgram_Param;

        public string Button_Name
        {
            get
            {
                return strButton_Name;
            }
            set
            {
                strButton_Name = value;
            }
        }

        public string Button_Visible
        {
            get
            {
                return strButton_Visible;
            }
            set
            {
                strButton_Visible = value;
            }
        }

        public string Program_Name
        {
            get
            {
                return strProgram_Name;
            }
            set
            {
                strProgram_Name = value;
            }
        }

        public string Program_Type
        {
            get
            {
                return strProgram_Type;
            }
            set
            {
                strProgram_Type = value;
            }
        }

        public string Program_Param
        {
            get
            {
                return strProgram_Param;
            }
            set
            {
                strProgram_Param = value;
            }
        }

    }
}
