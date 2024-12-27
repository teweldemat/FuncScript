using funcscript.core;
using funcscript.model;
using NetTopologySuite.Geometries;

namespace funcscript.sql.funcs.gis
{
    public class PointFunction : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "point";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != this.MaxParsCount)
                throw new error.EvaluationTimeException($"{this.Symbol} function: invalid parameter count. {this.MaxParsCount} expected, got {pars.Length}");

            var x = Convert.ToDouble(pars[0]);
            var y = Convert.ToDouble(pars[1]);

            return new Point(x, y);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "X coordinate",
                1 => "Y coordinate",
                _ => "",
            };
        }
    }
}