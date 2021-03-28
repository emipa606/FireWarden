using Verse;
using Verse.AI;

namespace PelFireWarden
{
    // Token: 0x0200001C RID: 28
    public class JobGiver_FWEquipFW : ThinkNode_JobGiver
    {
        // Token: 0x04000037 RID: 55
        private static readonly JobGiver_equipnewFW equipFW = new();

        // Token: 0x0600006D RID: 109 RVA: 0x00004A74 File Offset: 0x00002C74
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
}