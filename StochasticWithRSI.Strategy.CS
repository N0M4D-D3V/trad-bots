using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class StochasticWithRSI : SignalObject {

		private TimeSpan startTime, endTime;
		private IOrderMarket enterLong, exitLong;
		private IOrderPriced stoploss;

		private RSI rsiIndicator;
        private VariableSeries<Double> rsi;
		private ISeries<double> Price { get; set; }

		[Input] public string StartTime { get; set; }
		[Input] public string EndTime { get; set; }
        [Input] public int Length { get; set; }
		[Input] public double OverBought { get; set; }
		[Input] public double OverSold { get; set; }
		[Input] public double StoplossRate { get; set; }

		public StochasticWithRSI(object _ctx):base(_ctx){
            OverBought = 70;
			OverSold = 30;
            Length = 14;
			StoplossRate = 0.03;
		}

		protected override void Create() {
			StartTime = "10:00";
			EndTime = "17:00";

			rsiIndicator = new RSI(this);
            rsi = new VariableSeries<Double>(this);
			enterLong = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "buy-order", EOrderAction.Buy));
			exitLong = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "sell-order", EOrderAction.Sell));

			stoploss = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "stoploss", EOrderAction.Sell, OrderExit.FromAll));
		}

		protected override void StartCalc() {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

			Price = Bars.Close;
            rsiIndicator.price = Price;
            rsiIndicator.length = Length;
		}

		protected override void CalcBar(){
			 if (!TimeToTrade()){
                if (StrategyInfo.MarketPosition > 0){
					exitLong.Send();
				}
                return;
            }
			manageLong();
			manageStoploss();
		}

		private void manageLong(){
			rsi.Value = rsiIndicator[0];
			if (this.CrossesUnder(rsi, OverBought)){
				exitLong.Send();
			}
			if (this.CrossesOver(rsi, OverSold)){
				enterLong.Send();
			}
		}

		private void manageStoploss(){
			CurSpecOrdersMode = ESpecOrdersMode.PerContract;
            if (StrategyInfo.MarketPosition > 0){
                GenerateStopLoss(this.EntryPrice()*StoplossRate);
				GenerateProfitTarget(50);
            }else{
                stoploss.Send(Bars.Close[0]*(1.0 - StoplossRate));
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