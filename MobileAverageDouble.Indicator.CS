using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{

	[SameAsSymbol(true)]
	public class MobileAverageDouble : IndicatorObject {

		[Input] public int Length { get; set;}
		[Input] public int Displace { get; set;}
		[Input] public bool BreakSignal { get; set;}

		private ISeries<double> Price {get; set;}

		private Clue clue;
		private VariableSeries<double> averageDouble;
		private IPlotObject plot1;
		private IPlotObjectStr plotStr1;

		private string marketStage = "NO_STAGE_DEFINED";
		private Color cellColor = Color.AliceBlue;
		private bool risingTrend = true;
		private bool fallingTrend = false;

		enum MarketStages{
			Side, Rising, Falling
		}

		public MobileAverageDouble(object _ctx):base(_ctx){
			Length = 30;
			BreakSignal = false;
		}

		protected override void Create() {
			clue = new Clue(this);
            averageDouble  = new VariableSeries<double>(this);
			plot1 =  AddPlot(new PlotAttributes("AvgDouble", 0, Color.Blue,Color.Empty, 0, 0, true));
			plotStr1 =  AddPlot(new StringPlotAttributes("Trend", Color.Aqua));
		}

		protected override void StartCalc() {
			clue.Price = Bars.Close;
			clue.Length = Length;
		}

		protected override void CalcBar(){
			averageDouble.Value =  clue[0];
			if (((Displace >= 0) || Bars.CurrentBar > Math.Abs(Displace))){
               	 plot1.Set(Displace, averageDouble.Value);
            	}

			if(BreakSignal){
				if(clue[0] >= Bars.Close[0]){
					if(!marketStage.Equals("Falling")){
						marketStage = "Falling";
						Alerts.Alert("Get ready for the falling trend start");
					}
					cellColor = Color.Red;
				}
				else if(clue[0] <= Bars.Close[0]){
					if(!marketStage.Equals("Rising")){
						marketStage = "Rising";	
						Alerts.Alert("Get ready for the rising trend start");
					}
					cellColor = Color.Green;
				}		
			}
	
			plotStr1.Set(marketStage, cellColor);
		}
	}
}