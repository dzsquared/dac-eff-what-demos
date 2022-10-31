using Microsoft.SqlServer.Dac.Compare;

namespace SqlProj.SchemaCompare
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // first arg is previous dacpac
            var previousDacpac = args[0]; // "AdventureWorksLT2.dacpac"
            var previousEndpoint = new SchemaCompareDacpacEndpoint(previousDacpac);
            
            // second arg is current dacpac
            var currentDacpac = args[1]; // "AdventureWorksLT.dacpac"
            var currentEndpoint = new SchemaCompareDacpacEndpoint(currentDacpac);
            
            Console.WriteLine("Comparing dacpacs {0} and {1}", previousDacpac, currentDacpac);

            // setup schema comparison
            var schemaCompare = new SchemaComparison(previousEndpoint, currentEndpoint);
            SchemaComparisonResult schemaDifference = schemaCompare.Compare();

            if (schemaDifference.IsValid && !schemaDifference.IsEqual)
            {
                // comparison worked and has differences.
                Console.WriteLine("Schema comparison found differences");

                SqlChangeset changeset = populateChangeset(schemaDifference);
                PrintChangeset(changeset);

            }
            else
            {
                Console.WriteLine("No differences found.");
            }
        }
    
        private static SqlChangeset populateChangeset(SchemaComparisonResult schemaDifference)
        {
            var changeset = new SqlChangeset();
            foreach (var difference in schemaDifference.Differences)
            {
                if (difference.DifferenceType == SchemaDifferenceType.Object)
                {
                    // check for table
                    if (difference.SourceObject != null && difference.SourceObject.ObjectType.Name == "Table" )
                    {
                        var changedTable = new ChangedTable(difference.SourceObject.Name.ToString(), difference.UpdateAction);

                        // add columns to changed table
                        foreach (var child in difference.Children)
                        {
                            if (child.DifferenceType == SchemaDifferenceType.Object)
                            {
                                string objectType = "";
                                if (child.SourceObject != null)
                                {
                                    objectType = child.SourceObject.ObjectType.Name;
                                }
                                else if (child.TargetObject != null)
                                {
                                    objectType = child.TargetObject.ObjectType.Name;
                                }

                                if (objectType == "Column")
                                {
                                    if (child.UpdateAction == SchemaUpdateAction.Add)
                                    {
                                        changedTable.AddedColumns.Add(child.SourceObject.Name.ToString());
                                    }
                                    else if (child.UpdateAction == SchemaUpdateAction.Delete)
                                    {
                                        changedTable.DroppedColumns.Add(child.TargetObject.Name.ToString());
                                    }
                                    else if (child.UpdateAction == SchemaUpdateAction.Change)
                                    {
                                        changedTable.ModifiedColumns.Add(child.SourceObject.Name.ToString());
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("another type found");
                                }
                            }
                        }

                        changeset.ChangedTables.Add(changedTable);
                    }
                }
            }
            return changeset;
        }

        private static void PrintChangeset(SqlChangeset changeset)
        {
            foreach (var changedTable in changeset.ChangedTables)
            {
                Console.WriteLine("Table {0} has update action {1}", changedTable.Name, changedTable.UpdateAction);

                foreach (string columnname in changedTable.AddedColumns)
                {
                    Console.WriteLine("Column added: {0}", columnname);
                }
                foreach (string columnname in changedTable.ModifiedColumns)
                {
                    Console.WriteLine("Column modified: {0}", columnname);
                }
                foreach (string columnname in changedTable.DroppedColumns)
                {
                    Console.WriteLine("Column dropped: {0}", columnname);
                }
            }
        }

    }
}