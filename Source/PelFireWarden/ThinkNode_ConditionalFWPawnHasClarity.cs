using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class ThinkNode_ConditionalFWPawnHasClarity : ThinkNode_Conditional
{
    protected override bool Satisfied(Pawn pawn)
    {
        return pawn.IsColonistPlayerControlled && !pawn.Downed && !pawn.IsBurning() && !pawn.InMentalState &&
               !pawn.Drafted && pawn.Awake() && !HealthAIUtility.ShouldSeekMedicalRest(pawn);
    }
}