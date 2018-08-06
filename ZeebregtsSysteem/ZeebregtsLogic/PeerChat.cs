using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;

namespace ZeebregtsLogic
{
    [ServiceContract(Namespace = "ZeebregtsLogic", CallbackContract = typeof(IChat))]
    public interface IChat
    {
        [OperationContract(IsOneWay = true)]
        void Join(string member);

        [OperationContract(IsOneWay = true)]
        void Chat(string member, string msg);

        [OperationContract(IsOneWay = true)]
        void Leave(string member);
    }

    public interface IChatChannel : IChat, IClientChannel
    {
    }

    /// <summary>
    /// [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    /// </summary>
    /// [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PeerChat : IChat
    {
        private bool IsConnected { get; set; }

        string member;
        IChatChannel participant;
        DuplexChannelFactory<IChatChannel> factory;
        public delegate void JoinChat(string member);
        public delegate void ChatSendReceive(string member, string msg);
        public delegate void LeaveChat(string member);
        public delegate void ChatOnline(object sender, EventArgs e);
        public delegate void ChatOffline(object sender, EventArgs e);
        public event JoinChat OnJoin;
        public event ChatSendReceive OnChat;
        public event LeaveChat OnLeave;
        public event ChatOnline OnChatOnline;
        public event ChatOffline OnChatOffline;

        public PeerChat(string member)
        {
            this.member = member;
        }

        public bool Start(IPAddress endpoint)
        {
            bool result = true;
            InstanceContext instanceContext = new InstanceContext(this);
            
            //factory = new DuplexChannelFactory<IChatChannel>(instanceContext, "ChatEndpoint");


            factory = new DuplexChannelFactory<IChatChannel>(instanceContext);


            //MessageBox.Show("localaddress endpoint: " + endpoint.ToString());

            // TODO: regel hieronder UIT
            //factory.Endpoint.Address = new EndpointAddress("net.p2p://" + endpoint.ToString() + ":8089/ZeebregtsChannelService/Chat");
            
            // TODO: regel hieronder AAN
            //factory.Endpoint.Address = new EndpointAddress("net.p2p://" + "127.0.0.1" + ":8089/ZeebregtsChannelService/Chat");

            // TODO: regel hieronder UIT
            factory.Endpoint.Address = new EndpointAddress("net.p2p://" + "localhost" + ":8089/ZeebregtsChannelService/Chat");
            
            NetPeerTcpBinding binding = new NetPeerTcpBinding(); // ("ChatEndpoint");

            // TODO: regel hieronder uit
            binding.ListenIPAddress = endpoint; // IPAddress.Parse("127.0.0.1"); // endpoint;
            

            binding.Port = 8089;
            binding.Resolver.Mode = System.ServiceModel.PeerResolvers.PeerResolverMode.Custom;

            NetTcpBinding resolver = new NetTcpBinding();
            resolver.Security.Mode = SecurityMode.None;
            
            binding.Resolver.Custom.Address = new EndpointAddress("net.tcp://192.160.0.120:8089/ZeebregtsChannelService/peerResolverService");
            binding.Security.Mode = SecurityMode.None;

            binding.Resolver.Custom.Binding = resolver;
            factory.Endpoint.Binding = binding;

            ContractDescription cd = ContractDescription.GetContract(typeof(ZeebregtsLogic.IChat));
            factory.Endpoint.Contract = cd; // new System.ServiceModel.Description.ContractDescription("IChat"); //, "ZeebregtsLogic");
            factory.Endpoint.Name = "ChatEndpoint";



            participant = factory.CreateChannel(instanceContext);
            
            //MessageBox.Show("localaddress: " + participant.LocalAddress.ToString());
            //MessageBox.Show("chatserver: " + Global.ChannelServiceIpAddress);
            //MessageBox.Show("port: " + Global.ChannelServicePort.ToString());

            IOnlineStatus ostat = participant.GetProperty<IOnlineStatus>();
            ostat.Online += new EventHandler(OnOnline);
            ostat.Offline += new EventHandler(OnOffline);

            try
            {
                participant.Open();
                IsConnected = true;
            }
            catch (CommunicationException ex)
            {
                string test2 = ex.InnerException.Message;
                //MessageBox.Show(test2);

                result = false;

                IsConnected = false;
                return result;
            }

            participant.Join(member);
            return result;
        }

        public bool SendMessage(string message)
        {
            try
            {
                //MessageBox.Show("Send: " + message);
                participant.Chat(member, message);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public void End()
        {
            try
            {
                if (IsConnected)
                {
                    if (participant != null)
                    {
                        participant.Leave(member);
                        participant.Close();
                        participant.Dispose();
                    }

                    if (factory != null)
                    {
                        factory.Close();
                    }
                }

                IsConnected = false;
            }
            catch (Exception)
            {
            }
        }

        public void Join(string member)
        {
            if (OnJoin != null)
                OnJoin(member);
        }

        public void Chat(string member, string msg)
        {
            if (OnChat != null)
                OnChat(member, msg);
        }

        public void Leave(string member)
        {
            if (OnLeave != null)
                OnLeave(member);
        }

        public void OnOnline(object sender, EventArgs e)
        {
            if (OnChatOnline != null)
                OnChatOnline(member, e);
        }

        public void OnOffline(object sender, EventArgs e)
        {
            if (OnChatOffline != null)
                OnChatOffline(member, e);
        }
    }
}