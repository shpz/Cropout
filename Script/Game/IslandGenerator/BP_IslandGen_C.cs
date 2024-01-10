using System;
using Script.Common;
using Script.CoreUObject;
using Script.Cropout;
using Script.Engine;
using Script.GeometryScriptingCore;
using Script.IslandGenerator.Misc;
using Script.Library;

namespace Script.IslandGenerator
{
    [IsOverride]
    public partial class BP_IslandGen_C
    {
        [IsOverride]
        public virtual void ReceiveBeginPlay()
        {
            Create_h20_Island(false);
        }

        /**
         * 创建岛屿
         */
        [IsOverride]
        public virtual void Create_h20_Island(bool bSpawnMarkers)
        {
            DynMesh = DynamicMeshComponent.GetDynamicMesh();
            DynMesh.Reset();

            Generate();
            Solidify();
            SetNormal();
            Smoothing();
            PNTessellation();
            CutUnderside();
            FlattenTop();
            ProjectUV();
            SetIslandColor();

            if (bSpawnMarkers)
            {
                SpawnMarkers();
            }

            ReleaseAllComputeMeshes();
            GenNavmesh();
        }

        /**
         * 不同平台之间的参数切换
         */
        [IsOverride]
        public virtual int Platform_h20_Switch(int Low, int High)
        {
            int Result = High;
            FString PlatformName = UGameplayStatics.GetPlatformName();
            if (PlatformName == "Android"
                || PlatformName == "IOS"
                || PlatformName == "Switch")
            {
                Result = Low;
            }

            return Result;
        }

        /**
         * 生成岛屿基础图形
         */
        protected void Generate()
        {
            for (var i = 0; i < Islands; ++i)
            {
                double BaseRadius = Seed.FRandRange(Islands_h20_Size.X, Islands_h20_Size.Y);
                double TopRadius = BaseRadius / 4;
                double Distance = Max_h20_Spawn_h20_Distance * 0.5f;

                FVector UnitVector = Seed.VRand();
                FVector Location = new FVector(UnitVector.X * Distance, UnitVector.Y * Distance, -800);
                FRotator Rotator = new FRotator(0, 0, 0);
                FVector Scale = new FVector(1, 1, 1);
                FTransform Transform = UKismetMathLibrary.MakeTransform(Location, Rotator, Scale);

                FGeometryScriptPrimitiveOptions Options = new FGeometryScriptPrimitiveOptions();
                UGeometryScriptLibrary_MeshPrimitiveFunctions.AppendCone(
                    DynMesh,
                    Options,
                    Transform,
                    (float)BaseRadius,
                    (float)TopRadius,
                    1300f,
                    32,
                    1
                );

                SpawnPoints.Add(Location);
            }
        }

        /**
         * 体素化并且网格化。等同于 VoxelWrap
         */
        protected void Solidify()
        {
            // 往锥体底下加一个大盒体，后边体素化用
            FGeometryScriptPrimitiveOptions PrimitiveOptions = new FGeometryScriptPrimitiveOptions();
            FTransform Transform = UKismetMathLibrary.MakeTransform(
                new FVector(0.0f, 0.0f, -800.0f),
                new FRotator());
            UGeometryScriptLibrary_MeshPrimitiveFunctions.AppendBox(
                DynMesh,
                PrimitiveOptions,
                Transform,
                Max_h20_Spawn_h20_Distance + 10000f,
                Max_h20_Spawn_h20_Distance + 10000f,
                400.0f
            );

            // VoxelWrap
            FGeometryScriptSolidifyOptions SolidifyOptions = new FGeometryScriptSolidifyOptions();
            SolidifyOptions.GridParameters = new FGeometryScript3DGridParameters();
            SolidifyOptions.GridParameters.SizeMethod = EGeometryScriptGridSizingMethod.GridResolution;
            SolidifyOptions.GridParameters.GridResolution = Platform_h20_Switch(50, 60);

            SolidifyOptions.WindingThreshold = 0.5f;
            SolidifyOptions.bSolidAtBoundaries = false;
            SolidifyOptions.ExtendBounds = 0.0f;
            SolidifyOptions.SurfaceSearchSteps = 64;
            SolidifyOptions.bThickenShells = false;
            SolidifyOptions.ShellThickness = 1.0f;
            UGeometryScriptLibrary_MeshVoxelFunctions.ApplyMeshSolidify(
                DynMesh,
                SolidifyOptions
            );
        }

        /**
         * 逐顶点重新计算法线。
         */
        protected void SetNormal()
        {
            UGeometryScriptLibrary_MeshNormalsFunctions.SetPerVertexNormals(DynMesh);
        }

