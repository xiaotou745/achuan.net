using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace AC.Code.Helper
{
	/// <summary>
	/// ����Э��ͨ����
	/// </summary>
	public class CodeCommon
	{
	    private static readonly DataTypeHelper DataTypeHelper = DataTypeHelper.Create();

		#region �������

		/// <summary>
		/// �������
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static string Space(int num)
		{
			var str = new StringBuilder();
			for (int n = 0; n < num; n++)
			{
				str.Append("\t");
			}
			return str.ToString();
		}

		#endregion

		#region ת�� ���ݿ��ֶ����� Ϊ c#����

		/// <summary>
		/// ת�������ݿ��ֶ����͡� =��Ϊ��c#���͡�
		/// </summary>
		/// <param name="dbtype">���ݿ��ֶ�����</param>
		/// <returns>c#����</returns>		
		public static string DbTypeToCS(string dbtype)
		{
		    return DataTypeHelper.DbTypeToCS(dbtype);
		}

	    public static string DbTypeToJava(string dbtype)
	    {
	        return DataTypeHelper.DbTypeToJava(dbtype);
	    }

	    #endregion

		#region �Ƿ�c#�е�ֵ����

		/// <summary>
		/// �Ƿ�c#�е�ֵ����
		/// </summary>
		public static bool IsValueType(string cstype)
		{
		    return DataTypeHelper.IsValueType(cstype);
		}

		#endregion

		#region ���洢���̲������õ� ���ݿ��ֶ����� �� ���� (�����Ƿ���Ҫ��)

	    /// <summary>
	    /// ���洢���̲������õ����ݿ��ֶ����͵ĳ���(�����Ƿ���Ҫ��)
	    /// </summary>
	    /// <param name="dbtype">���ݿ��ֶ�����</param>
	    /// <param name="datatype"> </param>
	    /// <param name="length"> </param>
	    /// <returns></returns>
	    public static string DbTypeLength(string dbtype, string datatype, string length)
		{
			string strtype = "";
			switch (dbtype)
			{
				case "SQL2000":
				case "SQL2005":
					strtype = DbTypeLengthSQL(dbtype, datatype, length);
					break;
				case "Oracle":
					strtype = DbTypeLengthOra(datatype, length);
					break;
				case "MySQL":
					strtype = DbTypeLengthMySQL(datatype, length);
					break;
				case "OleDb":
					strtype = DbTypeLengthOleDb(datatype, length);
					break;
			}
			return strtype;
		}

		#region DbTypeLength

		/// <summary>
		/// �õ�ĳ�������ֶ�Ӧ�õĳ���
		/// </summary>
		public static string GetDataTypeLenVal(string datatype, string length)
		{
			string lenVal = "";
			switch (datatype.Trim())
			{
				case "int":
					lenVal = "4";
					break;
				case "char":
					{
						if (length.Trim() == "")
						{
							lenVal = "10";
						}
						else
						{
							lenVal = length;
						}
					}
					break;
				case "nchar":
					{
						lenVal = length;
						if (length.Trim() == "")
						{
							lenVal = "10";
						}
						//else
						//{
						//    LenVal = (int.Parse(Length.Trim()) / 2).ToString();
						//}   
					}
					break;
				case "varchar":
					{
						lenVal = length;
						if (length.Trim() == "")
						{
							lenVal = "50";
						}
						else
						{
							if (int.Parse(length.Trim()) < 1)
							{
								lenVal = "";
							}
						}
					}
					break;
				case "nvarchar":
					{
						lenVal = length;
						if (length.Trim() == "")
						{
							lenVal = "50";
						}
						else
						{
							if (int.Parse(length.Trim()) < 1)
							{
								lenVal = "";
							}
							//else
							//{
							//    LenVal = (int.Parse(Length.Trim()) / 2).ToString();
							//}
						}
					}
					break;
				case "varbinary":
					{
						lenVal = length;
						if (length.Trim() == "")
						{
							lenVal = "50";
						}
						else
						{
							if (int.Parse(length.Trim()) < 1)
							{
								lenVal = "";
							}
						}
					}
					break;
				case "bit":
					lenVal = "1";
					break;
				case "float":
				case "numeric":
				case "decimal":
				case "money":
				case "smallmoney":
				case "binary":
				case "smallint":
				case "bigint":
					lenVal = length;
					break;
				case "image ":
				case "datetime":
				case "smalldatetime":
				case "ntext":
				case "text":
					break;
				default:
					lenVal = length;
					break;
			}
			return lenVal;
		}

		private static string DbTypeLengthSQL(string dbtype, string datatype, string Length)
		{
			string LenVal = GetDataTypeLenVal(datatype, Length);
			string lenstr = "";
			if (LenVal != "")
			{
				lenstr = CSToProcType(dbtype, datatype) + "," + LenVal;
			}
			else
			{
				lenstr = CSToProcType(dbtype, datatype);
			}
			return lenstr;
		}

		private static string DbTypeLengthOra(string datatype, string Length)
		{
			string len = "";
			switch (datatype.Trim().ToLower())
			{
				case "number":
					len = "4";
					break;
				case "varchar2":
					{
						if (Length == "")
						{
							len = "50";
						}
						else
						{
							len = Length;
						}
					}
					break;
				case "char":
					{
						if (Length == "")
						{
							len = "50";
						}
						else
						{
							len = Length;
						}
					}
					break;
				case "date":
				case "nchar":
				case "nvarchar2":
				case "long":
				case "long raw":
				case "bfile":
				case "blob":
					break;
				default:
					len = Length;
					break;
			}

			if (len != "")
			{
				len = CSToProcType("Oracle", datatype) + "," + len;
			}
			else
			{
				len = CSToProcType("Oracle", datatype);
			}
			return len;
		}

		private static string DbTypeLengthMySQL(string datatype, string Length)
		{
			string len = "";
			switch (datatype.Trim().ToLower())
			{
				case "number":
					len = "4";
					break;
				case "varchar2":
					{
						if (Length == "")
						{
							len = "50";
						}
						else
						{
							len = Length;
						}
					}
					break;
				case "char":
					{
						if (Length == "")
						{
							len = "50";
						}
						else
						{
							len = Length;
						}
					}
					break;
				case "date":
				case "nchar":
				case "nvarchar2":
				case "long":
				case "long raw":
				case "bfile":
				case "blob":
					break;
				default:
					len = Length;
					break;
			}

			if (len != "")
			{
				len = CSToProcType("MySQL", datatype) + "," + len;
			}
			else
			{
				len = CSToProcType("MySQL", datatype);
			}
			return len;
		}

		private static string DbTypeLengthOleDb(string datatype, string Length)
		{
			string len = "";
			switch (datatype.Trim())
			{
				case "int":
					len = "4";
					break;
				case "varchar":
					{
						if (Length == "")
						{
							len = "50";
						}
						else
						{
							len = Length;
						}
					}
					break;
				case "char":
					{
						if (Length == "")
						{
							len = "50";
						}
						else
						{
							len = Length;
						}
					}
					break;
				case "bit":
					len = "1";
					break;
				case "float":
				case "numeric":
				case "decimal":
				case "money":
				case "smallmoney":
				case "binary":
				case "smallint":
				case "bigint":
					len = Length;
					break;
				case "image ":
				case "datetime":
				case "smalldatetime":
				case "nchar":
				case "nvarchar":
				case "ntext":
				case "text":
					break;
				default:
					len = Length;
					break;
			}

			if (len != "")
			{
				len = CSToProcType("OleDb", datatype) + "," + len;
			}
			else
			{
				len = CSToProcType("OleDb", datatype);
			}
			return len;
		}

		#endregion

		#endregion

		#region ת�� ��c#���� �� �������͡� תΪ ���洢���̵Ĳ�����

	    /// <summary>
	    /// ת��c#���ͺ���������תΪ�洢���̵Ĳ�������
	    /// </summary>
	    /// <param name="dbType">���ݿ��ֶ����� </param>
	    /// <param name="cstype"> </param>
	    /// <returns>c#����</returns>
	    public static string CSToProcType(string dbType, string cstype)
		{
			string strtype = cstype;
			switch (dbType)
			{
				case "SQL2000":
				case "SQL2005":
					strtype = CSToProcTypeSQL(cstype);
					break;
				case "Oracle":
					strtype = CSToProcTypeOra(cstype);
					break;
				case "MySQL":
					strtype = CSToProcTypeMySQL(cstype);
					break;
				case "OleDb":
					strtype = CSToProcTypeOleDb(cstype);
					break;
			}
			return strtype;
		}

		#region CSToProcType

		private static string CSToProcTypeSQL(string cstype)
		{
		    return DataTypeHelper.CSToProcTypeSQL(cstype);
		}

		private static string CSToProcTypeOra(string cstype)
		{
		    return DataTypeHelper.CSToProcTypeOra(cstype);
		}

		private static string CSToProcTypeMySQL(string cstype)
		{
		    return DataTypeHelper.CSToProcTypeMySQL(cstype);
		}

		private static string CSToProcTypeOleDb(string cstype)
		{
		    return DataTypeHelper.CSToProcTypeOleDb(cstype);
		}

		#endregion

		#endregion

		#region �����������Ƿ�ӵ�����

		/// <summary>
		/// �����������Ƿ�ӵ�����
		/// </summary>
		/// <param name="columnType">���ݿ�����</param>
		/// <returns></returns>
		public static bool IsAddMark(string columnType)
		{
		    return DataTypeHelper.IsAddMark(columnType);
		}

		#endregion

		#region byte������ת16���� 

		private static readonly char[] HexDigits = {
		                                           	'0', '1', '2', '3', '4', '5', '6', '7',
		                                           	'8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
		                                           };

		public static string ToHexString(byte[] bytes)
		{
			var chars = new char[bytes.Length*2];
			for (int i = 0; i < bytes.Length; i++)
			{
				int b = bytes[i];
				chars[i*2] = HexDigits[b >> 4];
				chars[i*2 + 1] = HexDigits[b & 0xF];
			}
			var str = new string(chars);
			return "0x" + str.Substring(0, bytes.Length);
		}

		#endregion

		#region  �õ�����ֶ�List������Ϣ

		/// <summary>
		/// �����ֶ���Ϣ������DataTable��תΪ �������ֶζ�������List-ColumnInfo��
		/// </summary>
		public static List<ColumnInfo> GetColumnInfos(DataTable dt)
		{
			var keys = new List<ColumnInfo>();
			if (dt != null)
			{
				var colexist = new ArrayList(); //�Ƿ��Ѿ�����
				ColumnInfo key;
				foreach (DataRow row in dt.Rows)
				{
					string Colorder = row["Colorder"].ToString();
					string ColumnName = row["ColumnName"].ToString();
					string TypeName = row["TypeName"].ToString();
					string isIdentity = row["IsIdentity"].ToString();
					string IsPK = row["IsPK"].ToString();
					string Length = row["Length"].ToString();
					string Preci = row["Preci"].ToString();
					string Scale = row["Scale"].ToString();
					string cisNull = row["cisNull"].ToString();
					string DefaultVal = row["DefaultVal"].ToString();
					string DeText = row["DeText"].ToString();

					key = new ColumnInfo();
					key.Colorder = Colorder;
					key.ColumnName = ColumnName;
					key.TypeName = TypeName;
					key.IsIdentity = (isIdentity == "��") ? true : false;
					key.IsPK = (IsPK == "��") ? true : false;
					key.Length = Length;
					key.Preci = Preci;
					key.Scale = Scale;
					key.cisNull = (cisNull == "��") ? true : false;
					key.DefaultVal = DefaultVal;
					key.DeText = DeText;

					if (!colexist.Contains(ColumnName))
					{
						keys.Add(key);
						colexist.Add(ColumnName);
					}
				}
				return keys;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// ���������ֶζ�������List-ColumnInfo��תΪ ���ֶ���Ϣ������DataTable��
		/// </summary>        
		public static DataTable GetColumnInfoDt(List<ColumnInfo> collist)
		{
			var cTable = new DataTable();
			cTable.Columns.Add("colorder");
			cTable.Columns.Add("ColumnName");
			cTable.Columns.Add("TypeName");
			cTable.Columns.Add("Length");
			cTable.Columns.Add("Preci");
			cTable.Columns.Add("Scale");
			cTable.Columns.Add("IsIdentity");
			cTable.Columns.Add("isPK");
			cTable.Columns.Add("cisNull");
			cTable.Columns.Add("defaultVal");
			cTable.Columns.Add("deText");

			foreach (ColumnInfo col in collist)
			{
				DataRow newRow = cTable.NewRow();
				newRow["colorder"] = col.Colorder;
				newRow["ColumnName"] = col.ColumnName;
				newRow["TypeName"] = col.TypeName;
				newRow["Length"] = col.Length;
				newRow["Preci"] = col.Preci;
				newRow["Scale"] = col.Scale;
				newRow["IsIdentity"] = (col.IsIdentity) ? "��" : "";
				newRow["isPK"] = (col.IsPK) ? "��" : "";
				newRow["cisNull"] = (col.cisNull) ? "��" : "";
				newRow["defaultVal"] = col.DefaultVal;
				newRow["deText"] = col.DeText;
				cTable.Rows.Add(newRow);
			}
			return cTable;
		}

		#endregion

		#region  ��������Ϣ �õ��������б�

		/// <summary>
		/// �õ���������������б� (���磺����Exists  Delete  GetModel �Ĳ�������)
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		public static string GetInParameter(List<ColumnInfo> keys)
		{
			var strclass = new StringPlus();
			foreach (ColumnInfo key in keys)
			{
				strclass.Append(DbTypeToCS(key.TypeName) + " " + SetFirstCharacterLower(key.ColumnName) + ",");
			}
			strclass.DelLastComma();
			return strclass.Value;
		}
        public static string GetInParameterOfJava(List<ColumnInfo> keys)
        {
            var strclass = new StringPlus();
            foreach (ColumnInfo key in keys)
            {
                strclass.Append(DbTypeToJava(key.TypeName) + " " + SetFirstCharacterLower(key.ColumnName) + ",");
            }
            strclass.DelLastComma();
            return strclass.Value;
        }

		public static string SetFirstCharacterLower(string str)
		{
			return str[0].ToString(CultureInfo.InvariantCulture).ToLower() + str.Substring(1);
		}

        public static string SetFirstCharacterUpper(string str)
        {
            return str[0].ToString().ToUpper() + str.Substring(1);
        }

		public static string GetInterfaceParameter(string str)
		{
			return str[1].ToString(CultureInfo.InvariantCulture).ToLower() + str.Substring(2, str.Length - 2);
		}

		/// <summary>
		/// �ֶε� select �б�
		/// </summary>
		public static string GetFieldstrlist(List<ColumnInfo> keys)
		{
			var fields = new StringPlus();
			foreach (ColumnInfo obj in keys)
			{
				fields.Append(obj.ColumnName + ",");
			}
			fields.DelLastComma();
			return fields.Value;
		}

		public static string GetFieldStringListWithFirstLower(List<ColumnInfo> keys)
		{
			var fields = new StringPlus();
			foreach (ColumnInfo obj in keys)
			{
				fields.Append(SetFirstCharacterLower(obj.ColumnName) + ",");
			}
			fields.DelLastComma();
			return fields.Value;
		}
		/// <summary>
		/// �ֶε� select �б�
		/// </summary>
		public static string GetFieldstrlistAdd(List<ColumnInfo> keys)
		{
			var fields = new StringPlus();
			foreach (ColumnInfo obj in keys)
			{
				fields.Append(obj.ColumnName + "+");
			}
			fields.DelLastChar("+");
			return fields.Value;
		}

		/// <summary>
		/// �õ�Where������� - SQL��ʽ (���磺����Exists  Delete  GetModel ��where)
		/// </summary>
		public static string GetWhereExpression(List<ColumnInfo> keys)
		{
			var strclass = new StringPlus();
			foreach (ColumnInfo key in keys)
			{
				if (IsAddMark(key.TypeName))
				{
					strclass.Append(key.ColumnName + "='\"+" + key.ColumnName + "+\"' and ");
				}
				else
				{
					strclass.Append(key.ColumnName + "=\"+" + key.ColumnName + "+\" and ");
				}
			}
			strclass.DelLastChar("and");
			return strclass.Value;
		}

		/// <summary>
		/// �õ�Where������� - SQL��ʽ (���磺����Exists  Delete  GetModel ��where)
		/// </summary>
		public static string GetModelWhereExpression(List<ColumnInfo> keys)
		{
			var strclass = new StringPlus();
			foreach (ColumnInfo key in keys)
			{
				if (IsAddMark(key.TypeName))
				{
					strclass.Append(key.ColumnName + "='\"+ model." + key.ColumnName + "+\"' and ");
				}
				else
				{
					strclass.Append(key.ColumnName + "=\"+ model." + key.ColumnName + "+\" and ");
				}
			}
			strclass.DelLastChar("and");
			return strclass.Value;
		}

		#endregion
	}
}