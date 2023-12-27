using System;
using System.Threading;
using System.Threading.Tasks;
using Script.AIModule;
using Script.AudioModulation;
using Script.Common;
using Script.CommonUI;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Audio.DATA.ControlBus;
using Script.Game.Blueprint.Core.Extras;
using Script.Game.Blueprint.Core.Save;
using Script.Game.Blueprint.Interactable;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.Blueprint.Villagers;
using Script.Game.UI.Game;
using Script.IslandGenerator.Misc;
using Script.IslandGenerator.Spawner;
using Script.Library;
using Script.NavigationSystem;

namespace Script.Game.Blueprint.Core.GameMode
{
    [IsOverride]
    public partial class BP_GM_C
    {
        /*
         * Start
         */
        [IsOverride]
        public override void ReceiveBeginPlay()
        {
            /*
             * Set loading screen to animate out, reset render target
             */
            BPF_Cropout_C.Get_h20_Cropout_h20_GI(this, out var GI);

            GI.TransitionOut();

            GI.Start_h20_Game_h20_Offset = UKismetSystemLibrary.GetGameTimeInSeconds(this);

            UKismetRenderingLibrary.ClearRenderTarget2D(this, Unreal.LoadObject<RT_GrassMove>(this),
                FLinearColor.Black);

            /*
             * Setup Island
             */
            GetSpawnRef();

            /*
             * Add Game HUD to screen
             */
            UI_HUD = Unreal.CreateWidget<UI_Layer_Game_C>(UGameplayStatics.GetGameInstance(this));

            UI_HUD.AddToViewport();

            UI_HUD.ActivateWidget();
        }

        [IsOverride]
        public override void ReceiveEndPlay(EEndPlayReason EndPlayReason)
        {
            IslandGenCompleteTokenSource?.Cancel();

            EndGameTokenSource?.Cancel();
        }

        /*
         * Once Island has finished building, check if save file exists.
         * If so, load data from Save file.
         * If not, being spawning assets
         */
        [IsOverride]
        public void Island_h20_Gen_h20_Complete()
        {
            IslandGenCompleteTokenSource = new CancellationTokenSource();

            OnIslandGenComplete();
        }

