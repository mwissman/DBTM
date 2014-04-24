using System;
using System.Linq;
using System.Linq.Expressions;

namespace DBTM.Cmd
{
    public static class Extensions
    {
        public static string GetArgumentValue(this string[] args,string argumentName)
        {
            string argumentFlag = string.Format("-{0}=",argumentName);

            return args.Where(s => s.StartsWith(argumentFlag,StringComparison.InvariantCultureIgnoreCase)).
               Select(s => s.Substring(argumentFlag.Length).Trim('"')).
               SingleOrDefault();
        }

        public static bool HasArgumentValue(this string[] args, string argumentName)
        {

            string argumentFlag = string.Format("-{0}=", argumentName);

            return args.Where(s => s.StartsWith(argumentFlag, StringComparison.InvariantCultureIgnoreCase)).Count() == 1;
        }

        public static string GetPropertyName<T, TReturn>(this Expression<Func<T, TReturn>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }
    }
}