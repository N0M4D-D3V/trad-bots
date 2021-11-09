using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class Clue : FunctionSimple<double>
		{
			public Clue(CStudyControl _master) : base(_master) { }
			public Clue(CStudyControl _master, int _ds) : base(_master, _ds) { }

			public ISeries<double> Price{ private get; set;}
			public int Length { private get; set;}

			private XAverage average1;
			private XAverage average2;
			private VariableSeries<double> body;
			private VariableSeries<double> bodyAverage;

			protected override void Create(){
				average1 = new XAverage(this);
				average2 = new XAverage(this);
				body = new VariableSeries<double>(this);
				bodyAverage = new VariableSeries<double>(this);
			}

			protected override void StartCalc(){
				average1.Price = Bars.Close;
				average1.Length = Length;
				average2.Price = body;
				average2.Length = Length;
			}

			protected override System.Double CalcBar(){
				body.Value =  average1[0];

				if(Bars.CurrentBar == 1){
					return Price[0];
				}else{
					return 2 * average1[0] - average2[0];
				}
			}
		}
	}
}