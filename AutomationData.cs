namespace Monad.FLParser
{
    public class AutomationData : IChannelData
    {
        public Channel Channel { get; set; } = null;
        public int Parameter { get; set; } = 0;
        public int InsertId { get; set; } = -1;
        public int SlotId { get; set; } = -1;
        public AutomationKeyframe[] Keyframes { get; set; } = new AutomationKeyframe[0];
    }
}
