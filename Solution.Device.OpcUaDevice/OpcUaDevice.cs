using System;
using System.Threading.Tasks;
using Solution.Device.Core;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace Solution.Device.OpcUaDevice
{
    public class OpcUaDevice : IDevice
    {
        /// <summary>
        /// 通知事件,设备状态、数据异常等都通过此事件对外发布
        /// </summary>
        public event DeviceNotificationEventHandler Notification;

        #region Private Fields
        private ApplicationInstance application;
        private ApplicationConfiguration m_configuration;
        private Session m_session;
        private bool m_IsConnected;                       //是否已经连接过
        private int m_reconnectPeriod = 10;               // 重连状态
        private bool m_useSecurity;

        private SessionReconnectHandler m_reconnectHandler;
        private EventHandler m_ReconnectComplete;
        private EventHandler m_ReconnectStarting;
        private EventHandler m_KeepAliveComplete;
        private EventHandler m_ConnectComplete;
        //private EventHandler<OpcUaStatusEventArgs> m_OpcStatusChange;
        #endregion



        public string OpcUaName { get; set; }

        public OpcUaDevice()
        {
            var certificateValidator = new CertificateValidator();
            certificateValidator.CertificateValidation += (sender, eventArgs) =>
            {
                if (ServiceResult.IsGood(eventArgs.Error))
                    eventArgs.Accept = true;
                else if ((eventArgs.Error.StatusCode.Code == StatusCodes.BadCertificateUntrusted) && true)
                    eventArgs.Accept = true;
                else
                    throw new Exception(string.Format("Failed to validate certificate with error code {0}: {1}", eventArgs.Error.Code, eventArgs.Error.AdditionalInfo));
            };

            // Build the application configuration
            application = new ApplicationInstance
            {
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = OpcUaName,
                ApplicationConfiguration = new ApplicationConfiguration
                {
                    ApplicationUri = "",
                    ApplicationName = OpcUaName,
                    ApplicationType = ApplicationType.Client,
                    CertificateValidator = certificateValidator,
                    ServerConfiguration = new ServerConfiguration
                    {
                        MaxSubscriptionCount = 100,
                        MaxMessageQueueSize = 100,
                        MaxNotificationQueueSize = 100,
                        MaxPublishRequestCount = 100
                    },
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        AutoAcceptUntrustedCertificates = true,
                    },
                    TransportQuotas = new TransportQuotas
                    {
                        OperationTimeout = 600000,
                        MaxStringLength = 1048576,
                        MaxByteStringLength = 1048576,
                        MaxArrayLength = 65535,
                        MaxMessageSize = 4194304,
                        MaxBufferSize = 65535,
                        ChannelLifetime = 600000,
                        SecurityTokenLifetime = 3600000
                    },
                    ClientConfiguration = new ClientConfiguration
                    {
                        DefaultSessionTimeout = 60000,
                        MinSubscriptionLifetime = 10000
                    },
                    DisableHiResClock = true
                }
            };

            // Assign a application certificate (when specified)
            // if (ApplicationCertificate != null)
            //    application.ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier(_options.ApplicationCertificate);


            m_configuration = application.ApplicationConfiguration;

        }

        /// <summary>
        /// 连接/打开设备
        /// </summary>
        /// <param name="param">连接/打开设备</param>
        /// <returns>连接/打开设备的结果</returns>
        public Task<bool> Connect(object param = null)
        {
            
            return Task.Factory.StartNew<bool>(() =>
            {
                m_session = Connect(param.ToString());
                if (Equals(m_session, null))
                {
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 断开/关闭设备
        /// </summary>
        /// <param name="param">断开/关闭设备的参数</param>
        /// <returns>断开/关闭设备的结果</returns>
        public Task<bool> DisConnect(object param = null)
        {
            //UpdateStatus(false, DateTime.UtcNow, "Disconnected");

            // stop any reconnect operation.
            //if (m_reconnectHandler != null)
            //{
            //    m_reconnectHandler.Dispose();
            //    m_reconnectHandler = null;
            //}
            return Task.Factory.StartNew<bool>(() =>
            {
                if (m_session != null)
                {
                    var result = m_session.Close(1000);
                    return result == StatusCodes.Good;
                }
                return true;
            });

            // disconnect any existing session.
            //if (m_session != null)
            //{
            //    m_session.Close(10000);
            //    m_session = null;
            //}

            // update the client status
            //m_IsConnected = false;

            // raise an event.
            //DoConnectComplete(null);
        }

        public bool UseSecurity { get; set; }

        /// <summary>
        /// The user identity to use when creating the session.
        /// </summary>
        public IUserIdentity UserIdentity { get; set; }

        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <returns>The new session object.</returns>
        private Session Connect(string serverUrl)
        {
            // disconnect from existing session.
            // Disconnect();
            if (m_configuration == null)
            {
                throw new ArgumentNullException("m_configuration");
            }

            // select the best endpoint.
            EndpointDescription endpointDescription = SelectEndpoint(serverUrl, UseSecurity);

            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);


            // create the session with server
            var result = Session.Create(
                  m_configuration,
                  endpoint,
                  false,
                  false,
                  (String.IsNullOrEmpty(OpcUaName)) ? m_configuration.ApplicationName : OpcUaName,
                  60000,
                  UserIdentity,
                  new string[] { });

            // set up keep alive callback.
            //m_session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);

            // update the client status
            m_IsConnected = true;
            //result.Start();
            // raise an event.
            //DoConnectComplete(null);
            m_session = result.Result;
            // return the new session.
            return m_session;
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="data">写入的数据</param>
        /// <returns>写入的结果</returns>
        public Task<object> Write(object data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="data">写入的数据</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>写入的结果</returns>
        public Task<object> Write(object data, uint timeOut)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="data">要读取的数据</param>
        /// <returns>读取的结果</returns>
        public Task<object> Read(object data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="data">要读取的数据</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>读取的结果</returns>
        public Task<object> Read(object data, uint timeOut)
        {
            throw new NotImplementedException();
        }



        public static EndpointDescription SelectEndpoint(string discoveryUrl, bool useSecurity)
        {
            // needs to add the '/discovery' back onto non-UA TCP URLs.
            if (!discoveryUrl.StartsWith(Utils.UriSchemeOpcTcp))
            {
                if (!discoveryUrl.EndsWith("/discovery"))
                {
                    discoveryUrl += "/discovery";
                }
            }

            // parse the selected URL.
            Uri uri = new Uri(discoveryUrl);

            // set a short timeout because this is happening in the drop down event.
            EndpointConfiguration configuration = EndpointConfiguration.Create();
            configuration.OperationTimeout = 5000;

            EndpointDescription selectedEndpoint = null;

            // Connect to the server's discovery endpoint and find the available configuration.
            using (DiscoveryClient client = DiscoveryClient.Create(uri, configuration))
            {
                EndpointDescriptionCollection endpoints = client.GetEndpoints(null);

                // select the best endpoint to use based on the selected URL and the UseSecurity checkbox. 
                for (int ii = 0; ii < endpoints.Count; ii++)
                {
                    EndpointDescription endpoint = endpoints[ii];

                    // check for a match on the URL scheme.
                    if (endpoint.EndpointUrl.StartsWith(uri.Scheme))
                    {
                        // check if security was requested.
                        if (useSecurity)
                        {
                            if (endpoint.SecurityMode == MessageSecurityMode.None)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (endpoint.SecurityMode != MessageSecurityMode.None)
                            {
                                continue;
                            }
                        }

                        // pick the first available endpoint by default.
                        if (selectedEndpoint == null)
                        {
                            selectedEndpoint = endpoint;
                        }

                        // The security level is a relative measure assigned by the server to the 
                        // endpoints that it returns. Clients should always pick the highest level
                        // unless they have a reason not too.
                        if (endpoint.SecurityLevel > selectedEndpoint.SecurityLevel)
                        {
                            selectedEndpoint = endpoint;
                        }
                    }
                }

                // pick the first available endpoint by default.
                if (selectedEndpoint == null && endpoints.Count > 0)
                {
                    selectedEndpoint = endpoints[0];
                }
            }

            // if a server is behind a firewall it may return URLs that are not accessible to the client.
            // This problem can be avoided by assuming that the domain in the URL used to call 
            // GetEndpoints can be used to access any of the endpoints. This code makes that conversion.
            // Note that the conversion only makes sense if discovery uses the same protocol as the endpoint.

            Uri endpointUrl = Utils.ParseUri(selectedEndpoint.EndpointUrl);

            if (endpointUrl != null && endpointUrl.Scheme == uri.Scheme)
            {
                UriBuilder builder = new UriBuilder(endpointUrl);
                builder.Host = uri.DnsSafeHost;
                builder.Port = uri.Port;
                selectedEndpoint.EndpointUrl = builder.ToString();
            }

            // return the selected endpoint.
            return selectedEndpoint;
        }





    }
}
