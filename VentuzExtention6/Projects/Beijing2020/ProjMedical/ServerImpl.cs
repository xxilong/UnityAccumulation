using System;
using Thrift.Protocol;
using Thrift.Transport;
using com.everythinghouse.movingtv;

public class ServerImpl : IDisposable
{
    private Server.Client mClient;
    private TTransport mTransport;

    public Server.Client GetClient(string serverIp, int serverPort)
    {
        if(mClient == null)
        {
            mTransport = new TSocket(serverIp, serverPort);
            TProtocol protocol = new TBinaryProtocol(mTransport);
            mClient = new Server.Client(protocol);
            mTransport.Open();
        }
        return mClient;
    }
    public void Dispose()
    {
        if(mTransport != null)
        {
            mTransport.Close();
            mClient = null;
            mTransport = null;
        }
    }
}
