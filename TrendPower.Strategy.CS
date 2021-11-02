using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

//C:\ProgramData\TS Support\MultiCharts .NET64 Special Edition\StudyServer\Techniques\CS
namespace PowerLanguage.Strategy {
	public class TrendPower : SignalObject {
		[Input] public int fastLength { get; set; }
		[Input] public int slowLength { get; set; }
		[Input] public double strongLevel { get; set; }
		
		private IOrderMarket openLongPosition;
		private IOrderMarket openShortPosition;
		private IOrderMarket closeLongPosition;
		private IOrderMarket closeShortPosition;
		
		private double oldPower;

		
		public TrendPower(object _ctx):base(_ctx){
			fastLength = 7;
			slowLength = 14;
			strongLevel = 0.9;
		}
		
		private IOrderMarket buy_order;
		
		private double AvgVal( int length ){
			Output.WriteLine("AvgVal CalcBar...");
			double aval = 0.0;
			for (int i = 0; i < length; ++i)
				aval += Bars.AvgPrice(i);
			return aval / length;
		}
		
		private int TrendDirection(){
			Output.WriteLine("Executing TrendDirection...");
			if (
				(Bars.Low[0] < Bars.Low[1])
				&&(Bars.High[0] < Bars.High[1])
				) {
					Output.WriteLine("-1");
					return -1;
				}
			if (
				(Bars.Low[0] > Bars.Low[1])
				&&(Bars.High[0] > Bars.High[1])
				){
					Output.WriteLine("1");
					return 1;
				}
			return 0;
		}


		
		protected override void Create() {
			Output.WriteLine("Executing Create...");
			//openLongPosition = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend Long Open", EOrderAction.Buy));
			//openShortPosition = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend Short Open", EOrderAction.SellShort));
			//closeLongPosition = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend Long Close", EOrderAction.Sell));
			//closeShortPosition = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend Short Close", EOrderAction.BuyToCover));
		}
		
		protected override void StartCalc() {
			Output.WriteLine("Executing StartCalc...");
			oldPower = 0;
		}
		
		protected override void CalcBar(){
			Output.WriteLine("Executing CalcBar...");
			
			double afast = AvgVal(fastLength);
			double aslow = AvgVal(slowLength);
			double power = Math.Abs(100*(afast - aslow)/Bars.Close[0]);
			Output.WriteLine("Power: ", fastLength);
			Output.WriteLine("OldPower: ", slowLength);
			
			if ( 
				(power >= strongLevel)
				&&(oldPower < strongLevel)
				){
					switch(TrendDirection()){
						case -1:
							//trend_SE.Send();
							Output.WriteLine("Opening short position");
							break;
						case 1:
							//trend_LE.Send();
							Output.WriteLine("Opening long position");
							break;
					}
				}
						
				if ((CurrentPosition.Side != EMarketPositionSide.Flat)
					&&(oldPower >= strongLevel)
					&&(power < strongLevel)
					){
						//trend_LX.Send();
						//trend_SX.Send();
						Output.WriteLine("Closing all positions");
					}
				oldPower = power;
		}
		
	}//class
}//namespace