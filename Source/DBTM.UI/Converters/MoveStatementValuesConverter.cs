using System;
using System.Linq;
using System.Windows.Data;
using DBTM.Application.Commands;
using DBTM.Domain.Entities;

namespace DBTM.UI.Converters
{
    public class MoveStatementValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((values.Length==2) && 
                (values.All(v=>v!=null)) && 
                (values[0] is SqlStatement) && 
                (values[1] is SqlStatementCollection))
            {
                return new StatementMoveRequest()
                           {
                               CollectionToMoveStatementIn = values[1] as SqlStatementCollection,
                               StatementToMove = values[0] as SqlStatement
                           };
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}