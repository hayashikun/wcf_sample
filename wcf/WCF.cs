using System;
using System.ServiceModel;

namespace wcf
{
    public struct Data
    {
        public string text;
        public int number;
    }

    public class WCF
    {
        static string Address = "net.pipe://localhost/wcf_sample";

        [ServiceContract(CallbackContract = typeof(ICallback))]
        public interface IHost
        {
            [OperationContract(IsOneWay = true)]
            void LoadData();

            [OperationContract(IsOneWay = true)]
            void UpdateSlider(double val);
        }

        [ServiceContract]
        public interface ICallback
        {
            [OperationContract(IsOneWay = true)]
            void SendData(Data data);

            [OperationContract(IsOneWay = true)]
            void Send();
        }

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
        public class Host : IHost
        {
            
            public delegate Data LoadDataListener();
            public delegate void UpdateSliderListener(double val);
            private LoadDataListener loadDataListener;
            private UpdateSliderListener updateSliderListener;


            public void LoadData()
            {
                var data = loadDataListener();
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                callback.SendData(data);
            }

            public void UpdateSlider(double val)
            {
                updateSliderListener(val);
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                callback.Send();
            }

            public static void StartHost(string id, LoadDataListener loadDataListener, UpdateSliderListener updateSliderListener)
            {
                var host = new Host() { loadDataListener = loadDataListener, updateSliderListener = updateSliderListener };
                var serviceHost = new ServiceHost(host);
                var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

                try
                {
                    serviceHost.AddServiceEndpoint(typeof(IHost), binding, Address + id);
                    serviceHost.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public class Callback : ICallback
        {
            private string id;
            public delegate void DataHandler(string id, Data data);
            public delegate void Handler(string id);
            private DataHandler dataHandler;
            private Handler handler;

            public void SendData(Data data)
            {
                dataHandler(id, data);
            }

            public void Send()
            {
                handler(id);
            }

            public static IHost ConnectHost(string id, DataHandler dataHandler, Handler handler)
            {
                var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

                Callback callback = new Callback() { id = id, dataHandler = dataHandler, handler = handler };

                var host = new DuplexChannelFactory<IHost>(callback,
                    new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),
                    new EndpointAddress(Address + id)).CreateChannel();
                return host;
            }
        }
    }
}
