namespace NotesApp1.Services
{
    public class ForeignKey
    {
        public string ColumnName { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedColumnName { get; set; }
    }
}
