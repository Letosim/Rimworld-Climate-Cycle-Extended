using UnityEngine;
using Verse;
using RimWorld;

namespace ClimateCycleExtended
{
    public class ClimateCycleExtendedModSettings : ModSettings
    {
        public float cyclePeriods = 5f;
        public float temperatureOffsetCold = 40f;
        public float temperatureOffsetWarm = 40f;
        public bool inverted = false;
        public GameConditionClimateCycleExtended gameCondition;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref this.cyclePeriods, "cyclePeriods", 5f);
            Scribe_Values.Look(ref this.inverted, "inverted", false);
            Scribe_Values.Look(ref this.temperatureOffsetCold, "temperatureOffsetCold", 40f);
            Scribe_Values.Look(ref this.temperatureOffsetWarm, "temperatureOffsetWarm", 40f);
        }
    }

    class ClimateCycleExtended : Mod
    {
        public ClimateCycleExtendedModSettings settings;
        public override string SettingsCategory() => "Climate Cycle Extended";

        public ClimateCycleExtended(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<ClimateCycleExtendedModSettings>();
            GameConditionClimateCycleExtended.settings = this.settings;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();

            if (!GenScene.InPlayScene)
                DoMainMenuSettings(inRect, listing);
            else
                DoInGameSettings(inRect, listing);
        }

        public void DoMainMenuSettings(Rect inRect, Listing_Standard listing)
        {

            int durationInDays = (int)settings.cyclePeriods * 60;
            int daysToHotPeak = settings.inverted ? durationInDays - durationInDays / 4 : durationInDays / 4;
            int daysToColdPeak = settings.inverted ? durationInDays / 4 : durationInDays - durationInDays / 4;

            listing.Begin(inRect);
            
            listing.AddLabeledNumericalTextField<float>("CCE_ModSettings_OffsetHot".Translate(), ref settings.temperatureOffsetWarm, minValue: 0f, maxValue: 250f);
            listing.AddLabeledNumericalTextField<float>("CCE_ModSettings_OffsetCold".Translate(), ref settings.temperatureOffsetCold, minValue: 0f, maxValue: 250f);
            listing.AddLabeledNumericalTextField<float>("CCE_ModSettings_CycleDuration".Translate(), ref settings.cyclePeriods, minValue: 1f, maxValue: 1000f);
            listing.CheckboxLabeled("CCE_ModSettings_InvertCycle".Translate(), ref settings.inverted, "CCE_ModSettings_InvertCycle_ToolTip".Translate());
            listing.Label("CCE_ModSettings_InfoTextA".Translate() + settings.cyclePeriods + "CCE_ModSettings_InfoTextB".Translate() + durationInDays + "CCE_ModSettings_InfoTextC".Translate() + daysToHotPeak + "CCE_ModSettings_InfoTextD".Translate() + daysToColdPeak + "CCE_ModSettings_InfoTextE".Translate());

            listing.End();

            settings.Write();
        }

        public void DoInGameSettings(Rect inRect, Listing_Standard listing)
        {
            if (settings.gameCondition == null)
            {
                DoMainMenuSettings(inRect, listing);
                return;
            }

            listing.Begin(inRect);

            listing.Label("CCE_ModSettings_InGame_OffsetHot".Translate() + (int)settings.gameCondition.temperatureOffsetWarmCurrentSave);
            listing.Label("CCE_ModSettings_InGame_OffsetCold".Translate() + (int)settings.gameCondition.temperatureOffsetColdCurrentSave);
            listing.Label("CCE_ModSettings_InGame_CycleDuration".Translate() + (int)settings.gameCondition.cycleLength);

            listing.End();
        }
    }
}
