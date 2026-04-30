using System.ComponentModel.DataAnnotations.Schema;

namespace pray_server.Commons
{
    public class SharedValueObjects
    {

    }

    public class GameAccountVo
    {
        public string Nickname { get; set; }
        public int Level { get; set; }
        public DateTime CreateDt { get; set; }
        public int Exp { get; set; }
        public DateTime? LastLoginDt { get; set; }
    }

}
