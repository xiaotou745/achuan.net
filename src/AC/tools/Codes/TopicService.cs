using System.Collections.Generic;

namespace AC.Tools.Codes
{
	/// <summary>
	/// Service类TopicService 的摘要说明。
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	[Spring(ConstructorArgs = "topicRepos:topicRepos")]
	public class TopicService : ITopicService
	{
		private readonly ITopicRepos topicRepos;
		public TopicService(ITopicRepos topicRepos)
		{
			this.topicRepos = topicRepos;
		}
		/// <summary>
		/// 新增一条记录
		/// </summary>
		public int Create(TopicDTO topicDTO)
		{
			return topicRepos.Insert(topicDTO);
		}

		/// <summary>
		/// 修改一条记录
		/// </summary>
		public void Modify(TopicDTO topicDTO)
		{
			topicRepos.Update(topicDTO);
		}

		/// <summary>
		/// 删除一条记录
		/// </summary>
		public void Remove(int topicId)
		{
			topicRepos.Delete(topicId);
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public TopicDTO GetById(int topicId)
		{
			return topicRepos.GetById(topicId);
		}

		/// <summary>
		/// 查询方法
		/// </summary>
		public IList<TopicDTO> Query(TopicQueryDTO topicQueryDTO)
		{
			return topicRepos.Query(topicQueryDTO);
		}

	}
}

