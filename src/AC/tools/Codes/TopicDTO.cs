using System;

namespace AC.Tools.Codes
{
	/// <summary>
	/// 实体类TopicDTO 。(属性说明自动提取数据库字段的描述信息)
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	public class TopicDTO
	{
		public TopicDTO() { }
		/// <summary>
		/// 
		/// </summary>
		public int TopicId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int TopicType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string TopicUrl { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PicUrl { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int IsDeleted { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Author { get; set; }

	}
	/// <summary>
	/// 查询对象类TopicQueryDTO 。(属性说明自动提取数据库字段的描述信息)
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	public class TopicQueryDTO
	{
		public TopicQueryDTO() { }
	}
}
