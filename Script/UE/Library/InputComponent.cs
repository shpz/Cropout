using System;
using Script.Common;
using Script.CoreUObject;
using Script.InputCore;
using Script.Library;
using Script.Slate;

namespace Script.Engine
{
    public partial class UInputComponent
    {
        public void BindAction(FName InActionName, EInputEvent InInputEvent, Action<FKey> InAction)
        {
            InputComponentImplementation
                .InputComponent_GetDynamicBindingObjectImplementation<UInputActionDelegateBinding>(
                    GetOwner().GetClass().GetHandle(),
                    UInputActionDelegateBinding.StaticClass().GetHandle(),
                    out var InputActionDelegateBinding);

            if (InputActionDelegateBinding != null)
            {
                foreach (var InputActionDelegate in InputActionDelegateBinding.InputActionDelegateBindings)
                {
                    if (InputActionDelegate.FunctionNameToBind.ToString() == InAction.Method.Name)
                    {
                        return;
                    }
                }

                var Binding = new FBlueprintInputActionDelegateBinding
                {
                    InputActionName = InActionName,
                    InputKeyEvent = InInputEvent,
                    FunctionNameToBind = InAction.Method.Name
                };

                InputActionDelegateBinding.InputActionDelegateBindings.Add(Binding);

                InputComponentImplementation.InputComponent_BindActionImplementation(
                    GetHandle(),
                    InputActionDelegateBinding.GetHandle(),
                    GetOwner().GetHandle(),
                    Binding.FunctionNameToBind);
            }
        }

        public void BindAxis(FName InAxisName, Action<Single> InAction)
        {
            InputComponentImplementation
                .InputComponent_GetDynamicBindingObjectImplementation<UInputAxisDelegateBinding>(
                    GetOwner().GetClass().GetHandle(),
                    UInputAxisDelegateBinding.StaticClass().GetHandle(),
                    out var InputAxisDelegateBinding);

            if (InputAxisDelegateBinding != null)
            {
                foreach (var InputAxisDelegate in InputAxisDelegateBinding.InputAxisDelegateBindings)
                {
                    if (InputAxisDelegate.FunctionNameToBind.ToString() == InAction.Method.Name)
                    {
                        return;
                    }
                }

                var Binding = new FBlueprintInputAxisDelegateBinding
                {
                    InputAxisName = InAxisName,
                    FunctionNameToBind = InAction.Method.Name
                };

                InputAxisDelegateBinding.InputAxisDelegateBindings.Add(Binding);

                InputComponentImplementation.InputComponent_BindAxisImplementation(
                    GetHandle(),
                    InputAxisDelegateBinding.GetHandle(),
                    GetOwner().GetHandle(),
                    Binding.FunctionNameToBind);
            }
        }

        public void BindAxisKey(FKey InKey, Action<Single> InAction)
        {
            InputComponentImplementation
                .InputComponent_GetDynamicBindingObjectImplementation<UInputAxisKeyDelegateBinding>(
                    GetOwner().GetClass().GetHandle(),
                    UInputAxisKeyDelegateBinding.StaticClass().GetHandle(),
                    out var InputAxisKeyDelegateBinding);

            if (InputAxisKeyDelegateBinding != null)
            {
                foreach (var InputAxisKeyDelegate in InputAxisKeyDelegateBinding.InputAxisKeyDelegateBindings)
                {
                    if (InputAxisKeyDelegate.FunctionNameToBind.ToString() == InAction.Method.Name)
                    {
                        return;
                    }
                }

                var Binding = new FBlueprintInputAxisKeyDelegateBinding
                {
                    AxisKey = InKey,
                    FunctionNameToBind = InAction.Method.Name
                };

                InputAxisKeyDelegateBinding.InputAxisKeyDelegateBindings.Add(Binding);

                InputComponentImplementation.InputComponent_BindAxisKeyImplementation(
                    GetHandle(),
                    InputAxisKeyDelegateBinding.GetHandle(),
                    GetOwner().GetHandle(),
                    Binding.FunctionNameToBind);
            }
        }

