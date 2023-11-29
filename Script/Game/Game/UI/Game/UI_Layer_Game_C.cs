using System;
using System.Threading;
using System.Threading.Tasks;
using Script.Common;
using Script.CommonUI;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Core.Player;
using Script.Game.Blueprint.Core.Player.Input;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.UI.UI_Elements;
using Script.Library;

namespace Script.Game.UI.Game
{
    [IsOverride]
    public partial class UI_Layer_Game_C
    {
        [IsOverride]
        public override void OnInitialized()
        {
            // Initialize();
        }

        [IsOverride]
        public override void PreConstruct(bool IsDesignTime)
        {
            Initialize();
        }

        [IsOverride]
        public override void Construct()
        {
            BTN_Pause.OnButtonBaseClicked.Add(this, OnPauseBtnClicked);
        }

        [IsOverride]
        public override void Destruct()
        {
            BTN_Pause.OnButtonBaseClicked.RemoveAll(this);
            
            TokenSource?.Cancel();
        }
        
        [IsOverride]
        public virtual void Add_h20_Stack_h20_Item(TSubclassOf<UCommonActivatableWidget> ActivatableWidgetClass)
        {
            MainStack.BP_AddWidget(ActivatableWidgetClass);
        }
        
        [IsOverride]
        public virtual void End_h20_Game(Boolean Win = false)
        {
            var UI_EndGame = MainStack.BP_AddWidget(UI_EndGame_C.StaticClass()) as UI_EndGame_C;

            UI_EndGame.EndGame();
            
            UI_EndGame.ActivateWidget();
        }
        
        [IsOverride]
        public virtual void Pull_h20_Current_h20_Active_h20_Widget()
        {
            MainStack.RemoveWidget(MainStack.GetActiveWidget());
        }
        
        private void Initialize()
        {
            /*
             * Get All Resources in Resource Enum and add widget to UI
             */
            // ResourceContainer.ClearChildren();
            
            if (TokenSource != null)
            {
                TokenSource.Cancel();
            }
            else
            {
                TokenSource = new CancellationTokenSource();
                
                AddResource();
            }
            
            /*
             * Bind to Villager Count
             */
            var BP_GM = UGameplayStatics.GetGameMode(this) as BP_GM_C;
            
            BP_GM.Update_h20_Villagers.Add(this, UpdateVillagerDetails);

            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;
            
            BP_PC.KeySwitch.Add(this, OnKeySwitch);
        }
        
        private async void AddResource()
        {
            while (!TokenSource.IsCancellationRequested)
            {
                Console.WriteLine("Before {0}", Resources);
                /*
                 * Check if end of enum is reached
                 */
                if (Resources > E_ResourceType.Stone)
                {
                    TokenSource?.Cancel();
                }
                else
                {
                    var UIE_Resource = Unreal.CreateWidget<UIE_Resource_C>(UGameplayStatics.GetPlayerController(this, 0));

                    UIE_Resource.SetResourceType(Resources);
                    
                    ResourceContainer.AddChild(UIE_Resource);

                    Resources += 1;
                    
                    Console.WriteLine("After {0}", Resources);
                }
                
                await Task.Delay(100);
            }
        }
        
        private void UpdateVillagerDetails(Int32 VillagerCount)
        {
            VillagerCounter.SetText(UKismetStringLibrary.Conv_IntToString(VillagerCount).ToString());
        }
        
        private void OnKeySwitch(E_InputType NewType)
        {
            BTN_Pause.SetRenderOpacity(NewType == E_InputType.Gamepad ? 0.0f : 1.0f);
        }

        private void OnPauseBtnClicked(UCommonButtonBase Button)
        {
            MainStack.BP_AddWidget(UI_Pause_C.StaticClass());
        }
        
        private CancellationTokenSource TokenSource;
    }
}