using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

//C:\ProgramData\TS Support\MultiCharts .NET64 Special Edition\StudyServer\Techniques\CS
namespace PowerLanguage.Indicator{
	public class TrendPower : IndicatorObject {
		[Input] public int fastLength {get; set;}
		[Input] public int slowLength {get; set;}
		[Input] public double strongLevel { get; set; }
		[Input] public Color strongColor { get; set;}
		[Input] public Color weakColor { get; set;}
		
		public TrendPower(object _ctx):base(_ctx){
			fastLength = 7;
			slowLength = 14; 
		}
		
		private IPlotObject powerLine;
		
		protected override void Create() {
			powerLine = AddPlot(new PlotAttributes("Power", EPlotShapes.Histogram, Color.Cyan));
		}
		protected override void StartCalc() {
			// assign inputs 
		}
		protected override void CalcBar(){
			double afast = AvgVal(fastLength); //Average price 4 short period
			double aslow = AvgVal(slowLength); //Average price 4 long period
			
			//Calculation of trend strength in percent from a close price of the current bar
			double power = Math.Abs(100*(afast - aslow)/Bars.Close[0]);
			Color color = weakColor;
			if (power >= strongLevel ) color = strongColor;
			
			//Plot value settings 4 current bar
			powerLine.Set(power, color);
		}
		
		private double AvgVal(int length){
			double aval = 0.0;
			for (int i = 0; i < length; ++i)
				aval += Bars.AvgPrice(i);
			
			return aval / length;
		}
	}
}