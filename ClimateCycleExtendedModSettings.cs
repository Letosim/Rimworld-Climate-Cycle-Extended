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
            Listing_Standard listing_Standard = new Listing_Standard();

            listing_Standard.Begin(inRect);
            
            listing_Standard.AddLabeledNumericalTextField<float>("CCE_ModSettings_OffsetHot".Translate(), ref settings.temperatureOffsetWarm, minValue: 0f, maxValue: 250f);
            listing_Standard.AddLabeledNumericalTextField<float>("CCE_ModSettings_OffsetCold".Translate(), ref settings.temperatureOffsetCold, minValue: 0f, maxValue: 250f);
            listing_Standard.AddLabeledNumericalTextField<float>("CCE_ModSettings_CycleDuration".Translate(), ref settings.cyclePeriods, minValue: 1f, maxValue: 1000f);
            listing_Standard.CheckboxLabeled("CCE_ModSettings_InvertCycle".Translate(), ref settings.inverted, "CCE_ModSettings_InvertCycle_ToolTip".Translate());

            int durationInDays = (int)settings.cyclePeriods * 60;
            int daysToHotPeak = settings.inverted ? durationInDays - durationInDays / 4 : durationInDays / 4;
            int daysToColdPeak = settings.inverted ?durationInDays / 4 : durationInDays - durationInDays / 4;
            listing_Standard.Label("The cycle will last " + settings.cyclePeriods + " years (" + durationInDays + "days). The first hot peak will reach its limit after " + daysToHotPeak + " days. And the first cold peak after " + daysToColdPeak + " days.");
            listing_Standard.End();
            //"The cycle will last " + + " years (" + + "days). The first hot peak will reach its limit after " + daysToHotPeak + " days. And the first cold peak after " + daysToColdPeak + " days."
            settings.Write();
        }
    }
}
