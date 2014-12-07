using System.Collections.Generic;

namespace AC.Tools.Codes
{
	/// <summary>
	/// 业务领域对象类ITopicRepos 的摘要说明。
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	public interface ITopicRepos
	{
		/// <summary>
		/// 新增一条记录
		/// </summary>
		int Insert(TopicDTO topicDTO);

		/// <summary>
		/// 修改一条记录
		/// </summary>
		void Update(TopicDTO topicDTO);

		/// <summary>
		/// 删除一条记录
		/// </summary>
		void Delete(int topicId);

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		TopicDTO GetById(int topicId);

		/// <summary>
		/// 查询方法
		/// </summary>
		IList<TopicDTO> Query(TopicQueryDTO topicQueryDTO);

	}
}

