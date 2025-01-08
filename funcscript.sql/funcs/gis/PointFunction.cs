using FuncScript.Core;
using FuncScript.Model;
using NetTopologySuite.Geometries;

namespace FuncScript.Sql.Funcs.Gis
{
    public class PointFunction : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "point";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != this.MaxParsCount)
                throw new Error.EvaluationTimeException($"{this.Symbol} function: invalid parameter count. {this.MaxParsCount} expected, got {pars.Length}");

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