        /*
         * Adds a resource value to the resource Map
         */
        [IsOverride]
        public void Add_h20_Resource(E_ResourceType Resource = E_ResourceType.None, Int32 Value = 0)
        {
            if (!Resources.Contains(Resource))
            {
                Resources.Add(Resource, 0);
            }

            Resources[Resource] += Value;

            Update_h20_Resources.Broadcast(Resource, Resources[Resource]);

            (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Resources(Resources);
        }

        /*
         * UI Interactions
         */
        [IsOverride]
        public void End_h20_Game(Boolean Win = false)
        {
            if (bDoOnce)
            {
                bDoOnce = false;

                EndGameTokenSource = new CancellationTokenSource();

                OnEndGame(Win);
            }
        }

        /*
         * UI Interactions
         */
        [IsOverride]
        public void Add_h20_UI(TSubclassOf<UCommonActivatableWidget> NewParam)
        {
            UI_HUD.Add_h20_Stack_h20_Item(NewParam);
        }

        [IsOverride]
        public void Remove_h20_Current_h20_UI_h20_Layer()
        {
            UI_HUD.Pull_h20_Current_h20_Active_h20_Widget();
        }

        /*
         * Spawn Town Hall
         */
        private void BeginASyncSpawning()
        {
            var OutActors = new TArray<AActor>();

            UGameplayStatics.GetAllActorsOfClass(this, BP_SpawnMarker_C.StaticClass(), ref OutActors);

            var Index = UKismetMathLibrary.RandomInteger(OutActors.Num());

            BPF_Cropout_C.Stepped_h20_Position(OutActors[Index].K2_GetActorLocation(), this, out var NewParam1);

            Town_h20_Hall = GetWorld().SpawnActor<AActor>(TownHall_Ref.LoadSynchronous(),
                new FTransform { Translation = NewParam1 });

            SpawnVillagers(3);

            /*
             * Spawn Interactables
             */
            SpawnRef.Spawn_h20_Random();
        }

        private void SpawnVillagers(Int32 Add = 0)
        {
            /*
             * Create villagers
             */
            for (var i = 1; i < Add; i++)
            {
                Spawn_h20_Villager();
            }

            /*
             * Update Villagers
             */
            Update_h20_Villagers.Broadcast(Villager_h20_Count);

            (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Villagers();
        }

        private void GetSpawnRef()
        {
            var OutActors = new TArray<AActor>();

            UGameplayStatics.GetAllActorsOfClass(this, BP_Spawner_C.StaticClass(), ref OutActors);

            SpawnRef = OutActors[0] as BP_Spawner_C;
        }

        private void SpawnLoadedInteractables()
        {
            (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C).Get_h20_All_h20_Interactables(out var NewParam);

            foreach (var SaveInteract in NewParam)
            {
                var BP_Interactable =
                    GetWorld().SpawnActor<BP_Interactable_C>(SaveInteract.Type.Get(), SaveInteract.Location);

                BP_Interactable.Progression_h20_State = 0.0f;

                BP_Interactable.Require_h20_Build = SaveInteract.Tag == "Build";

                BP_Interactable.Set_h20_Progressions_h20_State(SaveInteract.Health);

                if (SaveInteract.Type.Get() == TownHall_Ref)
                {
                    Town_h20_Hall = BP_Interactable;
                }
            }
        }

        private void LoadVillagers()
        {
            var BPI_GI = UGameplayStatics.GetGameInstance(this) as IBPI_GI_C;

            BPI_GI.Get_h20_Save(out var Save_h20_Data);

            foreach (var Villager in Save_h20_Data.Villagers)
            {
                var BPI_Villager = UAIBlueprintHelperLibrary.SpawnAIFromClass(this, Villager_Ref.Get(), null,
                    new FVector { X = Villager.Location.X, Y = Villager.Location.Y, Z = 42.0f }) as IBPI_Villager_C;

                BPI_Villager.Change_h20_Job(Villager.Task);
            }

            Update_h20_Villagers.Broadcast(Save_h20_Data.Villagers.Num());

            BPI_GI.Update_h20_All_h20_Resources(Resources);
        }

        [IsOverride]
        private void Spawn_h20_Villager()
        {
            var Origin = new FVector();

            var BoxExtent = new FVector();

            Town_h20_Hall.GetActorBounds(false, ref Origin, ref BoxExtent, false);

            var Min = Math.Min(BoxExtent.X, BoxExtent.Y);

            var RandomUnitVector = UKismetMathLibrary.RandomUnitVector();

            var Result = new FVector(RandomUnitVector.X * Min * 2.0,
                RandomUnitVector.Y * Min * 2,
                RandomUnitVector.Z * Min * 2);

            Result += Origin;

            var RandomLocation = new FVector();

            UNavigationSystemV1.K2_GetRandomReachablePointInRadius(this,
                new FVector { X = Result.X, Y = Result.Y }, ref RandomLocation,
                500.0f, null, null);

            UAIBlueprintHelperLibrary.SpawnAIFromClass(this, Villager_Ref.Get(), null, RandomLocation);

            Villager_h20_Count++;
        }

        [IsOverride]
        public void Check_h20_Resource(E_ResourceType NewParam1, out Boolean NewParam, out Int32 NewParam3)
        {
            if (Resources.Contains(NewParam1))
            {
                NewParam = true;

                NewParam3 = Resources[NewParam1];
            }
            else
            {
                NewParam = false;

                NewParam3 = 0;
            }
        }

        [IsOverride]
        public void Remove_h20_Target_h20_Resource(E_ResourceType NewParam = E_ResourceType.None, Int32 NewParam1 = 0)
        {
            /*
             * Update the target resource
             */
            if (Resources.Contains(NewParam))
            {
                Resources[NewParam] -= NewParam1;

                Update_h20_Resources.Broadcast(NewParam, Resources[NewParam]);
            }

            /*
             * If food drops below 0, show end game screen with Lose condition
             */
            if (Resources[E_ResourceType.Food] <= 0)
            {
                End_h20_Game();
            }
        }

        [IsOverride]
        public void Get_h20_Current_h20_Resources(out TMap<E_ResourceType, Int32> Resources)
        {
            Resources = this.Resources;
        }

        private async void OnIslandGenComplete()
        {
            while (!IslandGenCompleteTokenSource.IsCancellationRequested)
            {
                await Task.Delay(100);

                IslandGenCompleteTokenSource.Cancel();

                var BPI_GI = UGameplayStatics.GetGameInstance(this) as IBPI_GI_C;

                BPI_GI.Check_h20_Save_h20_Bool(out var Save_h20_Exist);

                if (Save_h20_Exist)
                {
                    SpawnLoadedInteractables();

                    BPI_GI.Get_h20_Save(out var Save_h20_Data);

                    Resources = Save_h20_Data.Resources;

                    SpawnRef.Spawn_h20_Mesh_h20_Only();

                    LoadVillagers();

                    UAudioModulationStatics.SetGlobalBusMixValue(this, Unreal.LoadObject<Cropout_Music_NewMap>(this),
                        0.0f, 0.0f);
                }
                else
                {
                    BeginASyncSpawning();

                    UAudioModulationStatics.SetGlobalBusMixValue(this, Unreal.LoadObject<Cropout_Music_NewMap>(this),
                        1.0f, 0.0f);
                }
            }
        }

        private async void OnEndGame(Boolean Win)
        {
            while (!EndGameTokenSource.IsCancellationRequested)
            {
                await Task.Delay(3000);

                EndGameTokenSource.Cancel();

                UI_HUD.End_h20_Game(Win);
            }
        }

        private CancellationTokenSource IslandGenCompleteTokenSource;

        private CancellationTokenSource EndGameTokenSource;

        private Boolean bDoOnce = true;
    }
}