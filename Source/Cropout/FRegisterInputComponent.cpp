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

	static void BindActionImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                     const FGarbageCollectionHandle InInputActionDelegateBinding,
	                                     const FGarbageCollectionHandle InObjectToBindTo,
	                                     MonoObject* InFunctionNameToBind)
	{
		if (const auto FoundObject = FCSharpEnvironment::GetEnvironment().GetObject<UInputComponent>(
			InGarbageCollectionHandle))
		{
			const auto InputActionDelegateBinding = FCSharpEnvironment::GetEnvironment().GetObject<
				UInputActionDelegateBinding>(InInputActionDelegateBinding);

			const auto ObjectToBindTo = FCSharpEnvironment::GetEnvironment().GetObject<UObject>(InObjectToBindTo);

			InputActionDelegateBinding->BindToInputComponent(FoundObject, ObjectToBindTo);

			const auto FunctionNameToBind = FName(UTF8_TO_TCHAR(
				FCSharpEnvironment::GetEnvironment().GetDomain()->String_To_UTF8(FCSharpEnvironment::GetEnvironment().
					GetDomain()->Object_To_String(InFunctionNameToBind, nullptr))));

			BindActionFunction(ObjectToBindTo->GetClass(), FunctionNameToBind);
		}
	}

	static void BindAxisImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                   const FGarbageCollectionHandle InInputAxisDelegateBinding,
	                                   const FGarbageCollectionHandle InObjectToBindTo,
	                                   MonoObject* InFunctionNameToBind)
	{
		if (const auto FoundObject = FCSharpEnvironment::GetEnvironment().GetObject<UInputComponent>(
			InGarbageCollectionHandle))
		{
			const auto InputAxisDelegateBinding = FCSharpEnvironment::GetEnvironment().GetObject<
				UInputAxisDelegateBinding>(InInputAxisDelegateBinding);

			const auto ObjectToBindTo = FCSharpEnvironment::GetEnvironment().GetObject<UObject>(InObjectToBindTo);

			InputAxisDelegateBinding->BindToInputComponent(FoundObject, ObjectToBindTo);

			const auto FunctionNameToBind = FName(UTF8_TO_TCHAR(
				FCSharpEnvironment::GetEnvironment().GetDomain()->String_To_UTF8(FCSharpEnvironment::GetEnvironment().
					GetDomain()->Object_To_String(InFunctionNameToBind, nullptr))));

			BindAxisFunction(ObjectToBindTo->GetClass(), FunctionNameToBind);
		}
	}

	static void BindAxisKeyImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                      const FGarbageCollectionHandle InInputAxisKeyDelegateBinding,
	                                      const FGarbageCollectionHandle InObjectToBindTo,
	                                      MonoObject* InFunctionNameToBind)
	{
		if (const auto FoundObject = FCSharpEnvironment::GetEnvironment().GetObject<UInputComponent>(
			InGarbageCollectionHandle))
		{
			const auto InputAxisKeyDelegateBinding = FCSharpEnvironment::GetEnvironment().GetObject<
				UInputAxisKeyDelegateBinding>(InInputAxisKeyDelegateBinding);

			const auto ObjectToBindTo = FCSharpEnvironment::GetEnvironment().GetObject<UObject>(InObjectToBindTo);

			InputAxisKeyDelegateBinding->BindToInputComponent(FoundObject, ObjectToBindTo);

			const auto FunctionNameToBind = FName(UTF8_TO_TCHAR(
				FCSharpEnvironment::GetEnvironment().GetDomain()->String_To_UTF8(FCSharpEnvironment::GetEnvironment().
					GetDomain()->Object_To_String(InFunctionNameToBind, nullptr))));

			BindAxisFunction(ObjectToBindTo->GetClass(), FunctionNameToBind);
		}
	}

	static void BindKeyImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                  const FGarbageCollectionHandle InInputKeyDelegateBinding,
	                                  const FGarbageCollectionHandle InObjectToBindTo, MonoObject* InFunctionNameToBind)
	{
		if (const auto FoundObject = FCSharpEnvironment::GetEnvironment().GetObject<UInputComponent>(
			InGarbageCollectionHandle))
		{
			const auto InputKeyDelegateBinding = FCSharpEnvironment::GetEnvironment().GetObject<
				UInputKeyDelegateBinding>(InInputKeyDelegateBinding);

			const auto ObjectToBindTo = FCSharpEnvironment::GetEnvironment().GetObject<UObject>(InObjectToBindTo);

			InputKeyDelegateBinding->BindToInputComponent(FoundObject, ObjectToBindTo);

			const auto FunctionNameToBind = FName(UTF8_TO_TCHAR(
				FCSharpEnvironment::GetEnvironment().GetDomain()->String_To_UTF8(FCSharpEnvironment::GetEnvironment().
					GetDomain()->Object_To_String(InFunctionNameToBind, nullptr))));

			BindKeyFunction(ObjectToBindTo->GetClass(), FunctionNameToBind);
		}
	}

	static void BindTouchImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                    const FGarbageCollectionHandle InInputTouchDelegateBinding,
	                                    const FGarbageCollectionHandle InObjectToBindTo,
	                                    MonoObject* InFunctionNameToBind)
	{
		if (const auto FoundObject = FCSharpEnvironment::GetEnvironment().GetObject<UInputComponent>(
			InGarbageCollectionHandle))
		{
			const auto InputTouchDelegateBinding = FCSharpEnvironment::GetEnvironment().GetObject<
				UInputTouchDelegateBinding>(InInputTouchDelegateBinding);

			const auto ObjectToBindTo = FCSharpEnvironment::GetEnvironment().GetObject<UObject>(InObjectToBindTo);

			InputTouchDelegateBinding->BindToInputComponent(FoundObject, ObjectToBindTo);

			const auto FunctionNameToBind = FName(UTF8_TO_TCHAR(
				FCSharpEnvironment::GetEnvironment().GetDomain()->String_To_UTF8(FCSharpEnvironment::GetEnvironment().
					GetDomain()->Object_To_String(InFunctionNameToBind, nullptr))));

			BindTouchFunction(ObjectToBindTo->GetClass(), FunctionNameToBind);
		}
	}

	static void BindVectorAxisImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                         const FGarbageCollectionHandle InInputVectorAxisDelegateBinding,
	                                         const FGarbageCollectionHandle InObjectToBindTo,
	                                         MonoObject* InFunctionNameToBind)
	{
		if (const auto FoundObject = FCSharpEnvironment::GetEnvironment().GetObject<UInputComponent>(
			InGarbageCollectionHandle))
		{
			const auto InputVectorAxisDelegateBinding = FCSharpEnvironment::GetEnvironment().GetObject<
				UInputVectorAxisDelegateBinding>(InInputVectorAxisDelegateBinding);

			const auto ObjectToBindTo = FCSharpEnvironment::GetEnvironment().GetObject<UObject>(InObjectToBindTo);

			InputVectorAxisDelegateBinding->BindToInputComponent(FoundObject, ObjectToBindTo);

			const auto FunctionNameToBind = FName(UTF8_TO_TCHAR(
				FCSharpEnvironment::GetEnvironment().GetDomain()->String_To_UTF8(FCSharpEnvironment::GetEnvironment().
					GetDomain()->Object_To_String(InFunctionNameToBind, nullptr))));

			BindVectorAxisFunction(ObjectToBindTo->GetClass(), FunctionNameToBind);
		}
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

	static void BindActionFunction(UClass* InClass, const FName& InFunctionName)
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

	static void BindAxisFunction(UClass* InClass, const FName& InFunctionName)
	{
		BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
		{
			const auto Property = new FFloatProperty(InFunction, TEXT("AxisValue"), RF_Public | RF_Transient);

			Property->SetPropertyFlags(CPF_Parm);

			InFunction->AddCppProperty(Property);
		});
	}

	static void BindKeyFunction(UClass* InClass, const FName& InFunctionName)
	{
		BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
		{
		});
	}

	static void BindTouchFunction(UClass* InClass, const FName& InFunctionName)
	{
		BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
		{
			const auto LocationProperty = new FStructProperty(InFunction, TEXT("Location"), RF_Public | RF_Transient);

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
	}

	static void BindVectorAxisFunction(UClass* InClass, const FName& InFunctionName)
	{
		BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
		{
			const auto Property = new FStructProperty(InFunction, TEXT("AxisValue"), RF_Public | RF_Transient);

			Property->ElementSize = TBaseStructure<FVector2D>().Get()->GetStructureSize();

			Property->Struct = TBaseStructure<FVector2D>().Get();

			Property->SetPropertyFlags(CPF_Parm);

			InFunction->AddCppProperty(Property);
		});
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
