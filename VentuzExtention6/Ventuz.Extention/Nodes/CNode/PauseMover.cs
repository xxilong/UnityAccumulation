using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Designer.Shared;
using Ventuz.Kernel;
using Ventuz.Kernel.CModel;
using Ventuz.Kernel.Gfx;
using Ventuz.Kernel.IO;
using Ventuz.Kernel.Sys;
using Ventuz.Kernel.Tools;

namespace Ventuz.Extention.Nodes.CNode
{
    [CustomVerbs("Ventuz.Designer.Wizards.MoverCV, Ventuz.Designer", typeof(ICustomVerbs))]
    [MetaData]
    public class PauseMover : VComponent, IValueGenerator
    {
        public override VSourceType SourceType
        {
            get
            {
                return VSourceType.Animation;
            }
        }

        protected override BGroup InputGroups
        {
            get
            {
                return BGroup.A | BGroup.B | BGroup.Z;
            }
        }

        [ManagedIcon("AppIcons.Misc.Reset")]
        [HelpDescription("Restart the Mover")]
        [VCategory("Control")]
        private void ResetInvoke(object sender, EventArgs e)
        {
            if (this.valuesOk)
            {
                this.oneShot = 0;
                base.Invalidate(BGroup.Z);
            }
        }

        public PauseMover(VSite site, string name) : base(site, name)
        {
            _Mode = new VInt32(BGroup.A | BGroup.Z, this, 1);
            _Duration = new VSingle(BGroup.A | BGroup.Z, this, 1f);
            _Swing = new VBoolean(BGroup.A | BGroup.Z, this, true);
            _Pause = new VBoolean(BGroup.A | BGroup.Z, this, false);
            _Function = new VInt32(BGroup.A | BGroup.Z, this, 0);
            _Min = new VSingle(BGroup.A | BGroup.Z, this, -1f);
            _Max = new VSingle(BGroup.A | BGroup.Z, this, 1f);
            _Value = new VSingle(BGroup.z, this, 0f);
            _OnRepeat = new VStaticEvent(BGroup.b, this, "OnRepeat", "Events", "Fired if the Mover finished one period.");
            _Reset = new VStaticMethod(BGroup.Z, this, "Reset", "ResetInvoke");
            _Min.Favored = true;
            _Max.Favored = true;
        }


        [VCategory("Control")]
        [VBindable]
        [DefaultValue(false)]
        public bool Pause
        {
            get => _Pause.Value;
            set => _Pause.SetValue(value);
        }


        [VCategory("Speed")]
        [VBindable]
        [DefaultValue(MoverMode.Infinite)]
        public MoverMode Mode
        {
            get
            {
                return (MoverMode)this._Mode.Value;
            }
            set
            {
                this._Mode.SetValue((int)value);
            }
        }

        [VBindable]
        [Unit("s")]
        [DefaultValue(1f)]
        [VCategory("Speed")]
        [MinMax(0f)]
        public float Duration
        {
            get
            {
                return this._Duration.Value;
            }
            set
            {
                this._Duration.SetValue(value);
            }
        }

        // Token: 0x17001334 RID: 4916
        // (get) Token: 0x06004503 RID: 17667 RVA: 0x0002DD65 File Offset: 0x0002BF65
        // (set) Token: 0x06004504 RID: 17668 RVA: 0x0002DD72 File Offset: 0x0002BF72
        [Sort(10, 0)]
        [VCategory("Range")]
        [VBindable]
        [DefaultValue(MoveFunction.Linear)]
        public MoveFunction Function
        {
            get
            {
                return (MoveFunction)this._Function.Value;
            }
            set
            {
                this._Function.SetValue((int)value);
            }
        }

        [DefaultValue(false)]
        [VBindable]
        [Sort(15, 0)]
        [VCategory("Range")]
        public bool Swing
        {
            get
            {
                return this._Swing.Value;
            }
            set
            {
                this._Swing.SetValue(value);
            }
        }

        [VBindable]
        [Sort(20, 0)]
        [DefaultValue(-1f)]
        [VCategory("Range")]
        public float Min
        {
            get
            {
                return this._Min.Value;
            }
            set
            {
                this._Min.SetValue(value);
            }
        }

        [VBindable]
        [Sort(30, 0)]
        [DefaultValue(1f)]
        [VCategory("Range")]
        public float Max
        {
            get
            {
                return this._Max.Value;
            }
            set
            {
                this._Max.SetValue(value);
            }
        }

        [VBindable(VBindableSupport.Source)]
        [VCategory("Value")]
        public float Value
        {
            get
            {
                return this._Value.Value;
            }
        }

        protected override void Serialize(ISerializer serializer, StreamingContext context)
        {
            base.Serialize(serializer, context);
            serializer.WriteVersion(4);
            serializer.Write(this._Mode);
            serializer.Write(this._Duration);
            serializer.Write(this._Swing);
            serializer.Write(this._Function);
            serializer.Write(this._Min);
            serializer.Write(this._Max);
            serializer.Write(this._Value);
            serializer.Write(this._OnRepeat);
            serializer.Write(this._Reset);
            serializer.Write(this._Pause);
        }

