using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator {
	public class nfsMacdBB : IndicatorObject {
		public nfsMacdBB(object _ctx):base(_ctx) {}

		private IPlotObject bbMacd, bbMacdFrame;
		private IPlotObject bbAvg, bbUpper, bbLower;
		private IPlotObject macdCross, macdLine, zeroLine;
		private Function.MACD macd;
		private VariableSeries<double> Values;
		private int m_fastLength = 12, m_slowLength = 26, m_bandLength = 10;
		private Color zeroColor = Color.Blue;


		[Input]
		public int FastLength {
			get { return m_fastLength; }
			set { m_fastLength = value; }
		}

		[Input]
		public int SlowLength {
			get { return m_slowLength; }
			set { m_slowLength = value; }
		}



		protected override void Create() {
			// create variable objects, function objects, plot objects etc.
			macd = new Function.MACD(this);
			Values = new VariableSeries<double>( this );

			// set up plots and their default styles
			zeroLine = AddPlot(new PlotAttributes("Zero", EPlotShapes.Line,
				Color.Gray, Color.Empty, 1, 0, false));
			bbAvg = AddPlot(new PlotAttributes("Average", EPlotShapes.Line,
				Color.DarkGray, Color.Empty, 1, 2, false));
			bbUpper = AddPlot(new PlotAttributes("Upper", EPlotShapes.Line,
				Color.Gray, Color.Empty, 1, 0, false));
			bbLower = AddPlot(new PlotAttributes("Lower", EPlotShapes.Line,
				Color.Gray, Color.Empty, 1, 0, false));
			macdCross = AddPlot(new PlotAttributes("MacdCross", EPlotShapes.Point,
				Color.Yellow, Color.Empty, 17, 0, false));
			macdLine = AddPlot(new PlotAttributes("Connector", EPlotShapes.Line,
				Color.White, Color.Empty, 2, 0, false));
			bbMacdFrame = AddPlot(new PlotAttributes("MACD Dot Outline",
				EPlotShapes.Point, Color.Black, Color.Empty, 19, 0, false));
			bbMacd = AddPlot(new PlotAttributes("MACD Dots", EPlotShapes.Point,
				Color.Gray, Color.Empty, 5, 0, false));
		}


		protected override void StartCalc() {
			// assign inputs
			macd.Price = Bars.Close;
			macd.FastLength = FastLength;
			macd.SlowLength = SlowLength;
		}


		protected override void CalcBar() {
			double macdValue = macd[0];
			Values.Value = macdValue;
			double macAvgValue = Values.Average(m_bandLength);
			double sdValue = Values.StandardDeviationCustom( m_bandLength,  1 );

			Color dotColor = Color.Red;

			if (macdValue > macd[1]) {
				if (macdValue > macAvgValue + sdValue)
					dotColor = Color.SpringGreen;
				else
					dotColor = Color.MediumAquamarine;
			} else {
				if (macdValue < macAvgValue - sdValue)
					dotColor = Color.Red;
				else
					dotColor = Color.DarkSalmon;
			}
			bbMacd.Set(macdValue, dotColor, 7);
			bbMacdFrame.Set(macdValue, Color.Black, bbMacd.Widths[1] + 2);
			macdLine.Set(macdValue, Color.White, 2);
			bbAvg.Set(macAvgValue);
			bbUpper.Set(macAvgValue + sdValue);
			bbLower.Set(macAvgValue - sdValue);
			if (this.CrossesOver(macd, 0)) {
				macdCross.Set(0, Color.Yellow, 17);
				zeroColor = Color.DarkSeaGreen;
			} else if (this.CrossesUnder(macd, 0)) {
				macdCross.Set(0, Color.Yellow, 17);
				zeroColor = Color.Red;
			} else
				macdCross.Reset();
			zeroLine.Set(0, zeroColor, 2);
		}
	}
}