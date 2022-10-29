using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobGiver_FWEquipFW : ThinkNode_JobGiver
{
    private static readonly JobGiver_equipnewFW equipFW = new JobGiver_equipnewFW();

    protected override Job TryGiveJob(Pawn pawn)
    {
        var thinkResult = equipFW.TryIssueJobPackage(pawn, default);
        Job result;
        if (thinkResult.IsValid)
        {
            result = thinkResult.Job;
        }
        else
        {
            result = null;
        }

        return result;
    }
}