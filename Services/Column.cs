using MySql.Data.MySqlClient;

namespace NotesApp1.Services
{
    public class Column
    {
        public string Name { get; set; }
        public MySqlDbType Type { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsNullable { get; set; }
    }
}
}
