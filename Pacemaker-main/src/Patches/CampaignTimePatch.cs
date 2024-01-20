using Pacemaker.Extensions;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
    [HarmonyDebug]
    [HarmonyPatch(typeof(CampaignTime))]
    internal static class CampaignTimePatch
    {
        private delegate long CurrentTicksDelegate();
        private static readonly Reflect.DeclaredGetter<CampaignTime> CurrentTicksRG = new("CurrentTicks");
        private static readonly CurrentTicksDelegate CurrentTicks = CurrentTicksRG.GetDelegate<CurrentTicksDelegate>();

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Elapsed[UNIT]sUntilNow

        [HarmonyPrefix]
        [HarmonyPatch("ElapsedSeasonsUntilNow", MethodType.Getter)]
        public static bool ElapsedSeasonsUntilNow(ref float __result, ref long ____numTicks)
        {
            __result = (CurrentTicks() - ____numTicks) / Main.TimeParam.TickPerSeasonF;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ElapsedYearsUntilNow", MethodType.Getter)]
        public static bool ElapsedYearsUntilNow(ref float __result, ref long ____numTicks)
        {
            __result = (CurrentTicks() - ____numTicks) / Main.TimeParam.TickPerYearF;
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Remaining[UNIT]sFromNow

        [HarmonyPrefix]
        [HarmonyPatch("RemainingSeasonsFromNow", MethodType.Getter)]
        public static bool RemainingSeasonsFromNow(ref float __result, ref long ____numTicks)
        {
            __result = (____numTicks - CurrentTicks()) / Main.TimeParam.TickPerSeasonF;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("RemainingYearsFromNow", MethodType.Getter)]
        public static bool RemainingYearsFromNow(ref float __result, ref long ____numTicks)
        {
            __result = (____numTicks - CurrentTicks()) / Main.TimeParam.TickPerYearF;
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // To[UNIT]s

        [HarmonyPrefix]
        [HarmonyPatch("ToSeasons", MethodType.Getter)]
        public static bool ToSeasons(ref double __result, ref long ____numTicks)
        {
            __result = ____numTicks / Main.TimeParam.TickPerSeasonD;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ToYears", MethodType.Getter)]
        public static bool ToYears(ref double __result, ref long ____numTicks)
        {
            __result = ____numTicks / Main.TimeParam.TickPerYearD;
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Get[UNIT]Of[UNIT]

        [HarmonyPrefix]
        [HarmonyPatch("GetDayOfSeason", MethodType.Getter)]
        public static bool GetDayOfSeason(ref int __result, ref long ____numTicks)
        {
            __result = (int)((____numTicks / TimeParams.TickPerDayL) % Main.TimeParam.DayPerSeason);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetDayOfYear", MethodType.Getter)]
        public static bool GetDayOfYear(ref int __result, ref long ____numTicks)
        {
            __result = (int)((____numTicks / TimeParams.TickPerDayL) % Main.TimeParam.DayPerYear);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetYear", MethodType.Getter)]
        public static bool GetYear(ref int __result, ref long ____numTicks)
        {
            __result = (int)(____numTicks / Main.TimeParam.TickPerYearL);
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // [UNIT]s (factory methods)

        [HarmonyPrefix]
        [HarmonyPatch("Years")]
        public static bool Years(float valueInYears, ref CampaignTime __result)
        {
            __result = CampaignTimeExtensions.Ticks((long)(valueInYears * Main.TimeParam.TickPerYearF));
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // [UNIT]sFromNow (factory methods)

        [HarmonyPrefix]
        [HarmonyPatch("YearsFromNow")]
        public static bool YearsFromNow(float valueInYears, ref CampaignTime __result)
        {
            __result = CampaignTimeExtensions.Ticks(CurrentTicks() + (long)(valueInYears * Main.TimeParam.TickPerYearF));
            return false;
        }

        [HarmonyPatch(typeof(CampaignTime), "GetSeasonOfYear", MethodType.Getter)]
        public static class GetSeasonOfYearPatch
        {
            public static bool Prefix(ref CampaignTime.Seasons __result)
            {
                // Calculate the current season based on the current ticks.
                long currentTicks = CurrentTicks();
                long ticksPerSeason = Main.TimeParam.TickPerSeasonL;
                int currentSeason = (int)(currentTicks / ticksPerSeason) % 4;

                // Assign the calculated season to the __result variable.
                __result = (CampaignTime.Seasons)currentSeason;

                return false; // Return false to skip the original method.
            }
        }
    }
}
