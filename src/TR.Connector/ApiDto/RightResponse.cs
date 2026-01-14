namespace TR.Connector.ApiDto
{
    internal class RightResponseData
    {
        public int id { get; set; }
        public string name { get; set; }
        public object users { get; set; }
    }

    internal class RightResponse
    {
        public List<RightResponseData> data { get; set; }
        public bool success { get; set; }
        public object errorText { get; set; }
        public int count { get; set; }
    }
}