        /**
         * 平滑网格体
         */
        protected void Smoothing()
        {
            FGeometryScriptMeshSelection Selection = new FGeometryScriptMeshSelection();

            FGeometryScriptIterativeMeshSmoothingOptions Options = new FGeometryScriptIterativeMeshSmoothingOptions();
            Options.NumIterations = 6;
            Options.Alpha = 0.2f;
            Options.EmptyBehavior = EGeometryScriptEmptySelectionBehavior.FullMeshSelection;

            UGeometryScriptLibrary_MeshDeformFunctions.ApplyIterativeSmoothingToMesh(
                DynMesh,
                Selection,
                Options
            );
        }

        /**
         * 曲面细分
         */
        protected void PNTessellation()
        {
            FGeometryScriptPNTessellateOptions Options = new FGeometryScriptPNTessellateOptions();
            UGeometryScriptLibrary_MeshSubdivideFunctions.ApplyPNTessellation(
                DynMesh,
                Options,
                Platform_h20_Switch(0, 2)
            );
        }

        /**
         * 裁剪底部
         */
        protected void CutUnderside()
        {
            FVector Location = new FVector(0f, 0f, -390f);
            FRotator Rotator = new FRotator(180f, 0f, 0f);
            FTransform Transform = UKismetMathLibrary.MakeTransform(Location, Rotator);

            FGeometryScriptMeshPlaneCutOptions Options = new FGeometryScriptMeshPlaneCutOptions();
            Options.bFillHoles = false;
            Options.bFillSpans = false;
            Options.bFlipCutSide = false;
            Options.UVWorldDimension = 1f;

            UGeometryScriptLibrary_MeshBooleanFunctions.ApplyMeshPlaneCut(
                DynMesh,
                Transform,
                Options
            );
        }

        /**
         * 压平顶部
         */
        protected void FlattenTop()
        {
            FVector Location = new FVector(0f, 0f, 0f);
            FRotator Rotator = new FRotator(0f, 0f, 0f);
            FTransform Transform = UKismetMathLibrary.MakeTransform(Location, Rotator);

            FGeometryScriptMeshPlaneCutOptions Options = new FGeometryScriptMeshPlaneCutOptions();

            UGeometryScriptLibrary_MeshBooleanFunctions.ApplyMeshPlaneCut(
                DynMesh,
                Transform,
                Options
            );
        }

        /**
         * 用平面投影到图形上，重新设置 uv
         */
        protected void ProjectUV()
        {
            FVector Location = new FVector(0f, 0f, 0f);
            FRotator Rotator = new FRotator(0f, 0f, 0f);
            FVector Scale = new FVector(0f, 0f, 0f);
            FTransform Transform = UKismetMathLibrary.MakeTransform(Location, Rotator, Scale);

            FGeometryScriptMeshSelection Selection = new FGeometryScriptMeshSelection();

            UGeometryScriptLibrary_MeshUVFunctions.SetMeshUVsFromPlanarProjection(
                DynMesh,
                0,
                Transform,
                Selection
            );
        }

        /**
         * 在绿色到蓝色之间随机岛屿颜色
         */
        protected void SetIslandColor()
        {
            MPC_Landscape Collection = Unreal.LoadObject<MPC_Landscape>(this);

            FLinearColor GrassColour = UKismetMaterialLibrary.GetVectorParameterValue(
                GetWorld(),
                Collection,
                "GrassColour"
            );

            float H = 0;
            float S = 0;
            float V = 0;
            float A = 0;
            UKismetMathLibrary.RGBToHSV(GrassColour, ref H, ref S, ref V, ref S);

            FLinearColor NewColor = UKismetMathLibrary.HSVToRGB(
                UKismetMathLibrary.RandomFloatInRangeFromStream(Seed, 0, 90),
                S,
                V,
                A);
            UKismetMaterialLibrary.SetVectorParameterValue(
                null,
                Collection,
                "GrassColour",
                NewColor);
        }

        /**
         * 生成占位标记，用于后期生成岛屿上物体定位
         */
        protected void SpawnMarkers()
        {
            foreach (var Loc in SpawnPoints)
            {
                GetWorld().SpawnActor<BP_SpawnMarker_C>(UKismetMathLibrary.MakeTransform(Loc, new FRotator()));
            }
        }
        
        protected void GenNavmesh()
        {
            var Offset = new FVector(0, 0, 0.05);
            var HitResult = new FHitResult();
            K2_AddActorWorldOffset(Offset, false, ref HitResult, false);
        }
    }
}