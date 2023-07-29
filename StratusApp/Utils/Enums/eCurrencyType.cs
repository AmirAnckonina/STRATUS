using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Utils.Enums
{
    public enum eCurrencyType
    {
        [Display(Name = "$")]
        Dollar,
        [Display(Name = "€")]
        Euro,
        [Display(Name = "₪")]
        Shekel,
    }
}
