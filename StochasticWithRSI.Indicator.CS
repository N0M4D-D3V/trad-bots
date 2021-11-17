using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class StochasticWithRSI : IndicatorObject {
        //COMMON LINES
        private IPlotObject overboughtLine;
        private IPlotObject oversoldLine;
        [Input] public double oversold { get; set; }
        [Input] public double overbought { get; set; }

		//RSI variables
		private Function.RSI rsiFunction;
        private VariableSeries<Double> rsi;
        private IPlotObject RSITrendLine;

		//Stochastic variables
		private Stochastic stochastic;
        private VariableSeries<Double> m_ofastk;
        private VariableSeries<Double> m_ofastd;
        private VariableObject<Double> m_oslowk;
        private VariableObject<Double> m_oslowd;
        private IPlotObject Plot1;
        private IPlotObject Plot2;

		//RSI inputs
		[Input] public int length { get; set; }
        [Input] public Color overscolor { get; set; }
        [Input] public Color overbcolor { get; set; }

		//Stochastic Inputs
        [Input] public int stochlength { get; set; }
		[Input] public int smoothinglength1 { get; set; }
        [Input] public int smoothinglength2 { get; set; }
        [Input] public int smoothingtype { get; set; }

		public StochasticWithRSI(object _ctx):base(_ctx){
			DefaultRSIConfiguration();
			DefaultStochasticConfiguration();
		}
	
		protected override void Create() {
			CreateRSI();
			CreateStochastic();
		}

		protected override void StartCalc() {
			StartCalcRSI();
			StartCalcStochastic();
		}

		protected override void CalcBar(){
			CalcBarRSI();
			CalcBarStochastic();
		}

		//RSI functions
		private void DefaultRSIConfiguration(){
			overbcolor = Color.Red;
            overscolor = Color.Cyan;
            length = 14;
            overbought = 70;
            oversold = 30;
		}

		private void CreateRSI(){
			rsiFunction = new Function.RSI(this);
            rsi = new VariableSeries<Double>(this);
            RSITrendLine = AddPlot(new PlotAttributes("RSI", 0, Color.Silver, Color.Empty, 0, 0, true));
            overboughtLine = AddPlot(new PlotAttributes("OverBot", 0, Color.Green, Color.Empty, 0, 0, true));
            oversoldLine = AddPlot(new PlotAttributes("OverSld", 0, Color.Green, Color.Empty, 0, 0, true));
		}

		private void StartCalcRSI(){
			rsiFunction.price = Bars.Close;
            rsiFunction.length = length;
		}

		private void CalcBarRSI(){
			rsi.Value = rsiFunction[0];
            RSITrendLine.Set(0, rsi.Value);
            overboughtLine.Set(0, overbought);
            oversoldLine.Set(0, oversold);
            if (PublicFunctions.DoubleGreater(rsi.Value, overbought)){
                RSITrendLine.Colors[0] = overbcolor;
            }
            else{
                if (PublicFunctions.DoubleLess(rsi.Value, oversold)){
                    RSITrendLine.Colors[0] = overscolor;
                }
            }
            if (this.CrossesOver(rsi, oversold)){
                Alerts.Alert("Indicator exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(rsi, overbought)){
                    Alerts.Alert("Indicator exiting overbought zone");
                }
            }
		}

		//STOCHASTIC FUNCTIONS
		private void DefaultStochasticConfiguration(){
			overbought = 70;
            oversold = 30;
            smoothingtype = 1;
            smoothinglength2 = 3;
            smoothinglength1 = 3;
            stochlength = 14;
		}

		private void CreateStochastic(){
			stochastic = new Stochastic(this);
            m_ofastk = new VariableSeries<Double>(this);
            m_ofastd = new VariableSeries<Double>(this);
            m_oslowk = new VariableObject<Double>(this);
            m_oslowd = new VariableObject<Double>(this);
            Plot1 = AddPlot(new PlotAttributes("FastK", 0, Color.Yellow, Color.Empty, 0, 0, true));
            Plot2 = AddPlot(new PlotAttributes("FastD", 0, Color.Blue, Color.Empty, 0, 0, true));
		}

		private void StartCalcStochastic(){
            stochastic.priceh = Bars.High;
            stochastic.pricel = Bars.Low;
            stochastic.pricec = Bars.Close;
            stochastic.stochlength = stochlength;
            stochastic.length1 = smoothinglength1;
            stochastic.length2 = smoothinglength2;
            stochastic.smoothingtype = smoothingtype;
            stochastic.ofastk = m_ofastk;
            stochastic.ofastd = m_ofastd;
            stochastic.oslowk = m_oslowk;
            stochastic.oslowd = m_oslowd;
		}

		private void CalcBarStochastic(){
			stochastic.Call();
            Plot1.Set(0, m_ofastk.Value);
            Plot2.Set(0, m_ofastd.Value);
            overboughtLine.Set(0, overbought);
            oversoldLine.Set(0, oversold);
            if (this.CrossesOver(m_ofastd, oversold)){
                Alerts.Alert("FastD exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(m_ofastd, overbought)){
                    Alerts.Alert("FastD exiting overbought zone");
                }
            }
		}
	}
}