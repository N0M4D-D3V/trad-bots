using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class StochasticWithRSI : SignalObject {

		private RSI rsiIndicator;
		private Stochastic stochasticIndicator;
		private AverageFC movAverageIndicator;

		private TimeSpan startTime, endTime;
		private IOrderMarket enterLong, exitLong;
		private IOrderPriced stoploss;

        private VariableSeries<Double> rsi;
		private VariableSeries<Double> average;
		private VariableSeries<Double> stochasticFastK;
        private VariableSeries<Double> stochasticFastD;
        private VariableSeries<Double> stochasticSlowK;
        private VariableSeries<Double> stochasticSlowD;

		private VariableObject<Int32> movAverageCounter;
		private ISeries<double> Price { get; set; }

		[Input] public int ConfirmBars { get; set; }

		[Input] public string StartTime { get; set; }
		[Input] public string EndTime { get; set; }
        [Input] public int Length { get; set; }
		[Input] public int AverageLength { get; set; }
		[Input] public double OverBought { get; set; }
		[Input] public double OverSold { get; set; }
		[Input] public double ContractPrice { get; set; }
		[Input] public double TargetProfitRate {get; set;}
		[Input] public double StoplossRate { get; set; }
		[Input] public int NumberOfContracts { get; set; }

		public StochasticWithRSI(object _ctx):base(_ctx){
			SetDefaultConfig();
		}

		protected override void Create() {
			rsiIndicator = new RSI(this);
			stochasticIndicator = new Stochastic(this);
			movAverageIndicator = new AverageFC(this);

            rsi = new VariableSeries<Double>(this);
			average = new VariableSeries<Double>(this);
			stochasticFastK = new VariableSeries<Double>(this);
            stochasticFastD = new VariableSeries<Double>(this);
            stochasticSlowK = new VariableSeries<Double>(this);
            stochasticSlowD = new VariableSeries<Double>(this);
			movAverageCounter = new VariableObject<int>(this);

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
			StartCalcStochastic();
		}

		private void StartCalcRSI(){
            rsiIndicator.price = Price;
            rsiIndicator.length = Length;
		}

		private void StartCalcMovAverage(){
			movAverageIndicator.price = Bars.Close;
            movAverageIndicator.length = Length;
		}

		private void StartCalcStochastic(){
			stochasticIndicator.priceh = Bars.High;
            stochasticIndicator.pricel = Bars.Low;
            stochasticIndicator.pricec = Bars.Close;
            stochasticIndicator.stochlength = Length;
            stochasticIndicator.length1 = 3;
            stochasticIndicator.length2 = 3;
            stochasticIndicator.smoothingtype = 1;
            stochasticIndicator.ofastk = stochasticFastK;
            stochasticIndicator.ofastd = stochasticFastD;
            stochasticIndicator.oslowk = stochasticSlowK;
            stochasticIndicator.oslowd = stochasticSlowD;
		}

		protected override void CalcBar(){
			if (!isTimeToTrade()){
                if (StrategyInfo.MarketPosition > 0){
					exitLong.Send(NumberOfContracts);
				}
                return;
            }
			manageLongPosition();
			manageStoploss();
		}

		private void manageLongPosition(){
			rsi.Value = rsiIndicator[0];
			average.Value = movAverageIndicator[0];
			stochasticIndicator.Call();

			if (PublicFunctions.DoubleGreater(Price[0], average.Value)){
                ++movAverageCounter.Value;
            }
            else{
                movAverageCounter.Value = 0;
            }

			if (isMovAverageIndicatingExit()){
				exitLong.Send(NumberOfContracts);
			}

			//this.CrossesOver(rsi, OverSold)
			if ( isMovAverageIndicatingEnter() && isStochasticIndicatingEnter() ){
				enterLong.Send(NumberOfContracts);
			}
		}

		private bool isStochasticIndicatingEnter(){
			return Bars.CurrentBar > 2
                && stochasticSlowK.CrossesOver(stochasticSlowD, ExecInfo.MaxBarsBack)
                && PublicFunctions.DoubleLess(stochasticSlowK.Value, OverSold);
		}

		private bool isMovAverageIndicatingEnter(){
			return (PublicFunctions.DoubleGreater(Bars.CurrentBar, ConfirmBars)
                 && movAverageCounter.Value == ConfirmBars);
		}

		private bool isMovAverageIndicatingExit(){
			return (Bars.CurrentBar > 1 && Bars.Close.CrossesUnder(average, ExecInfo.MaxBarsBack));
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

		private void SetDefaultConfig(){
            OverBought = 70;
			OverSold = 30;
            Length = 14;
			AverageLength = 9;
			ContractPrice=400;
			TargetProfitRate = 0.125;
			StoplossRate = 1.0;
			NumberOfContracts = 1;
			ConfirmBars = 1;
			StartTime = "10:00";
			EndTime = "17:00";
		}
	}
}