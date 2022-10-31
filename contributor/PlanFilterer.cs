using Microsoft.SqlServer.Dac.Deployment;
using Microsoft.SqlServer.Dac.Model;

namespace Azure.Samples.PlanFilterer
{
    // deployment plan modifier that filters out steps based on 1 or more names
    // objectnames are populated from the argument ObjectName
    [ExportDeploymentPlanModifier(PlanFilterer.PlanFiltererContributorId, "1.0.0.0")]
    public class PlanFilterer : DeploymentPlanModifier
    {
        public const string PlanFiltererContributorId = "Azure.Samples.PlanFilterer";
        private HashSet<string> _objectNames;

        protected override void OnExecute(DeploymentPlanContributorContext context)
        {
            // setup object filter with list from arguments
            InitializeFilter(context.Arguments);
            // p:AdditionalDeploymentContributorArguments="ObjectName=audits"

            DeploymentStep next = context.PlanHandle.Head;
            while (next != null)
            {
                DeploymentStep current = next;
                next = current.Next;
                Type stepType = current.GetType();

                if (stepType == typeof(CreateElementStep))
                {
                    CreateElementStep createStep = current as CreateElementStep;
                    if (ShouldFilter(createStep))
                    {
                        base.Remove(context.PlanHandle, createStep);
                    }
                }
                if (stepType == typeof(AlterElementStep))
                {
                    AlterElementStep alterStep = current as AlterElementStep;
                    if (ShouldFilter(alterStep))
                    {
                        base.Remove(context.PlanHandle, alterStep);
                    }
                }
                if (stepType == typeof(DropElementStep))
                {
                    DropElementStep dropStep = current as DropElementStep;
                    if (ShouldFilter(dropStep))
                    {
                        base.Remove(context.PlanHandle, dropStep);
                    }
                }
                if (stepType == typeof(SqlRenameStep))
                {
                    SqlRenameStep renameStep = current as SqlRenameStep;
                    if (ShouldFilter(renameStep))
                    {
                        base.Remove(context.PlanHandle, renameStep);
                    }
                }
            }
        }

        // returns true if the element being created should be filtered out
        private bool ShouldFilter(CreateElementStep createStep)
        {
            TSqlObject createdObject = createStep.SourceElement;
            return CheckFilterObject(createdObject);
        }

        // returns true if the element that would be changed should be filtered out
        // based on original name or new name
        private bool ShouldFilter(AlterElementStep alterStep)
        {
            TSqlObject sourceObject = alterStep.SourceElement;
            TSqlObject targetObject = alterStep.TargetElement;
            return (CheckFilterObject(sourceObject) || CheckFilterObject(targetObject));
        }

        // returns true if the element that would be dropped should be filtered out
        private bool ShouldFilter(DropElementStep dropStep)
        {
            TSqlObject droppedObject = dropStep.TargetElement;
            return CheckFilterObject(droppedObject);
        }

        // returns true if the element that would be renamed should be filtered out
        private bool ShouldFilter(SqlRenameStep renameStep)
        {
            TSqlObject renamedObject = renameStep.RenamedElement;
            return CheckFilterObject(renamedObject);
        }

        // returns true if the step with this object should be filtered out
        private bool CheckFilterObject(TSqlObject tsqlObject)
        {
            bool objectMatches = false;
            ObjectIdentifier id = tsqlObject.Name;

            if (id.HasName)
            {
                if (id.Parts.Count == 1) // 1-part name (eg table)
                {
                    objectMatches = _objectNames.Contains(id.Parts[0], StringComparer.OrdinalIgnoreCase);
                }
                else if (id.Parts.Count >= 2) // 2 or more part name (eg schema.table)
                {
                    objectMatches = _objectNames.Contains(id.Parts[0], StringComparer.OrdinalIgnoreCase) || _objectNames.Contains(id.Parts[1], StringComparer.OrdinalIgnoreCase);
                }
            }

            return objectMatches;
        }

        // setting up the object filter from the arguments
        private void InitializeFilter(Dictionary<string, string> arguments)
        {
            IEnumerable<string> objectNamesArg = arguments
                // can be ObjectName= or ObjectName1=something;ObjectName2=somethingelse
                .Where(pair => pair.Key.StartsWith("ObjectName"))
                .Select(pair => pair.Value);
            // populate filter list
            _objectNames = new HashSet<string>(objectNamesArg);
        }
    }
}