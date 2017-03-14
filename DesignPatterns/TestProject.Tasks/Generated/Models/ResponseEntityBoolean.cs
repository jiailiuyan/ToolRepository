// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace UserServiceClient.Models
{
    using System.Linq;

    /// <summary>
    /// 通用返回结果
    /// </summary>
    public partial class ResponseEntityBoolean
    {
        /// <summary>
        /// Initializes a new instance of the ResponseEntityBoolean class.
        /// </summary>
        public ResponseEntityBoolean() { }

        /// <summary>
        /// Initializes a new instance of the ResponseEntityBoolean class.
        /// </summary>
        /// <param name="code">执行结果状态码</param>
        /// <param name="bussCode">业务状态码</param>
        /// <param name="message">执行结果信息</param>
        /// <param name="data">执行结果内容</param>
        public ResponseEntityBoolean(int? code = default(int?), int? bussCode = default(int?), string message = default(string), bool? data = default(bool?))
        {
            Code = code;
            BussCode = bussCode;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Gets or sets 执行结果状态码
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Code")]
        public int? Code { get; set; }

        /// <summary>
        /// Gets or sets 业务状态码
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "BussCode")]
        public int? BussCode { get; set; }

        /// <summary>
        /// Gets or sets 执行结果信息
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets 执行结果内容
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Data")]
        public bool? Data { get; set; }

    }
}