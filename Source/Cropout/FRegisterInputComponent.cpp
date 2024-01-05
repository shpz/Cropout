#include "Binding/Class/TReflectionClassBuilder.inl"
#include "Environment/FCSharpEnvironment.h"
#include "Macro/BindingMacro.h"
#include "Macro/NamespaceMacro.h"
#include "Engine/DynamicBlueprintBinding.h"
#include "Engine/InputActionDelegateBinding.h"
#include "Engine/InputAxisDelegateBinding.h"
#include "Engine/InputAxisKeyDelegateBinding.h"
#include "Engine/InputKeyDelegateBinding.h"
#include "Engine/InputTouchDelegateBinding.h"
#include "Engine/InputVectorAxisDelegateBinding.h"

BINDING_REFLECTION_CLASS(UInputComponent);

struct FRegisterInputComponent
{
	static void GetDynamicBindingObjectImplementation(const FGarbageCollectionHandle InThisClass,
	                                                  const FGarbageCollectionHandle InBindingClass,
	                                                  MonoObject** OutValue)
	{
		const auto ThisClass = FCSharpEnvironment::GetEnvironment().GetObject<UBlueprintGeneratedClass>(InThisClass);

		const auto BindingClass = FCSharpEnvironment::GetEnvironment().GetObject<UClass>(InBindingClass);

		if (ThisClass != nullptr && BindingClass != nullptr)
		{
			UObject* DynamicBindingObject = UBlueprintGeneratedClass::GetDynamicBindingObject(ThisClass, BindingClass);

			if (DynamicBindingObject == nullptr)
			{
				DynamicBindingObject = NewObject<UObject>(GetTransientPackage(), BindingClass);

				ThisClass->DynamicBindingObjects.Add(reinterpret_cast<UDynamicBlueprintBinding*>(DynamicBindingObject));
			}

			*OutValue = FCSharpEnvironment::GetEnvironment().Bind(DynamicBindingObject);
		}
	}

