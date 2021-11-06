using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using System.Reflection;

namespace oneshotpropick
{
    public class ItemOneshotProspectingPick : ItemProspectingPick
    {
        public ItemOneshotProspectingPick():base()
        {
            System.Diagnostics.Debug.WriteLine("Created!");

        }

        protected override void ProbeBlockDensityMode(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel) 
        {
            System.Diagnostics.Debug.WriteLine($"Invoked Side:{world.Side}");

            ProbeBlockDensityModeInternal(world, byEntity, itemslot, blockSel);

            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            /*            foreach (Assembly a in asms)
                            System.Diagnostics.Debug.WriteLine(a.FullName);
            */
            var assem = asms.SingleOrDefault(assembly => assembly.GetName().Name == "ProspectorInfo");
            //Type type = Type.GetType("ProspectorInfo.Map.ProspectorOverlayLayer, ProspectorInfo");
            //System.Diagnostics.Debug.WriteLine($"OverlayLayer:{type == null}");
            //var type = assem.GetType("ProspectorInfo.Map.ProspectorOverlayLayer+PrintProbeResultsPatch");
            var type = Type.GetType("ProspectorInfo.Map.ProspectorOverlayLayer+PrintProbeResultsPatch,ProspectorInfo");
            //var type = assem.GetType("ProspectorInfo.Map.ProspectorOverlayLayer");
            MethodInfo meth = type.GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic);
            //FieldInfo fi = type.GetField("blocksSinceLastSuccessList", BindingFlags.Static | BindingFlags.NonPublic);
            //var val = fi.GetValue(null) as List<BlockSelection>;

            System.Diagnostics.Debug.WriteLine($"{world.Side}");
            //System.Diagnostics.Debug.WriteLine($"pros:{val.Count}");
            System.Diagnostics.Debug.WriteLine($"Hoge");
            /*            val.Add(blockSel);
                        val.Add(blockSel);
                        val.Add(blockSel);
            */
            meth.Invoke(null, new object[] { world, byEntity, itemslot, blockSel });
            meth.Invoke(null, new object[] { world, byEntity, itemslot, blockSel });
            meth.Invoke(null, new object[] { world, byEntity, itemslot, blockSel });

        }

        protected virtual void ProbeBlockDensityModeInternal(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel)
        {

            IPlayer byPlayer = null;
            if (byEntity is EntityPlayer) byPlayer = world.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

            Block block = world.BlockAccessor.GetBlock(blockSel.Position);
            float dropMul = 1f;
            if (block.BlockMaterial == EnumBlockMaterial.Ore || block.BlockMaterial == EnumBlockMaterial.Stone) dropMul = 0;

            block.OnBlockBroken(world, blockSel.Position, byPlayer, dropMul);

            if (!block.Code.Path.StartsWith("rock") && !block.Code.Path.StartsWith("ore")) return;

            IServerPlayer splr = byPlayer as IServerPlayer;

            if (splr == null) return;

            PrintProbeResults(world, splr, itemslot, blockSel.Position);

            return;
        }
    }
}
