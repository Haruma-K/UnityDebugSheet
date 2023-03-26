using System.Text;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public sealed class Physics2DDebugPage : PropertyListDebugPageBase<Physics2D>
    {
        protected override string Title => "Physics 2D";

        protected override bool TryGetOverridePropertyDescription(string propertyName, object value,
            out string description)
        {
            if (propertyName == nameof(Physics2D.jobOptions))
            {
                var lastData = (PhysicsJobOptions2D)value;
                description = JobOptionsToString(lastData);
                return true;
            }

            description = null;
            return false;
        }

        private string JobOptionsToString(PhysicsJobOptions2D jobOptions)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{ ");
            stringBuilder.Append($"{nameof(PhysicsJobOptions2D.useMultithreading)}: {jobOptions.useMultithreading}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.useConsistencySorting)}: {jobOptions.useConsistencySorting}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.interpolationPosesPerJob)}: {jobOptions.interpolationPosesPerJob}");
            stringBuilder.Append($", {nameof(PhysicsJobOptions2D.newContactsPerJob)}: {jobOptions.newContactsPerJob}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.collideContactsPerJob)}: {jobOptions.collideContactsPerJob}");
            stringBuilder.Append($", {nameof(PhysicsJobOptions2D.clearFlagsPerJob)}: {jobOptions.clearFlagsPerJob}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.clearBodyForcesPerJob)}: {jobOptions.clearBodyForcesPerJob}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.syncDiscreteFixturesPerJob)}: {jobOptions.syncDiscreteFixturesPerJob}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.syncContinuousFixturesPerJob)}: {jobOptions.syncContinuousFixturesPerJob}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.findNearestContactsPerJob)}: {jobOptions.findNearestContactsPerJob}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.updateTriggerContactsPerJob)}: {jobOptions.updateTriggerContactsPerJob}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.islandSolverCostThreshold)}: {jobOptions.islandSolverCostThreshold}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.islandSolverBodyCostScale)}: {jobOptions.islandSolverBodyCostScale}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.islandSolverContactCostScale)}: {jobOptions.islandSolverContactCostScale}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.islandSolverJointCostScale)}: {jobOptions.islandSolverJointCostScale}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.islandSolverBodiesPerJob)}: {jobOptions.islandSolverBodiesPerJob}");
            stringBuilder.Append(
                $", {nameof(PhysicsJobOptions2D.islandSolverContactsPerJob)}: {jobOptions.islandSolverContactsPerJob}");
            stringBuilder.Append(" }");
            return stringBuilder.ToString();
        }
    }
}
