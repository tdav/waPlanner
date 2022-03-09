namespace waPlanner.ModelViews
{
    public class Vars
    {
        public string PrivateKeyString { get; set; }
        public int CacheTimeOut { get; set; }
        public int PageSize { get; set; }

        public Smssetings SmsSetings { get; set; }
    }

    public class Smssetings
    {
        public string Url { get; set; }
        public string Path { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Text { get; set; }
    }

}
