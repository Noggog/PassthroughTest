using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;

namespace PassthroughTest
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "PassthroughTest.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            Console.WriteLine($"{state.LoadOrder.Count()} mods in the load order");
            int count = 0;
            foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
            {
                Console.WriteLine($"Copying {npcContext.Record} from {npcContext.ModKey}.");
                state.PatchMod.Npcs.GetOrAddAsOverride(npcContext.Record);
                count++;
            }
            Console.WriteLine($"Passed through {count} npcs");
            
            count = 0;
            foreach (var bookContext in state.LoadOrder.PriorityOrder.Book().WinningContextOverrides())
            {
                Console.WriteLine($"Copying {bookContext.Record} from {bookContext.ModKey}.");
                state.PatchMod.Books.GetOrAddAsOverride(bookContext.Record);
                count++;
            }
            Console.WriteLine($"Passed through {count} books");
        }
    }
}
