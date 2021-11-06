using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using System.Reflection;

namespace oneshotpropick
{
    public class ItemOneshotProspectingPick : ItemProspectingPick
    {
        static Assembly _proinfoAssembly = null;

        public ItemOneshotProspectingPick():base()
        {

        }

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            if (_proinfoAssembly == null) 
            {
                var assem = GetProspectingInfoAssembly();
                if (assem != null)
                {
                    _proinfoAssembly = assem;
                    if (api.Side == EnumAppSide.Server)
                    {
                        var sapi = api as ICoreServerAPI;
                        sapi.Logger?.Notification($"Found ProspectorInfo!");
                    }
                    else if (api.Side == EnumAppSide.Client)
                    {
                        var capi = api as ICoreClientAPI;
                        capi.Logger?.Notification($"Found ProspectorInfo!");
                    }
                }
            }

        }

        protected Assembly GetProspectingInfoAssembly()
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            var assem = asms.SingleOrDefault(assembly => assembly.GetName().Name == "ProspectorInfo");
            return assem;
        }

        protected override void ProbeBlockDensityMode(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel) 
        {
            ProbeBlockDensityModeInternal(world, byEntity, itemslot, blockSel);

            if (_proinfoAssembly != null)
            {
                var type = _proinfoAssembly.GetType("ProspectorInfo.Map.ProspectorOverlayLayer+PrintProbeResultsPatch");
                MethodInfo meth = type.GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic);

                // ProspectorInfo's Harmony postfix emulation
                // ProspectorInfo requires 3 blockSel histories on Survival Mode
                meth.Invoke(null, new object[] { world, byEntity, itemslot, blockSel });
                meth.Invoke(null, new object[] { world, byEntity, itemslot, blockSel });
                meth.Invoke(null, new object[] { world, byEntity, itemslot, blockSel });
            }
        }

        protected virtual void ProbeBlockDensityModeInternal(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel)
        {
            // Vanilla propick operation with only one block breaking (same with Creative Mode)

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
