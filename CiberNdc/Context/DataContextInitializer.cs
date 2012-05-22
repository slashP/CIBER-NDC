using System.Data.Entity;
using System.Web.Security;

namespace CiberNdc.Context
{ 
    public class DataContextInitializer:DropCreateDatabaseAlways<DataContext>
    {
    }
}