	template <typename T>
	static void BindImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                               const FGarbageCollectionHandle InInputDelegateBinding,
	                               const FGarbageCollectionHandle InObjectToBindTo,
	                               MonoObject* InFunctionNameToBind,
	                               const TFunction<void(UClass*, const FName&)>& InFunction
	)
	{
		if (const auto FoundObject = FCSharpEnvironment::GetEnvironment().GetObject<UInputComponent>(
			InGarbageCollectionHandle))
		{
			const auto InputDelegateBinding = FCSharpEnvironment::GetEnvironment().GetObject<T>(InInputDelegateBinding);

			const auto ObjectToBindTo = FCSharpEnvironment::GetEnvironment().GetObject<UObject>(InObjectToBindTo);

			InputDelegateBinding->BindToInputComponent(FoundObject, ObjectToBindTo);

			const auto FunctionNameToBind = FName(UTF8_TO_TCHAR(
				FCSharpEnvironment::GetEnvironment().GetDomain()->String_To_UTF8(FCSharpEnvironment::GetEnvironment().
					GetDomain()->Object_To_String(InFunctionNameToBind, nullptr))));

			InFunction(ObjectToBindTo->GetClass(), FunctionNameToBind);
		}
	}

	static void BindActionImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                     const FGarbageCollectionHandle InInputActionDelegateBinding,
	                                     const FGarbageCollectionHandle InObjectToBindTo,
	                                     MonoObject* InFunctionNameToBind)
	{
		BindImplementation<UInputActionDelegateBinding>(
			InGarbageCollectionHandle,
			InInputActionDelegateBinding,
			InObjectToBindTo,
			InFunctionNameToBind,
			[](UClass* InClass, const FName& InFunctionName)
			{
				BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
				{
					const auto Property = new FStructProperty(InFunction, TEXT("Key"), RF_Public | RF_Transient);

					Property->ElementSize = FKey::StaticStruct()->GetStructureSize();

					Property->Struct = FKey::StaticStruct();

					Property->SetPropertyFlags(CPF_Parm);

					InFunction->AddCppProperty(Property);
				});
			}
		);
	}

	static void BindAxisImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                   const FGarbageCollectionHandle InInputAxisDelegateBinding,
	                                   const FGarbageCollectionHandle InObjectToBindTo,
	                                   MonoObject* InFunctionNameToBind)
	{
		BindImplementation<UInputAxisDelegateBinding>(
			InGarbageCollectionHandle,
			InInputAxisDelegateBinding,
			InObjectToBindTo,
			InFunctionNameToBind,
			[](UClass* InClass, const FName& InFunctionName)
			{
				BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
				{
					const auto Property = new FFloatProperty(InFunction, TEXT("AxisValue"), RF_Public | RF_Transient);

					Property->SetPropertyFlags(CPF_Parm);

					InFunction->AddCppProperty(Property);
				});
			});
	}

	static void BindAxisKeyImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                      const FGarbageCollectionHandle InInputAxisKeyDelegateBinding,
	                                      const FGarbageCollectionHandle InObjectToBindTo,
	                                      MonoObject* InFunctionNameToBind)
	{
		BindImplementation<UInputAxisKeyDelegateBinding>(
			InGarbageCollectionHandle,
			InInputAxisKeyDelegateBinding,
			InObjectToBindTo,
			InFunctionNameToBind,
			[](UClass* InClass, const FName& InFunctionName)
			{
				BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
				{
					const auto Property = new FFloatProperty(InFunction, TEXT("AxisValue"), RF_Public | RF_Transient);

					Property->SetPropertyFlags(CPF_Parm);

					InFunction->AddCppProperty(Property);
				});
			});
	}

	static void BindKeyImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                  const FGarbageCollectionHandle InInputKeyDelegateBinding,
	                                  const FGarbageCollectionHandle InObjectToBindTo, MonoObject* InFunctionNameToBind)
	{
		BindImplementation<UInputKeyDelegateBinding>(
			InGarbageCollectionHandle,
			InInputKeyDelegateBinding,
			InObjectToBindTo,
			InFunctionNameToBind,
			[](UClass* InClass, const FName& InFunctionName)
			{
				BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
				{
					const auto Property = new FStructProperty(InFunction, TEXT("Key"), RF_Public | RF_Transient);

					Property->ElementSize = FKey::StaticStruct()->GetStructureSize();

					Property->Struct = FKey::StaticStruct();

					Property->SetPropertyFlags(CPF_Parm);

					InFunction->AddCppProperty(Property);
				});
			});
	}

	static void BindTouchImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                    const FGarbageCollectionHandle InInputTouchDelegateBinding,
	                                    const FGarbageCollectionHandle InObjectToBindTo,
	                                    MonoObject* InFunctionNameToBind)
	{
		BindImplementation<UInputTouchDelegateBinding>(
			InGarbageCollectionHandle,
			InInputTouchDelegateBinding,
			InObjectToBindTo,
			InFunctionNameToBind,
			[](UClass* InClass, const FName& InFunctionName)
			{
				BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
				{
					const auto LocationProperty = new FStructProperty(InFunction, TEXT("Location"),
					                                                  RF_Public | RF_Transient);

					LocationProperty->ElementSize = TBaseStructure<FVector2D>().Get()->GetStructureSize();

					LocationProperty->Struct = TBaseStructure<FVector2D>().Get();

					LocationProperty->SetPropertyFlags(CPF_Parm);

					InFunction->AddCppProperty(LocationProperty);

					const auto FingerIndexProperty = new FEnumProperty(InFunction, TEXT("FingerIndex"),
					                                                   RF_Public | RF_Transient);

					FingerIndexProperty->ElementSize = sizeof(uint8);

					FingerIndexProperty->SetEnum(LoadObject<UEnum>(nullptr, TEXT("/Script/InputCore.ETouchIndex")));

					FingerIndexProperty->SetPropertyFlags(CPF_Parm);

					InFunction->AddCppProperty(FingerIndexProperty);
				});
			});
	}

	static void BindVectorAxisImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                         const FGarbageCollectionHandle InInputVectorAxisDelegateBinding,
	                                         const FGarbageCollectionHandle InObjectToBindTo,
	                                         MonoObject* InFunctionNameToBind)
	{
		BindImplementation<UInputVectorAxisDelegateBinding>(
			InGarbageCollectionHandle,
			InInputVectorAxisDelegateBinding,
			InObjectToBindTo,
			InFunctionNameToBind,
			[](UClass* InClass, const FName& InFunctionName)
			{
				BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
				{
					const auto Property = new FStructProperty(InFunction, TEXT("AxisValue"), RF_Public | RF_Transient);

					Property->ElementSize = TBaseStructure<FVector2D>().Get()->GetStructureSize();

					Property->Struct = TBaseStructure<FVector2D>().Get();

					Property->SetPropertyFlags(CPF_Parm);

					InFunction->AddCppProperty(Property);
				});
			});
	}

	static void BindFunction(UClass* InClass, const FName& InFunctionName,
	                         const TFunction<void(UFunction* InFunction)>& InProperty)
	{
		if (InClass == nullptr)
		{
			return;
		}

		if (InClass->FindFunctionByName(InFunctionName))
		{
			return;
		}

		auto Function = NewObject<UFunction>(InClass, InFunctionName, EObjectFlags::RF_Transient);

		Function->FunctionFlags = FUNC_BlueprintEvent;

		InProperty(Function);

		Function->Bind();

		Function->StaticLink(true);

		InClass->AddFunctionToFunctionMap(Function, InFunctionName);

		Function->Next = InClass->Children;

		InClass->Children = Function;

		Function->AddToRoot();

		FCSharpEnvironment::GetEnvironment().Bind(Function);
	}

	FRegisterInputComponent()
	{
		TReflectionClassBuilder<UInputComponent>(NAMESPACE_LIBRARY)
			.Function("GetDynamicBindingObject", GetDynamicBindingObjectImplementation)
			.Function("BindAction", BindActionImplementation)
			.Function("BindAxis", BindAxisImplementation)
			.Function("BindAxisKey", BindAxisKeyImplementation)
			.Function("BindKey", BindKeyImplementation)
			.Function("BindTouch", BindTouchImplementation)
			.Function("BindVectorAxis", BindVectorAxisImplementation)
			.Register();
	}
};

static FRegisterInputComponent RegisterInputComponent;
