namespace Data.Json.Model
{
    public class Base
    {
        public string Name { get; set; }
        public string UserGivenName { get; set; }

        public Base(string name)
        {
            Name = name;
            this.UserGivenName = name;
        }
        public Base(string name, string userGivenName)
        {
            Name = name;
            UserGivenName = userGivenName;
        }
    }

    public class Column : Base
    {
        public Column(string name, Type type) : base(name)
        {
            Type = type;
        }

        public Column(string name, string userGivenName)
            : base(name, userGivenName)
        {
        }
        public Type Type { get; set; }
    }

}
