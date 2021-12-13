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
            Console.WriteLine($"{state.LoadOrder.Count()} mods in the load order:");
            foreach (var listing in state.LoadOrder)
            {
                Console.WriteLine($"   {listing.Value}");
            }
            Console.WriteLine();

            foreach (var listing in state.LoadOrder)
            {
                if (!listing.Key.Name.Contains("Bashed") || listing.Value.Mod == null) continue;
                Console.WriteLine($"Printing NPCs for {listing.Key}");
                foreach (var npc in listing.Value.Mod.Npcs)
                {
                    Console.WriteLine($"{listing.Key} {npc} had {npc.HeadParts.Count} head parts.");
                }
            }
            Console.WriteLine();

            int count = 0;
            foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
            {
                Console.WriteLine($"Copying {npcContext.Record} from {npcContext.ModKey}.  Had {npcContext.Record.HeadParts.Count} head parts.");
                state.PatchMod.Npcs.GetOrAddAsOverride(npcContext.Record);
                count++;
            }
            Console.WriteLine($"Passed through {count} npcs");
        }
    }
}
