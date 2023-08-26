using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Enums;

namespace Utils.Utils
{
    public static class UnitConvertorMapper
    {
        // Define a dictionary for unit conversion factors
        private static readonly Dictionary<string, eSizeUnit> unitConvertor;

        static UnitConvertorMapper() 
        {
            unitConvertor = new Dictionary<string, eSizeUnit>
            {
                {"KiB", eSizeUnit.KB},
                {"MiB", eSizeUnit.MB},
                {"GiB", eSizeUnit.GB},
                {"TiB", eSizeUnit.TB},
                {"KB", eSizeUnit.KB},
                {"MB", eSizeUnit.MB},
                {"GB", eSizeUnit.GB},
                {"TB", eSizeUnit.TB}
            };
        }


        public static Dictionary<string, eSizeUnit> UnitConvertor { get => unitConvertor; }
    }
}
