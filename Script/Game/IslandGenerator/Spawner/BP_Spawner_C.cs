using System;
using System.Threading.Tasks;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Engine.KismetSystemLibrary;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Core.Save;
using Script.IslandGenerator.Misc;
using Script.NavigationSystem;

namespace Script.IslandGenerator.Spawner
{
    [IsOverride]
    public partial class BP_Spawner_C
    {
        [IsOverride]
        public virtual void ReceiveBeginPlay()
        {
            AsyncLoad_h20_Classes();
            
            if (Auto_h20_Spawn)
            {
                DelayUntilNextTick();
            }
        }

        [IsOverride]
        public virtual void Ready_h20_To_h20_Spawn()
        {
            TickIfSpawn();
        }

        [IsOverride]
        public virtual void Finished_h20_Spawning()
        {
            (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Interactables();
            (UGameplayStatics.GetGameMode(GetWorld()) as IBPI_IslandPlugin_C)?.Spawning_h20_Complete();
        }

        [IsOverride]
        public virtual void Spawn_h20_Random()
        {
            DelayUntilNextTick();
        }

        [IsOverride]
        public virtual void Spawn_h20_Mesh_h20_Only()
        {
            Actor_h20_Switch = false;
            DelayUntilNextTick();
        }

        /**
         * 为了保持蓝图结构，保留了这个函数的命名，实际为同步加载
         */
        [IsOverride]
        public virtual void AsyncLoad_h20_Classes()
        {
            Class_h20_Ref_h20_Index = 0;
            Async_h20_Complete = false;

            Async_h20_Class();
        }

        /**
         * 实际加载资产的函数
         */
        [IsOverride]
        public virtual void Async_h20_Class()
        {
            UClass ClassRef = SpawnTypes[0].ClassRef.LoadSynchronous();
            if (ClassRef.IsValid())
            {
                Class_h20_Ref_h20_Index++;
                if (Class_h20_Ref_h20_Index > SpawnTypes.Num() - 1)
                {
                    Async_h20_Complete = true;
                }
                else
                {
                    // 这里递归调用是真 JB 深井冰
                    Async_h20_Class();
                }
            }
        }

        /**
         * 生成资产
         */
        [IsOverride]
        public virtual void SpawnAssets(TSubclassOf<AActor> Class, ST_SpawnData SpawnParams)
        {
            int Counter = 0;
            for (int i = 0; i < SpawnParams.Biome_h20_Count; ++i)
            {
                FVector Pos = new FVector();
                RandomBiomePoint(ref Pos);

                int Max = Seed.RandRange(0, SpawnParams.Spawn_h20_Per_h20_Biome);
                for (int j = 0; j < Max; ++j)
                {
                    FVector SpawnPos = new FVector();
                    UNavigationSystemV1.K2_GetRandomLocationInNavigableRadius(
                        GetWorld(),
                        Pos,
                        ref SpawnPos,
                        (float)SpawnParams.Biome_h20_Scale_h20__h20__h20__h20__h20_,
                        Nav_h20_Data,
                        null
                    );

                    FVector BiomePosition;
                    BPF_Cropout_C.Stepped_h20_Position(SpawnPos, this, out BiomePosition);

                    FRotator BiomeRotator = new FRotator();
                    BiomeRotator.Yaw =
                        UKismetMathLibrary.RandomFloatInRange(0f, SpawnParams.Random_h20_Rotation_h20_Range);

                    double Scale = UKismetMathLibrary.RandomFloatInRange(0f, SpawnParams.Scale_h20_Range + 1f);
                    FVector BiomeScale = UKismetMathLibrary.Conv_DoubleToVector(Scale);

                    FTransform Transform = UKismetMathLibrary.MakeTransform(
                        BiomePosition,
                        BiomeRotator,
                        BiomeScale
                    );

                    AActor Actor = GetWorld().SpawnActor<AActor>(
                        Class.Get(),
                        Transform,
                        null,
                        null,
                        ESpawnActorCollisionHandlingMethod.AlwaysSpawn
                    );

                    Counter++;

                    // BPI Island Plugin
                }
            }
        }

        [IsOverride]
        public virtual void Load_h20_Spawn(ST_SaveInteract NewParam = null)
        {
        }

        /**
         * 生成静态网格体
         */
        [IsOverride]
        public virtual void SpawnInst(UInstancedStaticMeshComponent Class = null, Single Radius = 0f,
            Int32 Biome_h20_Count = 0, Int32 Max_h20_Spawn = 0)
        {
            int Counter = 0;
            for (int i = 0; i <= Biome_h20_Count; ++i)
            {
                FVector Pos = new FVector();
                RandomBiomePoint(ref Pos);
                
                int Max = Seed.RandRange(0, Max_h20_Spawn);
                for (int j = 0; j <= Max; ++j)
                {
                    FVector SpawnPos = new FVector();
                    UNavigationSystemV1.K2_GetRandomLocationInNavigableRadius(
                        this,
                        Pos,
                        ref SpawnPos,
                        Radius,
                        Nav_h20_Data,
                        null
                    );

                    FVector NewLocation = new FVector(SpawnPos.X, SpawnPos.Y, 0);
                    FRotator NewRotator = new FRotator();

                    double Alpha = (Pos - SpawnPos).Length() / Radius;
                    FVector NewScale = UKismetMathLibrary.Conv_DoubleToVector(UKismetMathLibrary.Lerp(0.8, 1.5, Alpha));

                    FTransform InstanceTransform = UKismetMathLibrary.MakeTransform(NewLocation, NewRotator, NewScale);
                    Class.AddInstance(InstanceTransform, true);

                    Counter++;
                }
            }
        }

        protected async void DelayUntilNextTick()
        {
            Counter = 0;
            Index_h20_Counter = 0;

            TickIfSpawn();
        }

        /**
         * 异步检查导航网格有没有重建完成，资产类是不是加载完了，然后生成资产
         */
        protected async void TickIfSpawn()
        {
            UNavigationSystemV1 Nav = UNavigationSystemV1.GetNavigationSystem(this);
            bool bBuilt = UNavigationSystemV1.IsNavigationBeingBuilt(Nav);
            while (bBuilt || !Async_h20_Complete)
            {
                await Task.Delay(500);
            }

            // 这个变量用来切换生成资产还是网格体
            // 这个生成流程写的啥？？？
            if (Actor_h20_Switch)
            {
                ST_SpawnData Data = SpawnTypes[Index_h20_Counter];
                TSoftClassPtr<AActor> ClassRef = Data.ClassRef;
                SpawnAssets(ClassRef.Get(), Data);
                
                Index_h20_Counter++;
                if (Index_h20_Counter >= SpawnTypes.Num())
                {
                    Index_h20_Counter = 0;
                    Actor_h20_Switch = false;
                }
                
                TickIfSpawn();
            }
            else
            {
                UInstancedStaticMeshComponent Component = AddComponentByClass(
                    UInstancedStaticMeshComponent.StaticClass(),
                    false,
                    new FTransform(),
                    false
                ) as UInstancedStaticMeshComponent;
                
                ST_SpawnInstance Data = SpawnInstances[Index_h20_Counter];
                Component.SetStaticMesh(Data.ClassRef);

                SpawnInst(
                    Component,
                    (float)Data.Biome_h20_Scale_h20__h20__h20__h20__h20_,
                    Data.Biome_h20_Count,
                    Data.Spawn_h20_Per_h20_Biome
                );

                Index_h20_Counter++;
                if (Index_h20_Counter >= SpawnInstances.Num())
                {
                    if (Call_h20_Save)
                    {
                        Finished_h20_Spawning();
                    }
                }
                else
                {
                    TickIfSpawn();
                }
            }
        }

        /**
         * 随机一个物种位置
         */
        protected void RandomBiomePoint(ref FVector Point)
        {
            FVector Origin = new FVector(0f, 0f, 0f);
            UNavigationSystemV1.K2_GetRandomLocationInNavigableRadius(
                GetWorld(),
                Origin,
                ref Point,
                10000f,
                Nav_h20_Data,
                null
            );
        }
    }
}