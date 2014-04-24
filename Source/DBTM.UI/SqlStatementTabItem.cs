using System.Data;
using System.Windows;
using System.Windows.Controls;
using DBTM.Domain.Entities;

namespace DBTM.UI
{
    public class SqlStatementTabItem : TabItem
    {
        public static readonly DependencyProperty StatementTypeProperty = DependencyProperty.Register("StatementType",
                                                                                              typeof (SqlStatementType),
                                                                                              typeof (SqlStatementTabItem));
        public SqlStatementType StatementType
        {
            get { return (SqlStatementType)GetValue(StatementTypeProperty); }
            set { SetValue(StatementTypeProperty, value); }
        }
    }
}