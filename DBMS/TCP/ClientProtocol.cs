using DTO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DBMS.TCP
{
    public delegate TransferObject ExcecuteCommandDelegate(string commandTextLine);
    public class ClientProtocol : IDisposable
    {
        /// <summary>
        /// TCP Client
        /// </summary>
        private readonly TcpClient _tcpClient;
        /// <summary>
        /// Stream to read/write tcp data
        /// </summary>
        private NetworkStream _tcpStream;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly Logger _logger;
        /// <summary>
        /// Client Ip Address
        /// </summary>
        private readonly string _clientIp;

        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly CancellationToken _token;

        public readonly DateTime Started;
        public ExcecuteCommandDelegate ExcecuteCommand { get; set; }

        public ClientProtocol(TcpClient tcpClient)
        {
            Started = DateTime.Now;
            _tcpClient = tcpClient;
            _cancelTokenSource = new CancellationTokenSource();
            _token = _cancelTokenSource.Token;
            _logger = LogManager.GetCurrentClassLogger();
            _clientIp = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();

            _logger.Trace($"IP: {_clientIp}. Init protocol.");
        }


        async public Task StartAsync()
        {
            //_tcpStream = _tcpClient.GetStream();

            _logger.Trace($"IP: {_clientIp}. Got stream.");

            while(!_token.IsCancellationRequested)
            {
                if(!_tcpClient.Connected || _tcpClient.Available == 0)
                {
                    _cancelTokenSource.Cancel();
                    break;
                }
                else if(_tcpClient.Available > 0)
                {
                    _tcpStream = _tcpClient.GetStream();
                }

                var data = await readStreamAsync();

                if(data.IndexOf("LOG_IN:") == 0)
                {

                }
                else if(data.IndexOf("LOG_OUT:") == 0)
                {
                    _cancelTokenSource.Cancel();
                }
                else
                {
                    var result = ExcecuteCommand(data);
                    var json = JsonConvert.SerializeObject(result, Formatting.None);
                    var bytes = ASCIIEncoding.UTF8.GetBytes(json);
                    await writeStreamAsync(bytes);
                }
            }
            closeConnection();
            return;
        }

        async Task<string> readStreamAsync()
        {
            _logger.Trace($"IP: {_clientIp}. Reading stream.");
            var length = _tcpClient.ReceiveBufferSize;
            byte[] bytes = new byte[length];
            var data = String.Empty;
            if (_tcpStream.CanRead)
            {
                await _tcpStream.ReadAsync(bytes, 0, length);
                if (length != 0)
                {
                    var buffer = Encoding.UTF8.GetString(bytes, 0, length);
                    data = buffer.Substring(0, buffer.IndexOf('\0'));
                    _logger.Trace($"IP: {_clientIp}. Stream was read [{data}].");
                }
            }
            else
            {
                _logger.Trace($"IP: {_clientIp}. Stream was not read [{data}].");
            }
            return data;
        }

        async Task writeStreamAsync(byte[] bytes)
        {
            _logger.Trace($"IP: {_clientIp}. Writing to stream.");
            if (_tcpStream.CanWrite)
            {
                await _tcpStream.WriteAsync(bytes, 0, bytes.Length);
                _logger.Trace($"IP: {_clientIp}. Data was written.");
            }
            else
            {
                _logger.Trace($"IP: {_clientIp}. Data was not written.");
            }
            _tcpStream?.Close();
            return;
        }

        private void closeConnection()
        {
            _tcpStream?.Close();
            _tcpClient?.Close();
        }

        public void Dispose()
        {
            closeConnection();
        }
    }
}
