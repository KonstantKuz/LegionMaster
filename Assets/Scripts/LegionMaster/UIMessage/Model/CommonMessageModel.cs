using LegionMaster.UIMessage.Panel;

namespace LegionMaster.UIMessage.Model
{
    public class CommonMessageModel
    {
        private const int DEFAULT_TIMEOUT = 5;
        public string Message { get; }
        public int Timeout { get; private set; }
        public string Prefab { get; private set; }   
        public MessageType MessageType { get; private set; }
        
        private CommonMessageModel(string message)
        {
            Message = message;
            Timeout = DEFAULT_TIMEOUT;
            Prefab = CommonUIMessagePanel.PREFAB_PATH;
            MessageType = MessageType.CENTER;
        }
        public static Builder Create(string message)
        {
            return new Builder(message);
        }
        public class Builder
        {
            private readonly CommonMessageModel _model;

            public Builder(string message)
            {
                _model = new CommonMessageModel(message);
            }
            public Builder Timeout(int timeout)
            {
                _model.Timeout = timeout;
                return this;
            }
            public Builder Prefab(string path)
            {
                _model.Prefab = path;
                return this;
            }    
            public Builder MessageType(MessageType type)
            {
                _model.MessageType = type;
                return this;
            }
            public CommonMessageModel Build()
            {
                return _model;
            }
        }
    }
}