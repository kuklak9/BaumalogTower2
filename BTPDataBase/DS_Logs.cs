namespace BTPDataBase
{
}

namespace BTPDataBase
{
}

namespace BTPDataBase.DS_LogsTableAdapters
{
    public partial class QueriesTableAdapter
    {
        public string ConnectionString
        {
            get
            {
                return CommandCollection[0].Connection.ConnectionString;
            }
            set
            {
                foreach (var command in CommandCollection)
                {
                    command.Connection.ConnectionString = value;
                }
            }
        }
    }
}
namespace BTPDataBase
{


    public partial class DS_Logs
    {
    }
}
