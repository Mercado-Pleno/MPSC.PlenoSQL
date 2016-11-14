﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao.AF.Fake
{
	public class DataReaderFake : IDataReader
	{
		private Int32 _registro = -1;
		private readonly List<Dictionary<String, Object>> tabela = new List<Dictionary<String, Object>>();
		public DataReaderFake(params Dictionary<String, Object>[] itens)
		{
			tabela.AddRange(itens);
		}

		void IDataReader.Close() { }
		Int32 IDataReader.Depth { get { return 0; } }
		DataTable IDataReader.GetSchemaTable() { return null; }
		Boolean IDataReader.IsClosed { get { return false; } }
		Boolean IDataReader.NextResult() { return false; }
		Boolean IDataReader.Read() { return ((_registro < (tabela.Count - 1)) ? ++_registro : -1) >= 0; }
		Int32 IDataReader.RecordsAffected { get { return tabela.Count; } }
		void IDisposable.Dispose() { }
		Int32 IDataRecord.FieldCount { get { return tabela[_registro].Count; } }
		Boolean IDataRecord.GetBoolean(Int32 i) { return Convert.ToBoolean(getValue(i)); }
		Byte IDataRecord.GetByte(Int32 i) { return Convert.ToByte(getValue(i)); }
		Int64 IDataRecord.GetBytes(Int32 i, Int64 fieldOffset, Byte[] buffer, Int32 bufferOffset, Int32 length) { return Convert.ToInt64(getValue(i)); }
		Char IDataRecord.GetChar(Int32 i) { return Convert.ToChar(getValue(i)); }
		Int64 IDataRecord.GetChars(Int32 i, Int64 fieldOffset, char[] buffer, Int32 bufferOffset, Int32 length) { return Convert.ToInt64(getValue(i)); }
		IDataReader IDataRecord.GetData(Int32 i) { return this; }
		String IDataRecord.GetDataTypeName(Int32 i) { return getValue(i).GetType().Name; }
		DateTime IDataRecord.GetDateTime(Int32 i) { return Convert.ToDateTime(getValue(i)); }
		Decimal IDataRecord.GetDecimal(Int32 i) { return Convert.ToDecimal(getValue(i)); }
		Double IDataRecord.GetDouble(Int32 i) { return Convert.ToDouble(getValue(i)); }
		Type IDataRecord.GetFieldType(Int32 i) { return getValue(i).GetType(); }
		Single IDataRecord.GetFloat(Int32 i) { return Convert.ToSingle(getValue(i)); }
		Guid IDataRecord.GetGuid(Int32 i) { return (Guid)getValue(i); }
		Int16 IDataRecord.GetInt16(Int32 i) { return Convert.ToInt16(getValue(i)); }
		Int32 IDataRecord.GetInt32(Int32 i) { return Convert.ToInt32(getValue(i)); }
		Int64 IDataRecord.GetInt64(Int32 i) { return Convert.ToInt64(getValue(i)); }
		String IDataRecord.GetName(Int32 i) { return tabela[_registro].Keys.Skip(i).FirstOrDefault(); }
		Int32 IDataRecord.GetOrdinal(String name) { var o = -1; return tabela[_registro].Keys.SkipWhile((k, i) => { o = i; return k != name; }).FirstOrDefault() == name ? o : -1; }
		String IDataRecord.GetString(Int32 i) { return Convert.ToString(getValue(i)); }
		Object IDataRecord.GetValue(Int32 i) { return getValue(i); }
		Int32 IDataRecord.GetValues(Object[] values) { return 0; }
		Boolean IDataRecord.IsDBNull(Int32 i) { return getValue(i) == null; }
		Object IDataRecord.this[String name] { get { return tabela[_registro][name]; } }
		Object IDataRecord.this[Int32 i] { get { return getValue(i); } }
		private Object getValue(Int32 i) { return tabela[_registro].Values.Skip(i).FirstOrDefault(); }
	}
}