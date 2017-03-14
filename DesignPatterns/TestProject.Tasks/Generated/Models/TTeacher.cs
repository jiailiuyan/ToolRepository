// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace UserServiceClient.Models
{
    using System.Linq;

    public partial class TTeacher
    {
        /// <summary>
        /// Initializes a new instance of the TTeacher class.
        /// </summary>
        public TTeacher() { }

        /// <summary>
        /// Initializes a new instance of the TTeacher class.
        /// </summary>
        /// <param name="rowNum">行号</param>
        /// <param name="gradeId">年级</param>
        /// <param name="classId">班级</param>
        /// <param name="pjson">老师职位对象</param>
        /// <param name="postStr">列表显示</param>
        /// <param name="teachStr">列表显示</param>
        public TTeacher(int? teacherId = default(int?), string teacherUserName = default(string), string teacherName = default(string), int? schoolId = default(int?), string phone = default(string), string teacherPostJson = default(string), string teacherRoleJson = default(string), System.DateTime? createDate = default(System.DateTime?), bool? isDelete = default(bool?), long? rowNum = default(long?), int? gradeId = default(int?), int? classId = default(int?), TeacherPostJson pjson = default(TeacherPostJson), string postStr = default(string), string teachStr = default(string))
        {
            TeacherId = teacherId;
            TeacherUserName = teacherUserName;
            TeacherName = teacherName;
            SchoolId = schoolId;
            Phone = phone;
            TeacherPostJson = teacherPostJson;
            TeacherRoleJson = teacherRoleJson;
            CreateDate = createDate;
            IsDelete = isDelete;
            RowNum = rowNum;
            GradeId = gradeId;
            ClassId = classId;
            Pjson = pjson;
            PostStr = postStr;
            TeachStr = teachStr;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "TeacherId")]
        public int? TeacherId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "TeacherUserName")]
        public string TeacherUserName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "TeacherName")]
        public string TeacherName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "SchoolId")]
        public int? SchoolId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "TeacherPostJson")]
        public string TeacherPostJson { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "TeacherRoleJson")]
        public string TeacherRoleJson { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "CreateDate")]
        public System.DateTime? CreateDate { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "IsDelete")]
        public bool? IsDelete { get; set; }

        /// <summary>
        /// Gets or sets 行号
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "RowNum")]
        public long? RowNum { get; set; }

        /// <summary>
        /// Gets or sets 年级
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "GradeId")]
        public int? GradeId { get; set; }

        /// <summary>
        /// Gets or sets 班级
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "ClassId")]
        public int? ClassId { get; set; }

        /// <summary>
        /// Gets or sets 老师职位对象
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Pjson")]
        public TeacherPostJson Pjson { get; set; }

        /// <summary>
        /// Gets or sets 列表显示
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "PostStr")]
        public string PostStr { get; set; }

        /// <summary>
        /// Gets or sets 列表显示
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "TeachStr")]
        public string TeachStr { get; set; }

    }
}