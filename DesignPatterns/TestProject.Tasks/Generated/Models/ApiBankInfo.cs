// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace UserServiceClient.Models
{
    using System.Linq;

    public partial class ApiBankInfo
    {
        /// <summary>
        /// Initializes a new instance of the ApiBankInfo class.
        /// </summary>
        public ApiBankInfo() { }

        /// <summary>
        /// Initializes a new instance of the ApiBankInfo class.
        /// </summary>
        public ApiBankInfo(int? bankID = default(int?), string bankName = default(string))
        {
            BankID = bankID;
            BankName = bankName;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "BankID")]
        public int? BankID { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "BankName")]
        public string BankName { get; set; }

    }
}
