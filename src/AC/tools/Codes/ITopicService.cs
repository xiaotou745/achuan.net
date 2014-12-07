using System.Collections.Generic;

namespace AC.Tools.Codes
{
	/// <summary>
	/// 业务逻辑类ITopicService 的摘要说明。
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	public interface ITopicService
	{
		/// <summary>
		/// 新增一条记录
		///<param name="topicDTO">要新增的对象</param>
		/// </summary>
		int Create(TopicDTO topicDTO);

		/// <summary>
		/// 修改一条记录
		///<param name="topicDTO">要修改的对象</param>
		/// </summary>
		void Modify(TopicDTO topicDTO);

		/// <summary>
		/// 删除一条记录
		/// </summary>
		void Remove(int topicId);

		/// <summary>
		/// 根据Id得到一个对象实体
		/// </summary>
		TopicDTO GetById(int topicId);

		/// <summary>
		/// 查询方法
		/// </summary>
		IList<TopicDTO> Query(TopicQueryDTO topicQueryDTO);

	}
}

