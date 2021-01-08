using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Remoting4;

namespace VentuzRemoteControl.Clusters
{
    class ClusterManager
    {
        public void Start()
        {
            m_cluster.ClusterStateChanged += ClusterStateChanged;
            m_cluster.Log += Log;
            m_cluster.AddMachine(new IPEndPoint(IPAddress.Parse("127.0.0.1"), Cluster.DEFAULT_PORT));
            m_cluster.LogLevel = LogLevel.Debug;
            m_cluster.Start();
        }

        public void Stop()
        {
            m_cluster.Shutdown();
        }

        public async void ShowDataMode()
        {
            string mode = await m_cluster.DataModel(null);
            Console.WriteLine("DataMode: {0}", mode);

            var data = await m_cluster.ProjectDataItem(".testData", null);
            Console.WriteLine("testdata: {0}", data);

            m_cluster.ProjectDataItem(".testData", 80, null, null);
        }

        public async void ShowVentuzInfo()
        {
            var info = await m_cluster.GetVentuzInfo(null);
            Console.WriteLine("EditionName: {0}, GroupID: {1}, MachineID: {2}, MachineName: {3}",
                info.EditionName, info.GroupID, info.MachineID, info.MachineName);
            Console.WriteLine("Mode: {0}, PipeCount: {1}, PipeMode: {2}", info.Mode, info.PipeCount, info.PipeModes);
            Console.WriteLine("ProcessID: {0}, ProjectName: {1}, RateDen: {2}, RateNum: {3}", info.ProcessID, info.ProjectName, info.RateDen, info.RateNum);
            Console.WriteLine("SystemID: {0}, Version: {1}", info.SystemID, info.Version);
        }

        public async void ListScene(string topiid)
        {
            IID[] ids = await m_cluster.Scenes(topiid, true, null);
            foreach(var iid in ids)
            {
                Console.WriteLine("IID: {0}", iid);
                Console.WriteLine("MODE: {0}", await m_cluster.SceneModel(iid, null));
                Console.WriteLine();
            }
        }

        public async void LoadScene(string name)
        {
            FlaggedIID id = await m_cluster.Load("0000", name, LoadFlags.New | LoadFlags.Existing, null);
            SetScene(id.IID.ToString());
        }

        public async void SetScene(string id)
        {
            FlaggedIID fid = await m_cluster.PortStatus("0000", 0, true, id, null, null);
            Console.WriteLine(fid);
        }

        void ClusterStateChanged(object sender, EventArgs e)
        {
            ClusterState state = m_cluster.ClusterState;
            Console.WriteLine("Cluster Status Changed: " + state);

            if(state == ClusterState.Ok)
            {
                foreach(PortInfo portinfo in m_cluster.PipePorts)
                {
                    Console.WriteLine("Port: {0}", portinfo.ToString());
                }

                int pipeIndex = 0;
                while(m_cluster.TryGetLocalPipeInfo(pipeIndex, out PipeInfo info))
                {
                    Console.WriteLine("Pipe {0}:{1}X{2} IID={3}", pipeIndex, info.Width, info.Height, info.LayoutIID);
                    ++pipeIndex;
                }
            }
        }

        void Log(object sender, LogEventArgs e)
        {
            Console.WriteLine("Log: {0}", e);
        }

        private Cluster m_cluster = new Cluster { Name = "LocalControl" };
    }
}
