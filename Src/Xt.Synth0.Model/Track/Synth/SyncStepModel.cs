using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public static class SyncStepModel
    {
        public static Native[] Steps { get; private set; }
        static readonly int[] Bases = { 1, 2, 3, 4, 6, 9, 16 };

        static int GCD(int x, int y)
        {
            while (y > 0)
            {
                int remainder = x % y;
                x = y;
                y = remainder;
            }
            return x;
        }

        static IEnumerable<Native> Cartesian()
        {
            foreach (int n in Bases)
                foreach (int d in Bases)
                    yield return new Native(n, d);
        }

        static IEnumerable<Native> All()
        {
            var result = new List<Native>();
            result.Add(new Native(0, 1));
            result.AddRange(Cartesian());
            foreach (var s in Cartesian())
                if (s.value < 1.0f)
                    result.Add(new Native(s.numerator + s.denominator, s.denominator));
            return result;
        }

        static SyncStepModel()
        {
            var steps = new List<Native>();
            foreach (var s in All())
                steps.Add(s.Simplify());
            Steps = steps.Distinct().OrderBy(s => s.value).ToArray();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct Native
        {
            internal int numerator;
            internal int denominator;
            internal float value => numerator / (float)denominator;
            internal Native(int n, int d) => (numerator, denominator) = (n, d);

            public override string ToString() => $"{numerator}/{denominator}";
            public override int GetHashCode() => numerator + 37 * denominator;

            internal Native Simplify()
            {
                int gcd = GCD(numerator, denominator);
                return new Native(numerator / gcd, denominator / gcd);
            }

            public override bool Equals(object obj)
            {
                Native s = (Native)obj;
                return s.numerator == numerator && s.denominator == denominator;
            }
        }
    }
}