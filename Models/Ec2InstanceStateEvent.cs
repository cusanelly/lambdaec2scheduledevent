namespace ec2startstopserver.Models
{
    public class Ec2InstanceStateEvent
    {
        public string[] source { get; set; }
        public string[] detailtype { get; set; }
        public Detail detail { get; set; }
    }   
    public class Detail
    {
        public string[] state { get; set; }
        public string[] instanceid { get; set; }
    }
}
