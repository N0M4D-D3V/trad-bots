using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class Mini_Bullish : SignalObject {

		private RSI rsiIndicator;
		private AverageFC fastMovAverageIndicator;
		private AverageFC slowMovAverageIndicator;

        private VariableSeries<Double> rsi;
		private VariableSeries<Double> fastAverage;
		private VariableSeries<Double> slowAverage;

		private TimeSpan startTime, endTime;
		private IOrderMarket enterLong, exitLong;
		private IOrderPriced stoploss;

		private ISeries<double> Price { get; set; }

		[Input] public string StartTime { get; set; }
		[Input] public string EndTime { get; set; }
		[Input] public double ContractPrice { get; set; }
		[Input] public double TargetProfitRate {get; set;}
		[Input] public double StoplossRate { get; set; }
		[Input] public int NumberOfContracts { get; set; }

		[Input] public int Length { get; set; }
        [Input] public double OverSold { get; set; }

		[Input] public int FastLength { get; set; }

        [Input] public int SlowLength { get; set; }

		public Mini_Bullish(object _ctx):base(_ctx){
			BasicConfiguration();
			RSIConfiguration();
			MovAverageConfiguration();
		}

		protected override void Create() {
			CreateRSI();
			CreateMovAverage();
			CreateOrders();
		}

		private void CreateRSI(){
			rsiIndicator = new RSI(this);
			rsi = new VariableSeries<Double>(this);
		}

		private void CreateMovAverage(){
            fastMovAverageIndicator = new AverageFC(this);
            slowMovAverageIndicator = new AverageFC(this);
            fastAverage = new VariableSeries<Double>(this);
            slowAverage = new VariableSeries<Double>(this);
		}

		private void CreateOrders(){
			enterLong = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.UserSpecified, "buy-order", EOrderAction.Buy));
			exitLong = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.UserSpecified, "sell-order", EOrderAction.Sell));
			stoploss = OrderCreator.Stop(new SOrderParameters(Contracts.UserSpecified, "stoploss", EOrderAction.Sell, OrderExit.FromAll));
		}

		protected override void StartCalc() {
			startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

			Price = Bars.Close;
			StartCalcRSI();
			StartCalcMovAverage();
		}

		private void StartCalcRSI(){
            rsiIndicator.price = Price;
            rsiIndicator.length = Length;
		}

		private void StartCalcMovAverage(){
			fastMovAverageIndicator.price = Price;
            fastMovAverageIndicator.length = FastLength;
            slowMovAverageIndicator.price = Price;
            slowMovAverageIndicator.length = SlowLength;
		}

		protected override void CalcBar(){
			if (!isTimeToTrade()){
                if (StrategyInfo.MarketPosition > 0){
					exitLong.Send(NumberOfContracts);
				}
                return;
            }
			managePosition();
			manageStoploss();
		}

		private void managePosition(){
			rsi.Value = rsiIndicator[0];
			fastAverage.Value = fastMovAverageIndicator[0];
            slowAverage.Value = slowMovAverageIndicator[0];

			if ( isOversold() && isFastAverageUnderPrice()){
				enterLong.Send(NumberOfContracts);
			}
		}

		private bool isOversold(){
			return this.CrossesOver(rsi, OverSold);
		}

		private bool isFastAverageUnderPrice(){
			return (fastAverage.Value < Price.Value);
		}

		private void manageStoploss(){
			CurSpecOrdersMode = ESpecOrdersMode.PerContract;
            if (StrategyInfo.MarketPosition > 0){
                GenerateStopLoss(ContractPrice*StoplossRate);
				GenerateProfitTarget(ContractPrice*TargetProfitRate);
            }else{
                stoploss.Send(ContractPrice*StoplossRate);
            }
		}

		private TimeSpan ConvertToTime(string timeToConvert){
            DateTime dt = DateTime.ParseExact(timeToConvert, "H:mm", System.Globalization.CultureInfo.InvariantCulture);
            return dt.TimeOfDay;
        }

		private bool isTimeToTrade(){
            TimeSpan barTime = Bars.TimeValue.TimeOfDay;
            if (startTime < endTime)
                return (barTime >= startTime) && (barTime <= endTime);

            return (barTime > startTime) || (barTime < endTime);
        }

		private void BasicConfiguration(){
			ContractPrice=400;
			TargetProfitRate = 0.03;
			StoplossRate = 0.015;
			NumberOfContracts = 1;
			StartTime = "10:00";
			EndTime = "17:00";
		}

		private void RSIConfiguration(){
			OverSold = 30;
            Length = 14;
		}

		private void MovAverageConfiguration(){
			SlowLength = 18;
            FastLength = 9;
		}
	}
}