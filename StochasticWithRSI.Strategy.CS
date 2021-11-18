using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class StochasticWithRSI : SignalObject {

		private TimeSpan startTime, endTime;
		private IOrderMarket enterLong, enterShort, exitLong, exitShort;

		private RSI rsiIndicator;
        private VariableSeries<Double> rsi;
		private ISeries<double> Price { get; set; }

		[Input] public string StartTime { get; set; }
		[Input] public string EndTime { get; set; }
        [Input] public int Length { get; set; }
		[Input] public double OverBought { get; set; }
		[Input] public double OverSold { get; set; }

		public StochasticWithRSI(object _ctx):base(_ctx){
            OverBought = 70;
			OverSold = 30;
            Length = 14;
		}

		protected override void Create() {
			StartTime = "8:00";
			EndTime = "20:00";

			rsiIndicator = new RSI(this);
            rsi = new VariableSeries<Double>(this);
			enterLong = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "buy-order", EOrderAction.Buy));
			enterShort = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "sell-short-order", EOrderAction.SellShort));
			exitLong = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "sell-order", EOrderAction.Sell));
			exitShort = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "buy-to-cover", EOrderAction.BuyToCover));
		}

		protected override void StartCalc() {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

			Price = Bars.Close;
            rsiIndicator.price = Price;
            rsiIndicator.length = Length;
		}

		protected override void CalcBar(){
			 if (!TimeToTrade())
            {
                if (StrategyInfo.MarketPosition > 0){
					exitShort.Send();
					exitLong.Send();
				}

                return;
            }

			rsi.Value = rsiIndicator[0];
			if (this.CrossesUnder(rsi,OverBought)){
				//exitShort.Send();
				exitLong.Send();
				//enterShort.Send();
			}
			if (this.CrossesOver(rsi,OverSold)){
				exitShort.Send();
				//exitLong.Send();
				enterLong.Send();
			}
		}

		private TimeSpan ConvertToTime(string timeToConvert){
            DateTime dt = DateTime.ParseExact(timeToConvert, "H:mm",
                System.Globalization.CultureInfo.InvariantCulture);

            return dt.TimeOfDay;
        }

		private bool TimeToTrade(){
            TimeSpan barTime = Bars.TimeValue.TimeOfDay;
            if (startTime < endTime)
                return (barTime >= startTime) && (barTime <= endTime);

            return (barTime > startTime) || (barTime < endTime);
        }
	}
}