using System;
using System.Runtime.CompilerServices;
using Skyline_Sports_Project;
using Unity.Burst;
using UnityEngine.Scripting;

namespace Unity.Entities.CodeGeneratedRegistry;

[Preserve]
public class AssemblyTypeRegistry
{
	public static readonly TypeRegistry TypeRegistry;

	unsafe static AssemblyTypeRegistry()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected Obj, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected Obj, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected Obj, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Expected Obj, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected Obj, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected Obj, but got Unknown
		TypeRegistry val = new TypeRegistry();
		val.AssemblyName = "Skyline_Sports_Project";
		val.TypeInfosPtr = null;
		val.TypeInfosCount = 0;
		val.Types = new Type[0];
		val.TypeNames = new string[0];
		val.EntityOffsetsPtr = null;
		val.EntityOffsetsCount = 0;
		val.BlobAssetReferenceOffsetsPtr = null;
		val.BlobAssetReferenceOffsetsCount = 0;
		val.UnityObjectReferenceOffsetsPtr = null;
		val.UnityObjectReferenceOffsetsCount = 0;
		val.WeakAssetReferenceOffsetsPtr = null;
		val.WeakAssetReferenceOffsetsCount = 0;
		val.WriteGroups = new Type[0];
		val.SystemTypes = new Type[3]
		{
			typeof(SportsDataSystem),
			typeof(SportsUISystem),
			typeof(SportsVenueSection)
		};
		val.SystemFilterFlags = (WorldSystemFilterFlags[])(object)new int[3] { 1792, 1792, 1792 };
		val.SystemTypeNames = new string[3] { "Skyline_Sports_Project.SportsDataSystem", "Skyline_Sports_Project.SportsUISystem", "Skyline_Sports_Project.SportsVenueSection" };
		val.SystemTypeSizes = new int[3] { 8, 8, 8 };
		val.SystemTypeHashes = new long[3]
		{
			BurstRuntime.GetHashCode64<SportsDataSystem>(),
			BurstRuntime.GetHashCode64<SportsUISystem>(),
			BurstRuntime.GetHashCode64<SportsVenueSection>()
		};
		val.SystemTypeFlags = new int[3] { 536870912, 536870912, 536870912 };
		val.BoxedEquals = Equals;
		val.BoxedEqualsPtr = Equals;
		val.BoxedGetHashCode = BoxedGetHashCode;
		val.ConstructComponentFromBuffer = ConstructComponentFromBuffer;
		val.GetSystemAttributes = GetSystemAttributes;
		val.SetSharedTypeIndices = SetSharedStaticTypeIndices;
		TypeRegistry = val;
	}

	public static bool Equals(object lhs, object rhs, int typeIndex)
	{
		switch (typeIndex)
		{
		default:
			;
			break;
		}
		throw new NotSupportedException("FATAL: Tried to call TypeManager.Equals() for a component type unknown to the static TypeRegistry");
	}

	public unsafe static bool Equals(object lhs, void* rhs, int typeIndex)
	{
		switch (typeIndex)
		{
		default:
			;
			break;
		}
		throw new NotSupportedException("FATAL: Tried to call TypeManager.Equals() for a component type unknown to the static TypeRegistry");
	}

	public static int BoxedGetHashCode(object val, int typeIndex)
	{
		switch (typeIndex)
		{
		default:
			;
			break;
		}
		throw new NotSupportedException("FATAL: Tried to call TypeManager.GetHashCode() for a component type unknown to the static TypeRegistry");
	}

	public unsafe static object ConstructComponentFromBuffer(void* buffer, int typeIndexNoFlags)
	{
		switch (typeIndexNoFlags)
		{
		default:
			;
			break;
		}
		throw new NotSupportedException("FATAL: Tried to construct a type that is unknown to the StaticTypeRegistry");
	}

	public static Attribute[] GetSystemAttributes(Type systemType)
	{
		if ((object)systemType == typeof(SportsDataSystem))
		{
			return new Attribute[1]
			{
				new CompilerGeneratedAttribute()
			};
		}
		if ((object)systemType == typeof(SportsUISystem))
		{
			return new Attribute[1]
			{
				new CompilerGeneratedAttribute()
			};
		}
		if ((object)systemType == typeof(SportsVenueSection))
		{
			return new Attribute[0];
		}
		throw new ArgumentException("FATAL: GetSystemAttributes asked to create an unknown Type.");
	}

	public unsafe static void SetSharedStaticTypeIndices(int* pTypeInfos, int count)
	{
		if (count != 0)
		{
			throw new ArgumentException("The passed in 'count' does not match the expected count of '0' component types");
		}
	}
}
