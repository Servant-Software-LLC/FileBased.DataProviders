namespace Data.Json.Model
{
    public class JsonTable:Base
    {
        public JsonTable():base("")
        {

        }
        public JsonTable(string name) : base(name)
        {
        }

        public JsonTable(string name, string userGivenName) 
            : base(name, userGivenName)
        {
        }
    }

}
