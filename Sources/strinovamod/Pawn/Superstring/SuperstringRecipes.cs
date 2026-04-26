using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Strinova
{
    public class RecipeWorker_SpawnCelestia : RecipeWorker
    {
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            base.Notify_IterationCompleted(billDoer, ingredients);

            PawnKindDef kindDef = DefDatabase<PawnKindDef>.GetNamed("Celestia");
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(
                kindDef,
                Faction.OfPlayer,
                PawnGenerationContext.NonPlayer,
                forceGenerateNewPawn: true,
                canGeneratePawnRelations: true
            ));

            Map map = billDoer.Map;
            IntVec3 spawnPos;
            if (!CellFinder.TryFindRandomCellNear(
                    billDoer.Position,
                    map, 3,
                    c => c.Standable(map) && !c.Fogged(map),
                    out spawnPos))
            {
                spawnPos = billDoer.Position;
            }
            pawn.Name = new NameSingle("星绘");
            GenSpawn.Spawn(pawn, spawnPos, map);

            Find.LetterStack.ReceiveLetter(
                "星绘 加入了殖民地",
                $"{pawn.Name.ToStringShort}已从晶核打印机中诞生，准备好加入你的殖民地了。",
                LetterDefOf.PositiveEvent,
                pawn
            );
        }
    }
}
