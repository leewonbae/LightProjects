namespace Snowpipe.Commons.Packets
{
    public enum E_PACKET_ERROR_CODE
    {
        SUCCESS = 0,
        //공용 System Error는 1000 이하로 지정한다.
        SERVER_ERROR = 1,
        INVALID_PACKET = 10,
    }
}
