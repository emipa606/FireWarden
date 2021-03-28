using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
    // Token: 0x02000020 RID: 32
    public class ThinkNode_ConditionalFWPawnHasClarity : ThinkNode_Conditional
    {
        // Token: 0x06000077 RID: 119 RVA: 0x0000504F File Offset: 0x0000324F
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.IsColonistPlayerControlled && !pawn.Downed && !pawn.IsBurning() && !pawn.InMentalState &&
                   !pawn.Drafted && pawn.Awake() && !HealthAIUtility.ShouldSeekMedicalRest(pawn);
        }
    }
}