namespace Monad.FLParser
{
    public class AutomationData : IChannelData
    {
        public Channel Channel { get; set; } = null;
        public int Parameter { get; set; } = 0;
        public AutomationKeyframe[] Keyframes { get; set; } = new AutomationKeyframe[0];
    }
}