        public void BindKey(FInputChord InInputChord, EInputEvent InInputEvent, Action InAction)
        {
            InputComponentImplementation.InputComponent_GetDynamicBindingObjectImplementation<UInputKeyDelegateBinding>(
                GetOwner().GetClass().GetHandle(),
                UInputKeyDelegateBinding.StaticClass().GetHandle(),
                out var InputKeyDelegateBinding);

            if (InputKeyDelegateBinding != null)
            {
                foreach (var InputKeyDelegate in InputKeyDelegateBinding.InputKeyDelegateBindings)
                {
                    if (InputKeyDelegate.FunctionNameToBind.ToString() == InAction.Method.Name)
                    {
                        return;
                    }
                }

                var Binding = new FBlueprintInputKeyDelegateBinding
                {
                    InputChord = InInputChord,
                    InputKeyEvent = InInputEvent,
                    FunctionNameToBind = InAction.Method.Name
                };

                InputKeyDelegateBinding.InputKeyDelegateBindings.Add(Binding);

                InputComponentImplementation.InputComponent_BindKeyImplementation(
                    GetHandle(),
                    InputKeyDelegateBinding.GetHandle(),
                    GetOwner().GetHandle(),
                    Binding.FunctionNameToBind);
            }
        }

        public void BindKey(FKey InKey, EInputEvent InInputEvent, Action InAction)
        {
            BindKey(new FInputChord
                {
                    Key = InKey,
                    bShift = false,
                    bCtrl = false,
                    bAlt = false,
                    bCmd = false
                },
                InInputEvent,
                InAction);
        }

        public void BindTouch(EInputEvent InInputEvent, Action<ETouchIndex, FVector> InAction)
        {
            InputComponentImplementation
                .InputComponent_GetDynamicBindingObjectImplementation<UInputTouchDelegateBinding>(
                    GetOwner().GetClass().GetHandle(),
                    UInputTouchDelegateBinding.StaticClass().GetHandle(),
                    out var InputTouchDelegateBinding);

            if (InputTouchDelegateBinding != null)
            {
                foreach (var InputTouchDelegate in InputTouchDelegateBinding.InputTouchDelegateBindings)
                {
                    if (InputTouchDelegate.FunctionNameToBind.ToString() == InAction.Method.Name)
                    {
                        return;
                    }
                }

                var Binding = new FBlueprintInputTouchDelegateBinding
                {
                    InputKeyEvent = InInputEvent,
                    FunctionNameToBind = InAction.Method.Name
                };

                InputTouchDelegateBinding.InputTouchDelegateBindings.Add(Binding);

                InputComponentImplementation.InputComponent_BindTouchImplementation(
                    GetHandle(),
                    InputTouchDelegateBinding.GetHandle(),
                    GetOwner().GetHandle(),
                    Binding.FunctionNameToBind);
            }
        }

        public void BindVectorAxis(FKey InKey, Action<FVector> InAction)
        {
            InputComponentImplementation
                .InputComponent_GetDynamicBindingObjectImplementation<UInputVectorAxisDelegateBinding>(
                    GetOwner().GetClass().GetHandle(),
                    UInputVectorAxisDelegateBinding.StaticClass().GetHandle(),
                    out var InputVectorAxisDelegateBinding);

            if (InputVectorAxisDelegateBinding != null)
            {
                foreach (var InputAxisKeyDelegate in InputVectorAxisDelegateBinding.InputAxisKeyDelegateBindings)
                {
                    if (InputAxisKeyDelegate.FunctionNameToBind.ToString() == InAction.Method.Name)
                    {
                        return;
                    }
                }

                var Binding = new FBlueprintInputAxisKeyDelegateBinding
                {
                    AxisKey = InKey,
                    FunctionNameToBind = InAction.Method.Name
                };

                InputVectorAxisDelegateBinding.InputAxisKeyDelegateBindings.Add(Binding);

                InputComponentImplementation.InputComponent_BindVectorAxisImplementation(
                    GetHandle(),
                    InputVectorAxisDelegateBinding.GetHandle(),
                    GetOwner().GetHandle(),
                    Binding.FunctionNameToBind);
            }
        }
    }
}