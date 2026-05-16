using System;

namespace Snowpipe.Commons
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
