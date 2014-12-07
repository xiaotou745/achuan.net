using System.Collections.Generic;

namespace AC.Data.ConnString
{
	/// <summary>
	/// 创建ConnString接口
	/// </summary>
	public interface IConnectionStringCreator
	{
		/// <summary>
		/// 创建所有的ConnString.
		/// </summary>
		/// <returns></returns>
		IList<IConnectionString> CreateConnStrings();
	}
}