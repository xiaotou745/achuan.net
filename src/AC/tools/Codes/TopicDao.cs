#region 引用

using System;
using System.Data;
using System.Text;
using System.Collections.Generic;

#endregion

namespace AC.Tools.Codes
{
	/// <summary>
	/// 数据访问类TopicDao。
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	[Spring]
	public class TopicDao : Daobase, ITopicRepos	{
		public TopicDao()
		{}
		#region ITopicRepos  Members

		/// <summary>
		/// 增加一条记录
		/// </summary>
		public int Insert(TopicDTO topicDTO)
		{
			const string INSERT_SQL = @"
insert into topic(TopicType,TopicUrl,Title,Description,Status,CreateTime,PicUrl,IsDeleted,Author)
values(@TopicType,@TopicUrl,@Title,@Description,@Status,@CreateTime,@PicUrl,@IsDeleted,@Author)

select @@IDENTITY";

			IDbParameters dbParameters = DbHelper.CreateDbParameters();
			dbParameters.AddWithValue("TopicType", topicDTO.TopicType);
			dbParameters.AddWithValue("TopicUrl", topicDTO.TopicUrl);
			dbParameters.AddWithValue("Title", topicDTO.Title);
			dbParameters.AddWithValue("Description", topicDTO.Description);
			dbParameters.AddWithValue("Status", topicDTO.Status);
			dbParameters.AddWithValue("CreateTime", topicDTO.CreateTime);
			dbParameters.AddWithValue("PicUrl", topicDTO.PicUrl);
			dbParameters.AddWithValue("IsDeleted", topicDTO.IsDeleted);
			dbParameters.AddWithValue("Author", topicDTO.Author);


			object result = DbHelper.ExecuteScalar(ConnStringName, INSERT_SQL, dbParameters);
			if (result == null)
			{
				return 0;
			}
			return int.Parse(result.ToString());
		}

		/// <summary>
		/// 更新一条记录
		/// </summary>
		public void Update(TopicDTO topicDTO)
		{
			const string UPDATE_SQL = @"
update  topic
set  TopicType=@TopicType,TopicUrl=@TopicUrl,Title=@Title,Description=@Description,Status=@Status,CreateTime=@CreateTime,PicUrl=@PicUrl,IsDeleted=@IsDeleted,Author=@Author
where  TopicId=@TopicId ";

			IDbParameters dbParameters = DbHelper.CreateDbParameters();
			dbParameters.AddWithValue("TopicId", topicDTO.TopicId);
			dbParameters.AddWithValue("TopicType", topicDTO.TopicType);
			dbParameters.AddWithValue("TopicUrl", topicDTO.TopicUrl);
			dbParameters.AddWithValue("Title", topicDTO.Title);
			dbParameters.AddWithValue("Description", topicDTO.Description);
			dbParameters.AddWithValue("Status", topicDTO.Status);
			dbParameters.AddWithValue("CreateTime", topicDTO.CreateTime);
			dbParameters.AddWithValue("PicUrl", topicDTO.PicUrl);
			dbParameters.AddWithValue("IsDeleted", topicDTO.IsDeleted);
			dbParameters.AddWithValue("Author", topicDTO.Author);


			DbHelper.ExecuteNonQuery(ConnStringName,  UPDATE_SQL, dbParameters);
		}

		/// <summary>
		/// 删除一条记录
		/// </summary>
		public void Delete(int topicId)
		{
			const string DELETE_SQL = @"delete from topic where TopicId=@TopicId ";

			IDbParameters dbParameters = DbHelper.CreateDbParameters();
			dbParameters.AddWithValue("TopicId", topicId);



			DbHelper.ExecuteNonQuery(ConnStringOfBBS, DELETE_SQL, dbParameters);
		}

		/// <summary>
		/// 查询对象
		/// </summary>
		public IList<TopicDTO> Query(TopicQueryDTO topicQueryDTO)
		{
			string condition = BindQueryCriteria(topicDTO);
			string QUERY_SQL = @"
select  TopicId,TopicType,TopicUrl,Title,Description,Status,CreateTime,PicUrl,IsDeleted,Author
from  topic (nolock)" + condition;
			return DbHelper.QueryWithRowMapper(ConnStringOfBBS,QUERY_SQL,new topicRowMapper());
		}


		/// <summary>
		/// 根据ID获取对象
		/// </summary>
		public TopicDTO GetById(int topicId)
		{
			const string GETBYID_SQL = @"
select  TopicId,TopicType,TopicUrl,Title,Description,Status,CreateTime,PicUrl,IsDeleted,Author
from  topic (nolock)
where  TopicId=@TopicId ";

			IDbParameters dbParameters = DbHelper.CreateDbParameters();
			dbParameters.AddWithValue("TopicId", topicId);


			return DbHelper.QueryForObject(ConnStringOfBBS, GETBYID_SQL, dbParameters, new topicRowMapper());
		}

		#endregion

		#region  Nested type: topicRowMapper

		/// <summary>
		/// 绑定对象
		/// </summary>
		private class topicRowMapper : IDataTableRowMapper<TopicDTO>
		{
			public TopicDTO MapRow(DataRow dataReader)
			{
				var result = new TopicDTO();
				object obj;
				obj = dataReader["TopicId"];
				if (obj != null && obj != DBNull.Value)
				{
					result.TopicId = int.Parse(obj.ToString());
				}
				obj = dataReader["TopicType"];
				if (obj != null && obj != DBNull.Value)
				{
					result.TopicType = int.Parse(obj.ToString());
				}
				result.TopicUrl = dataReader["TopicUrl"].ToString();
				result.Title = dataReader["Title"].ToString();
				result.Description = dataReader["Description"].ToString();
				obj = dataReader["Status"];
				if (obj != null && obj != DBNull.Value)
				{
					result.Status = int.Parse(obj.ToString());
				}
				obj = dataReader["CreateTime"];
				if (obj != null && obj != DBNull.Value)
				{
					result.CreateTime = DateTime.Parse(obj.ToString());
				}
				result.PicUrl = dataReader["PicUrl"].ToString();
				obj = dataReader["IsDeleted"];
				if (obj != null && obj != DBNull.Value)
				{
					result.IsDeleted = int.Parse(obj.ToString());
				}
				result.Author = dataReader["Author"].ToString();

				return result;
			}
		}

		#endregion

		#region  Other Members

		/// <summary>
		/// 构造查询条件
		/// </summary>
		public static string BindQueryCriteria(TopicQueryDTO topicQueryDTO)
		{
			var stringBuilder = new StringBuilder(" where 1=1 ");
			if (topicQueryDTO == null)
			{
				return stringBuilder.ToString();
			}

			//TODO:在此加入查询条件构建代码

			return stringBuilder.ToString();
		}

		#endregion
	}
}
