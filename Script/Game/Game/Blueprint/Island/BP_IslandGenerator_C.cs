using System;
using System.Collections.Generic;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.GeometryFramework;
using Script.GeometryScriptingCore;

namespace Script.Game.Blueprint.Island
{
    [IsOverride]
    public partial class BP_IslandGenerator_C
    {
        [IsOverride]
        public virtual void ReceiveBeginPlay()
        {
            Console.WriteLine(">>>>>Debug ReceiveBeginPlay");
            TargetMesh = DynamicMeshComponent.GetDynamicMesh();
            CreateIsland(false);
        }

        private List<FVector> SpawnPoints = new List<FVector>();
        private UDynamicMesh TargetMesh;

        [IsOverride]
        public virtual void CreateIsland(bool bSpawnMarkers)
        {
            TargetMesh.Reset();

            Generate();
            Solidify();
            SetNormal();
            Smoothing();
            PNTessellation();
            FlattenTop();
            ProjectUV();

            if (bSpawnMarkers)
            {
                SpawnMarkers();
            }
            
            ReleaseAllComputeMeshes();
        }

        protected void Generate()
        {
            for (var i = 0; i < IslandsNumber; ++i)
            {
                float BaseRadius = UKismetMathLibrary.RandomFloatInRangeFromStream(
                    RandomStream,
                    (float)IslandsSize.X,
                    (float)IslandsSize.Y
                );

                float TopRadius = BaseRadius / 4f;
                double Distance = MaxSpawnDistance * 0.5f;

                FVector UnitVector = RandomStream.VRand();
                FVector Location = new FVector(UnitVector.X * Distance, UnitVector.Y * Distance, -800);
                FRotator Rotator = new FRotator(0, 0, 0);
                FVector Scale = new FVector(1, 1, 1);
                FTransform Transform = UKismetMathLibrary.MakeTransform(Location, Rotator, Scale);

                FGeometryScriptPrimitiveOptions Options = new FGeometryScriptPrimitiveOptions();
                UGeometryScriptLibrary_MeshPrimitiveFunctions.AppendCone(
                    TargetMesh,
                    Options,
                    Transform,
                    BaseRadius,
                    TopRadius,
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
                TargetMesh,
                PrimitiveOptions,
                Transform,
                (float)MaxSpawnDistance + 10000f,
                (float)MaxSpawnDistance + 10000f,
                400.0f
            );

            // VoxelWrap
            FGeometryScriptSolidifyOptions SolidifyOptions = new FGeometryScriptSolidifyOptions();
            SolidifyOptions.GridParameters = new FGeometryScript3DGridParameters();
            SolidifyOptions.GridParameters.SizeMethod = EGeometryScriptGridSizingMethod.GridResolution;
            SolidifyOptions.GridParameters.GridResolution = PlatformSwitch(50, 60);

            SolidifyOptions.WindingThreshold = 0.5f;
            SolidifyOptions.bSolidAtBoundaries = false;
            SolidifyOptions.ExtendBounds = 0.0f;
            SolidifyOptions.SurfaceSearchSteps = 64;
            SolidifyOptions.bThickenShells = false;
            SolidifyOptions.ShellThickness = 1.0f;
            UGeometryScriptLibrary_MeshVoxelFunctions.ApplyMeshSolidify(
                TargetMesh,
                SolidifyOptions
            );
        }

        protected void SetNormal()
        {
            UGeometryScriptLibrary_MeshNormalsFunctions.SetPerVertexNormals(TargetMesh);
        }

        protected void Smoothing()
        {
            FGeometryScriptMeshSelection Selection = new FGeometryScriptMeshSelection();

            FGeometryScriptIterativeMeshSmoothingOptions Options = new FGeometryScriptIterativeMeshSmoothingOptions();
            Options.NumIterations = 6;
            Options.Alpha = 0.2f;
            Options.EmptyBehavior = EGeometryScriptEmptySelectionBehavior.FullMeshSelection;

            UGeometryScriptLibrary_MeshDeformFunctions.ApplyIterativeSmoothingToMesh(
                TargetMesh,
                Selection,
                Options
            );
        }

        protected void PNTessellation()
        {
            FGeometryScriptPNTessellateOptions Options = new FGeometryScriptPNTessellateOptions();
            UGeometryScriptLibrary_MeshSubdivideFunctions.ApplyPNTessellation(
                TargetMesh,
                Options,
                PlatformSwitch(0, 2)
            );
        }

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
                TargetMesh,
                Transform,
                Options
            );
        }

        protected void FlattenTop()
        {
            FVector Location = new FVector(0f, 0f, 0f);
            FRotator Rotator = new FRotator(0f, 0f, 0f);
            FTransform Transform = UKismetMathLibrary.MakeTransform(Location, Rotator);

            FGeometryScriptMeshPlaneCutOptions Options = new FGeometryScriptMeshPlaneCutOptions();

            UGeometryScriptLibrary_MeshBooleanFunctions.ApplyMeshPlaneCut(
                TargetMesh,
                Transform,
                Options
            );
        }

        protected void ProjectUV()
        {
            FVector Location = new FVector(0f, 0f, 0f);
            FRotator Rotator = new FRotator(0f, 0f, 0f);
            FVector Scale = new FVector(0f, 0f, 0f);
            FTransform Transform = UKismetMathLibrary.MakeTransform(Location, Rotator, Scale);

            FGeometryScriptMeshSelection Selection = new FGeometryScriptMeshSelection();

            UGeometryScriptLibrary_MeshUVFunctions.SetMeshUVsFromPlanarProjection(
                TargetMesh,
                0,
                Transform,
                Selection
            );
        }

        protected void SpawnMarkers()
        {
            foreach (var Loc in SpawnPoints)
            {
                GetWorld().SpawnActor<BP_SpawnMark_C>(UKismetMathLibrary.MakeTransform(Loc, new FRotator()));
            }
        }

        protected int PlatformSwitch(int Low, int High)
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
    }
}