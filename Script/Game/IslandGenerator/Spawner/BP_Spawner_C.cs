using System;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Engine.KismetSystemLibrary;
using Script.Library;

namespace Script.IslandGenerator.Spawner
{
    public partial class BP_Spawner_C
    {
        public virtual void ReceiveBeginPlay()
        {
            AsyncLoad_h20_Classes();

            if (Auto_h20_Spawn)
            {
                TickAsyncLoadComplete();
            }
        }

        protected void TickAsyncLoadComplete()
        {
            FLatentActionInfo Info = new FLatentActionInfo();
            Info.UUID = UKismetMathLibrary.RandomInteger(1000);
            Info.ExecutionFunction = "OnAsyncLoadComplete";
            Info.CallbackTarget = this;
            Info.Linkage = 0;

            UKismetSystemLibrary.DelayUntilNextTick(
                this,
                Info
            );
        }

        protected void OnAsyncLoadComplete()
        {
            Counter++;
            Index_h20_Counter++;
            
            UKismetSystemLibrary.K2_SetTimerDelegate()
        }

        public virtual void AsyncLoad_h20_Classes()
        {
            Class_h20_Ref_h20_Index = 0;
            Async_h20_Complete = false;

            var Delegate = new FOnAssetClassLoaded();
            Delegate.Bind(OnAssetLoaded);

            Async_h20_Class();
        }

        public virtual void Async_h20_Class()
        {
            UClass ClassRef = SpawnTypes[0].ClassRef.LoadSynchronous();
            // @todo feature
            // UKismetSystemLibrary.LoadAssetClass(
            //     this,
            //     SpawnTypes[0].ClassRef.Get(),
            //     Delegate,
            //     new FLatentActionInfo()
            // );

            if (ClassRef.IsValid())
            {
                Class_h20_Ref_h20_Index++;
                if (Class_h20_Ref_h20_Index >= SpawnTypes.Num())
                {
                    Async_h20_Complete = true;
                }
                else
                {
                    Async_h20_Class();
                }
            }
        }

        protected virtual void OnAssetLoaded(TSubclassOf<UObject> Asset)
        {
        }
    }
}