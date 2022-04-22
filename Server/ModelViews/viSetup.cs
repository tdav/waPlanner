namespace waPlanner.ModelViews
{
    public class viSetup
    {
        public string Lang { get; set; }
        public string Theme { get; set; }


        public static viSetup FromJson(string s)
        {
           return Newtonsoft.Json.JsonConvert.DeserializeObject<viSetup>(s);
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
