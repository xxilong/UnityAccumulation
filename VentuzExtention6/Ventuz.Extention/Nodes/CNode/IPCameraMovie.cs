using System;
using System.ComponentModel;
using Ventuz.ImageProc;
using Ventuz.Kernel;
using Ventuz.Kernel.CModel;
using Ventuz.Kernel.Gfx;
using Ventuz.Kernel.Tools;
using Ventuz.Extention.Nodes.Helper;
using Ventuz.Kernel.IO;
using System.Runtime.Serialization;
using System.Drawing.Design;

namespace Ventuz.Extention.Nodes.CNodes
{
    [MetaData]
    class IPCameraMovie : TextureBase, IValueGenerator
    {
        private struct ParamStruct
        {
            public string videoId;
        }

        public IPCameraMovie(VSite site, string name) :
            base(site, name)
        {
            _Enable = new VBoolean(BGroup.A, this, true);
            _VideoId = new VString(BGroup.A, this, "Test");
        }

        [DefaultValue("Test")]
        [VBindable]
        [VCategory("Setting")]
        [Sort(1, 0)]
        [TextEditor(VTextType.PlainSingleLine, false)]
        [Editor("Ventuz.Designer.TextTypeEditor, Ventuz.Designer", typeof(UITypeEditor))]
        public string VideoId
        {
            get
            {
                return _VideoId.Value;
            }
            set
            {
                _VideoId.SetValue(value);
            }
        }

        [DefaultValue(true)]
        [VBindable]
        [VCategory("Setting")]
        [Sort(2, 0)]
        public bool Enable
        {
            get { return _Enable.Value; }
            set { _Enable.SetValue(value); }
        }

        protected override BGroup InputGroups => BGroup.A | BGroup.Z;

        protected override void Serialize(ISerializer serializer, StreamingContext context)
        {
            base.Serialize(serializer, context);
            serializer.WriteVersion(1);
            serializer.Write(_Enable);
            serializer.Write(_VideoId);
        }

        protected override void Deserialize(IDeserializer deserializer, StreamingContext context)
        {
            base.Deserialize(deserializer, context);
            int ver = deserializer.ReadVersion(1);
            _Enable = (VBoolean)deserializer.ReadObject();
            _VideoId = (VString)deserializer.ReadObject();
        }

        protected override BGroup ValidateComponent2(VRenderer renderer, VDevice device, BGroup groups)
        {
            if(job == null)
            {
                job = new TextureFromNative();
            }

            if ((groups & BGroup.A) != BGroup.None)
            {
                VResource oldResource = _Texture.Value;
                ParamStruct param = default(ParamStruct);
                param.videoId = _VideoId.Value;
                vresTexture = (VResTexture)renderer.ResourceManager.GetResource(this, oldResource, 
                    TextureGenerator.Singleton, param, device);
                _Texture.SetValue(vresTexture);
                groups |= BGroup.a;
            }

            if ((groups & BGroup.Z) != BGroup.None && vresTexture != null && _Enable.Value)
            {
                groups |= BGroup.z;

                VTexture t = job.CreateTextureFromNative(_VideoId.Value);
                if(t != null)
                {
                    vresTexture.SetTexture(t, "CV_" + _VideoId.Value);
                    groups |= BGroup.a;
                }
            }

            return groups;
        }

        private VResTexture vresTexture = null;
        private TextureFromNative job;
        private VBoolean _Enable;
        private VString _VideoId;

        private class TextureGenerator : IResourceGenerator
        {
            public MemoryPools ResourceMemoryPool => MemoryPools.Video;
            public PerfMon.Token PerformanceToken => VResourceManager.VResTextureToken;
            public VResource GenerateResource(IVComponent creator, VResourceManager manager, VResource reuse, ValueType parameterSet,
                VDevice device) => reuse != null ? reuse : new VResTexture(manager, device, null, "CV_" + ((ParamStruct)parameterSet).videoId);
            
            public static TextureGenerator Singleton = new TextureGenerator();
        }
    }
}
