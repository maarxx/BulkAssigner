using System.Reflection;
using HarmonyLib;
using Verse;

namespace BulkAssigner;

[StaticConstructorOnStartup]
internal class Main
{
    static Main()
    {
        var harmony = new Harmony("com.github.harmony.rimworld.maarx.bulkassigner");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}