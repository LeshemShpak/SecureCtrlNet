
namespace fin_pro
{
    public class InfoData
    {
        public string data {  get; set; }
        public List<CommandData> commands { get; set; }
    }
    public class CommandData
    {
        public Dictionary<int, string> Commands { get; set; }
    }
    public class ProcessInfo
    {
        public string name { get; set; }
        public int id { get; set; }
    }

    public class CommandInfo
    { 
        public string cmd_to_execute { get; set; }
        public string command { get; set; } 
        public string asking_ip { get; set; } 
        public string process_id { get;set; }
        public string file_name { get; set; }
        public string file_data { get; set; }

    }
    public class CommandInfo_Ans
    {
        public string command { get; set; }
    }
    public class CommandInfo_Ans_Process : CommandInfo_Ans
    {
        public ProcessInfo[] out_put { get; set; }   
    }
    public class CommandInfo_Ans_Cmd : CommandInfo_Ans
    {
        public string out_put { get; set; }
    }
}
