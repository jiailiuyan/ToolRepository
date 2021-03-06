// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace UserServiceClient.Models
{
    using System.Linq;

    /// <summary>
    /// 班主任
    /// </summary>
    public partial class HeadMaster
    {
        /// <summary>
        /// Initializes a new instance of the HeadMaster class.
        /// </summary>
        public HeadMaster() { }

        /// <summary>
        /// Initializes a new instance of the HeadMaster class.
        /// </summary>
        /// <param name="grade">年级</param>
        /// <param name="classid">班级</param>
        public HeadMaster(int? grade = default(int?), int? classid = default(int?))
        {
            Grade = grade;
            Classid = classid;
        }

        /// <summary>
        /// Gets or sets 年级
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "grade")]
        public int? Grade { get; set; }

        /// <summary>
        /// Gets or sets 班级
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "classid")]
        public int? Classid { get; set; }

    }
}
