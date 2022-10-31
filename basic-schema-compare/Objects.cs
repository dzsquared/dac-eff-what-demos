using Microsoft.SqlServer.Dac.Compare;

namespace SqlProj.SchemaCompare
{
    public class SqlChangeset
    {
        public List<ChangedTable> ChangedTables { get; set; }

        public SqlChangeset()
        {
            ChangedTables = new List<ChangedTable>();
        }

    }

    public class ChangedTable
    {
        public string Name { get; set; }
        public SchemaUpdateAction UpdateAction { get; set; }
        public string UpdateScript { get; set; }
        public List<string> AddedColumns { get; set; }
        public List<string> DroppedColumns { get; set; }
        public List<string> ModifiedColumns { get; set; }

        public ChangedTable(string name, SchemaUpdateAction updateAction)
        {
            Name = name;
            UpdateAction = updateAction;
            UpdateScript = "";
            AddedColumns = new List<string>();
            DroppedColumns = new List<string>();
            ModifiedColumns = new List<string>();
        }
    }
}