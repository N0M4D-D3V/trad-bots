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

		private IOrderMarket trendLongEnter;
		private IOrderMarket trendShortEnter;
		private IOrderMarket trendLongExit;
		private IOrderMarket trendShortExit;

		double oldPower;

		public TrendPower(object _ctx):base(_ctx){
			fastLength = 7;
			slowLength = 14;
			strongLevel = 0.9;
		}

		protected override void Create() {
			trendLongEnter = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend LE", EOrderAction.Buy));
			trendShortEnter = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend SE", EOrderAction.SellShort));
			trendLongExit = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend LX", EOrderAction.Sell));
			trendShortExit = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend SX", EOrderAction.BuyToCover));
		}

		private double AvgVal( int length ){
			double aval = 0.0;
			for (int i = 0; i < length; ++i)
			aval += Bars.AvgPrice(i);
			return aval / length;
		}

		protected override void StartCalc() {
			oldPower = 0;
		}

		protected override void CalcBar() {
			double afast = AvgVal(fastLength);
			double aslow = AvgVal(slowLength);
			double power = Math.Abs(100*(afast - aslow/Bars.Close[0]));

			if((power >= strongLevel) && (oldPower < strongLevel)){
				switch (TrendDir())
				{
					case -1:
						trendShortEnter.Send();
						break;
					case 1:
						trendLongEnter.Send();
						break;
				}
			}

			if((CurrentPosition.Side != EMarketPositionSide.Flat)
				&& (oldPower >= strongLevel)
				&& (power < strongLevel)
				){
					trendLongExit.Send();
					trendShortExit.Send();
				}
				oldPower = power;
		}

		private int TrendDir() {
			if((Bars.Low[0] < Bars.Low[1]) && (Bars.High[0] < Bars.High[1])) return -1;
			if((Bars.Low[0] > Bars.Low[1]) && (Bars.High[0] > Bars.High[1])) return 1;
			return 0;
		} 

	}//class
}//namespace