        protected override void Deserialize(IDeserializer deserializer, StreamingContext context)
        {
            base.Deserialize(deserializer, context);
            deserializer.ReadVersion(4);
            this._Mode = (VInt32)deserializer.ReadObject();
            this._Duration = (VSingle)deserializer.ReadObject();
            this._Swing = (VBoolean)deserializer.ReadObject();
            this._Function = (VInt32)deserializer.ReadObject();
            this._Min = (VSingle)deserializer.ReadObject();
            this._Max = (VSingle)deserializer.ReadObject();
            this._Value = (VSingle)deserializer.ReadObject();
            this._OnRepeat = (VStaticEvent)deserializer.ReadObject();
            this._Reset = (VStaticMethod)deserializer.ReadObject();
            this._Pause = (VBoolean)deserializer.ReadObject();
        }

        protected override BGroup ValidateComponent2(VRenderer renderer, VDevice device, BGroup groups)
        {
            if ((groups & BGroup.A) != BGroup.None)
            {
                this.duration = (double)this._Duration.Value;
                this.min = (double)this._Min.Value;
                this.max = (double)this._Max.Value;
                this.range = this.max - this.min;
                this.swing = this._Swing.Value;
                this.func = (MoveFunction)this._Function.Value;
                MoverMode value = (MoverMode)this._Mode.Value;
                if (this.mode != value)
                {
                    this.lastTime = VRenderer.RendererUpTime;
                    this.relTime = 0.0;
                    this.mode = value;
                    this.oneShot = -1;
                }
                this.valuesOk = true;
            }
            if ((groups & BGroup.Z) != BGroup.None)
            {
                this.GenerateValue(renderer, ref groups);
            }
            return groups;
        }

        public void GenerateValue(VRenderer renderer, ref BGroup groups)
        {
            if (valuesOk)
            {
                GenerateValueOnOK(renderer, ref groups);
            }

            lastTime = VRenderer.RendererUpTime;
        }

        private void GenerateValueOnOK(VRenderer renderer, ref BGroup groups)
        {
            double outValue = min;

            if(_Pause.Value)
            {
                return;
            }

            if(duration <= 0.0)
            {
                return;
            }

            double period = 0.0;
            double curtime = 0.0;

            switch(oneShot)
            {
                case -1:  // 修改了模式
                    relTime = 0.0;
                    curtime = 0.0;
                    lastPeriod = period;
                    break;

                case 0: // 点了 Reset
                    curtime = 0.0;
                    lastPeriod = period;
                    relTime = 0.0;
                    lastTime = VRenderer.RendererUpTime;
                    oneShot = 1;
                    break;

                case 1:
                    relTime += (VRenderer.RendererUpTime - lastTime) / duration;
                    if (relTime < 1.0)
                    {
                        curtime = relTime;
                        period = lastPeriod;

                    }
                    else if (mode == MoverMode.OneShot)
                    {
                        oneShot = 2;
                        curtime = 1.0;
                        period = lastPeriod + 1.0;
                    }
                    else if (mode == MoverMode.Infinite)
                    {
                        relTime -= 1.0;
                        curtime = relTime;
                        period = lastPeriod + 1.0;
                        lastTime = VRenderer.RendererUpTime;
                        oneShot = 1;
                    }
                    break;

                default:
                    period = this.lastPeriod;
                    curtime = 1.0;
                    break;
            }                       

            switch (this.func)
            {
                case MoveFunction.Linear:
                    if (this.swing)
                    {
                        outValue = (1.0 - Math.Abs(curtime - 0.5) * 2.0) * this.range + this.min;
                    }
                    else
                    {
                        outValue = curtime * this.range + this.min;
                    }
                    break;
                case MoveFunction.Sine:
                    if (this.swing)
                    {
                        outValue = (Math.Sin(6.2831853071795862 * curtime) + 1.0) / 2.0 * this.range + this.min;
                    }
                    else
                    {
                        outValue = Math.Sin(3.1415926535897931 * curtime) * this.range + this.min;
                    }
                    break;
                case MoveFunction.Cosine:
                    if (this.swing)
                    {
                        outValue = (Math.Cos(6.2831853071795862 * curtime) + 1.0) / 2.0 * this.range + this.min;
                    }
                    else
                    {
                        outValue = (Math.Cos(3.1415926535897931 * curtime) + 1.0) / 2.0 * this.range + this.min;
                    }
                    break;
                case MoveFunction.Alternate:
                    if (curtime < 0.5)
                    {
                        outValue = this.min;
                    }
                    else
                    {
                        outValue = this.max;
                    }
                    break;
                case MoveFunction.Infinite:
                    outValue = VRenderer.RendererUpTime / this.duration * this.range + this.min;
                    break;
            }

            if (this._Value.SetValue((float)outValue))
            {
                groups |= BGroup.z;
            }

            if (period != this.lastPeriod)
            {
                this.lastPeriod = period;
                this._OnRepeat.EnqueueFire(EventArgs.Empty);
            }
        }

        private VStaticEvent _OnRepeat;
        private VStaticMethod _Reset;
        private VInt32 _Mode;
        private VSingle _Duration;
        private VInt32 _Function;
        private VBoolean _Swing;
        private VSingle _Min;
        private VSingle _Max;
        private VSingle _Value;
        private VBoolean _Pause;

        private double lastPeriod;
        private bool valuesOk;
        private double duration;
        private double min;
        private double max;
        private double range;
        private bool swing;

        private MoveFunction func;
        private MoverMode mode;

        private double lastTime;
        private double relTime;
        private int oneShot;

        public enum MoveFunction
        {
            Linear,
            Sine,
            Cosine,
            Alternate,
            Infinite
        }

        public enum MoverMode
        {
            OneShot = 1,
            Infinite
        }
    }
}
