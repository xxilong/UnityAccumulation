using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Nodes.Manop;
using Ventuz.Kernel.CModel;
using Ventuz.Kernel;
using System.Drawing;

namespace Ventuz.Extention.Test
{
    class Script:ScriptBace,System.IDisposable
    {
        
        private string[] CnLocation;
        private string[] EnLocation;
        private float[] Longitude;
        private float[] Latitude;
        private float[] BuildOver;
        private Color DoneColor,UndoneColor;

        private bool isChinese = true;
        private List<VLinkable> locationList = new List<VLinkable>();
        private VLinkable demo;
        private VLinkable parent;
        // This member is used by the Validate() method to indicate
        // whether the Generate() method should return true or false
        // during its next execution.
        private bool changed;

        // This Method is called if the component is loaded/created.
        public Script()
        {
            // Note: Accessing input or output properties from this method
            // will have no effect as they have not been allocated yet.
        }

        // This Method is called if the component is unloaded/disposed
        public virtual void Dispose()
        {
        }

        // This Method is called if an input property has changed its value
        public override void Validate()
        {            
                      
            // Remember: set changed to true if any of the output 
            // properties has been changed, see Generate()
        }

        public void SpawnNode()
        {
            //找到需要复制的节点
            demo = HierarchyNode.FindHierarchyNode("LocationDemo");
            //找到父节点
            parent = HierarchyNode.FindHierarchyNode("Parent");
            //找到所有子节点
            HierarchyNode.DeleteChildsWithContent(parent);
            locationList.Clear();
            //创建新的子节点，设置旋转和显影
            for (int i = 0; i < CnLocation.Length; i++)
            {
                if (!string.IsNullOrEmpty(CnLocation[i])
                    && Longitude[i] != 0
                    && Latitude[i] != 0)
                {
                    VLinkable vl = HierarchyNode.CopyNodes(demo, parent);
                    vl.Name = CnLocation[i];
                    ContentNode.SetProperty(vl, "RotationY", -Longitude[i]);
                    ContentNode.SetProperty(vl, "RotationZ", Latitude[i]);
                    VentuzNode.SetSubNodeProperty(vl, "BlockText", "Text", CnLocation[i]);
                    isChinese = true;
                    if (BuildOver[i] == 0)
                    {
                        VentuzNode.SetSubNodeProperty(vl, "CircleMaterial", "MaterialLightingModelBaseColor", DoneColor);
                    }
                    else if (BuildOver[i] == 1)
                    {
                        VentuzNode.SetSubNodeProperty(vl, "CircleMaterial", "MaterialLightingModelBaseColor", UndoneColor);
                    }

                    vl.Blocked = false;
                    locationList.Add(vl);
                }
            }

            changed = true;
        }
        // This Method is called every time before a frame is rendered.
        // Return value: if true, Ventuz will notify all nodes bound to this
        //               script node that one of the script's outputs has a
        //               new value and they therefore need to validate. For
        //               performance reasons, only return true if output
        //               values really have been changed.
        public override bool Generate()
        {
            if (changed)
            {
                changed = false;
                return true;
            }

            return false;
        }

        public VLinkable CopyNodes(VLinkable src, VLinkable parent)
        {
            VAdvancedSelection sel = new VAdvancedSelection(SelectionType.WithChildren|SelectionType.WithSources, uint.MaxValue);
            sel.BeginUpdate();
            sel.Add(src);
            sel.EndUpdate();

            byte[] bindata = sel.Serialize();
            string data = Convert.ToBase64String(bindata);
            return  HierarchyNode.LoadFromBase64Archive(data, parent);
        }
        // This Method is called if the function/method Reset is invoked by the user or a bound event.
        // Return true, if this component has to be revalidated!
        public bool OnReset(int arg)
        {
            SpawnNode();
            return false;
        }
        // This Method is called if the function/method ChangeLanguage is invoked by the user or a bound event.
        // Return true, if this component has to be revalidated!
        public bool OnChangeLanguage(int arg)
        {
            //创建新的子节点，设置旋转和显影            
            for (int i = 0; i < locationList.Count; i++)
            {
                if (isChinese)
                {
                    VentuzNode.SetSubNodeProperty(locationList[i], "BlockText", "Text", EnLocation[i]);                    
                }
                else
                {
                    VentuzNode.SetSubNodeProperty(locationList[i], "BlockText", "Text", CnLocation[i]);
                }
                HierarchyNode.FindContentNode(locationList[i], "BlockText").Invalidate(BGroup.All);
            }
            isChinese = !isChinese;
            changed = true;
            return false;
        }

    }
    
}
