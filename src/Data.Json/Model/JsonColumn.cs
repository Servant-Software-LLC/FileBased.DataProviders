namespace Data.Json.Model
{

    public class JsonColumn : Base
    {
        public JsonColumn(string name, Type type) : base(name)
        {
            Type = type;
        }

        public JsonColumn(string name, string userGivenName)
            : base(name, userGivenName)
        {
        }
        public Type? Type { get; set; }
    }

}
