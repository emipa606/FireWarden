using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobGiver_FWEquipFW : ThinkNode_JobGiver
{
    private static readonly JobGiver_equipnewFW equipFW = new();

    protected override Job TryGiveJob(Pawn pawn)
    {
        var thinkResult = equipFW.TryIssueJobPackage(pawn, default);

        return thinkResult.IsValid ? thinkResult.Job : null;
    }
}