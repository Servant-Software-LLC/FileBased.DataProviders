namespace System.Data.JsonClient
{
    public class JsonDataReader : IDataReader
    {
        private readonly JsonReader _reader;
        private DataRow _currentObjectEnumerator;

        public JsonDataReader(JsonCommand command, IDbConnection jsonConnection)
        {
            _reader = new JsonReader(command.QueryParser,
                ((JsonConnection)jsonConnection).GetDatabase());
        }

        public int Depth => 0;
        public bool IsClosed => _reader == null;
        public int RecordsAffected => -1;

        public void Close()
        {
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            if (_reader.MoveNext())
            {
                _currentObjectEnumerator = _reader.Current;
                return true;
            }

            return false;
        }

        public int FieldCount => _reader.FieldCount;

        public bool GetBoolean(int i)
        {
            return GetValueAsType<bool>(i);
        }

        public byte GetByte(int i)
        {
            return GetValueAsType<byte>(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return GetValueAsType<long>(i);

        }

        public char GetChar(int i)
        {
            return GetValueAsType<char>(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return GetValueAsType<long>(i);

        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return GetValueAsType<string>(i).GetType().Name;
        }

        public DateTime GetDateTime(int i)
        {
            return GetValueAsType<DateTime>(i);
        }

        public decimal GetDecimal(int i)
        {
            return GetValueAsType<decimal>(i);
        }

        public double GetDouble(int i)
        {
            return GetValueAsType<double>(i);
        }

        public Type GetFieldType(int i)
        {
            return _reader.GetType(i);
        }

        public float GetFloat(int i)
        {
            return GetValueAsType<float>(i);
        }

        public Guid GetGuid(int i)
        {
            return GetValueAsType<Guid>(i);
        }

        public short GetInt16(int i)
        {
            return GetValueAsType<short>(i);
        }

        public int GetInt32(int i)
        {
            return GetValueAsType<int>(i);
        }

        public long GetInt64(int i)
        {
            return GetValueAsType<long>(i);
        }

        public string GetName(int i)
        {
            return _reader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return _reader.GetOrdinal(name);
        }
        public string GetString(int i)
        {
            return GetValueAsType<string>(i);
        }
        public object GetValue(int i)
        {
            return _currentObjectEnumerator[i];
        }
        public int GetValues(object[] values)
        {
            int count = Math.Min(values.Length, FieldCount);

            for (int i = 0; i < count; i++)
            {
                values[i] = GetValue(i);
            }

            return count;
        }
        public bool IsDBNull(int i)
        {
            return _currentObjectEnumerator.IsNull(i);
        }
        public void Dispose()
        {
            _reader.Dispose();
        }
        public object this[string name]
        {
            get
            {
                int ordinal = GetOrdinal(name);
                return GetValue(ordinal);
            }
        }
        public T GetValueAsType<T>(int index)
        {
            return (T)Convert.ChangeType(_currentObjectEnumerator[index], typeof(T));
        }


        public object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }
    }

}