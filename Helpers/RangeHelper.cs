using System;
using System.Collections;
using System.Collections.Generic;

namespace Helpers
{
	public class RangeHelper
	{
		public IEnumerable<double> CreateRange(double a, double b, double step)
		{
			for (double val = a; val <= b; val += step)
				yield return val;
		}
	}
}

