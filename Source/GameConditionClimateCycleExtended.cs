using RimWorld;
using UnityEngine;
using Verse;

namespace ClimateCycleExtended 
{
	public class GameConditionClimateCycleExtended : GameCondition
	{
		public static ClimateCycleExtendedModSettings settings;
        public override string TooltipString{ get {return toolTip;}}
        private string toolTip = "";

        public bool cycleInverted = false;
        public int currentPeriod = -1;//0 normal 1 hot 2 cold
        public int daysTillCurrentPauseEnds = 0;
        public int timesShift = 0;
        public float currentDay = 0;
        public float cycleLength = 0;

        public float temperature = 0;
        public float temperatureOffsetColdCurrentSave;
        public float temperatureOffsetWarmCurrentSave;

        public override void Init()
        {
            base.Init();

            if (cycleLength == 0)
            {
                cycleLength = settings.cyclePeriods;
                temperatureOffsetColdCurrentSave = settings.temperatureOffsetCold;
                temperatureOffsetWarmCurrentSave = settings.temperatureOffsetWarm;
                currentDay = GenDate.DaysPassed;
                cycleInverted = settings.inverted;
                if (currentDay == 0)
                    currentPeriod = 0;
                UpdateGameCondition(false);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref this.daysTillCurrentPauseEnds, "daysTillCurrentPauseEnds", 0, true);
            Scribe_Values.Look(ref this.timesShift, "timesShift", 0, true);
            Scribe_Values.Look(ref this.temperatureOffsetColdCurrentSave, "temperatureOffsetColdCurrentSave", settings.temperatureOffsetCold, true);
            Scribe_Values.Look(ref this.temperatureOffsetWarmCurrentSave, "temperatureOffsetWarmCurrentSave", settings.temperatureOffsetWarm, true);
            Scribe_Values.Look(ref this.cycleLength, "cycleLength", settings.cyclePeriods, true);
            Scribe_Values.Look(ref this.currentPeriod, "currentPeriod", -1, true);
            Scribe_Values.Look(ref this.currentDay, "currentDay",(float)(timesShift + GenDate.DaysPassed), true);
            Scribe_Values.Look(ref this.cycleInverted, "cycleInverted", settings.inverted, true);

            UpdateGameCondition(false);
        }

        public override float TemperatureOffset()
        {
            if((int)currentDay != timesShift + GenDate.DaysPassed)
            {
                UpdateGameCondition(true);
                currentDay = (float)(timesShift + GenDate.DaysPassed);
            }

            return temperature;
        }


        public void UpdateGameCondition(bool endOfDay)
        {
            temperature = Mathf.Sin((currentDay / (cycleLength * (float)GenDate.DaysPerYear) - Mathf.Floor(currentDay / (cycleLength * (float)GenDate.DaysPerYear))) * 6.28f);
            temperature = cycleInverted ? temperature : temperature *= -1f;
            temperature = daysTillCurrentPauseEnds == 0 ? temperature : Mathf.Round(temperature);

            if (endOfDay) 
                UpdatePeriodSystem(temperature);

            temperature = temperature < 0 ? temperature * temperatureOffsetColdCurrentSave : temperature * temperatureOffsetWarmCurrentSave;
            
            toolTip = "CCE_GameCondition_ToolTip".Translate() + (int)temperature + "°";
        }


        public void UpdatePeriodSystem(float curve)
        {
            int period = currentPeriod;

            if(curve < -0.95f)
                period = 2;
            else 
                if(curve > 0.95f)
                    period = 1;
                else 
                    if(curve < 0.025f && curve > -0.025f)
                        period = 0;

            if(currentPeriod != period)
            {
                float durationClamped = Mathf.Clamp((cycleLength * (float)GenDate.DaysPerYear), 0, 6000);
                int minDaysPause = (int)Mathf.Lerp(6f, 30f, currentDay / durationClamped);//clamped clamps
                int maxDaysPause = (int)Mathf.Lerp(12f, 60f, currentDay / durationClamped);

                daysTillCurrentPauseEnds = Random.Range(minDaysPause, maxDaysPause);
                daysTillCurrentPauseEnds = period != 0 ? daysTillCurrentPauseEnds : daysTillCurrentPauseEnds / 4;

                currentPeriod = period;
            }

            if(daysTillCurrentPauseEnds != 0)
            {
                daysTillCurrentPauseEnds --;
                timesShift--;
            }
        }
    }
}