using _LumenLib.CoreSystem.EventChannelSystem.Runtime;

namespace GGW.game._00.Codes.Traps
{
    public interface IListenAlarmable
    {
        public EventChannelSO AlarmEvent {get;}

        void Listen(AlarmEvent evt);
    }
}