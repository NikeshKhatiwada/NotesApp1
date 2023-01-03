using MySqlX.XDevAPI.Relational;

namespace NotesApp1.Services
{
    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; }
    }
}
