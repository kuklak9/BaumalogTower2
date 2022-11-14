namespace BTPDataBase.DS_TraysTableAdapters
{
    public partial class QueriesTableAdapter
    {
        public QueriesTableAdapter(string connectionString)
        {
            foreach(var command in CommandCollection)
                command.Connection.ConnectionString = connectionString;
        }
    }
}

namespace BTPDataBase
{


    partial class DS_Trays
    {
    }
}
