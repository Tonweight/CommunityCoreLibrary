using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RimWorld;
using Verse;
using Verse.AI;

namespace CommunityCoreLibrary
{

    public class DetourInjector : SpecialInjector
    {

        private const BindingFlags          UniversalBindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public override bool                Inject()
        {
            // Change CompGlower into CompGlowerToggleable
            FixGlowers();

            // Change Building_NutrientPasteDispenser into Building_AdvancedPasteDispenser
            UpgradeNutrientPasteDispensers();

            // Detour RimWorld.JoyGiver_SocialRelax.TryGiveJobInt
            MethodInfo RimWorld_JoyGiver_SocialRelax_TryGiveJobInt = typeof( JoyGiver_SocialRelax ).GetMethod( "TryGiveJobInt", UniversalBindingFlags );
            MethodInfo CCL_JoyGiver_SocialRelax_TryGiveJobInt = typeof( Detour._JoyGiver_SocialRelax ).GetMethod( "_TryGiveJobInt", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_JoyGiver_SocialRelax_TryGiveJobInt, CCL_JoyGiver_SocialRelax_TryGiveJobInt ) )
                return false;

            // Detour RimWorld.ThingSelectionUtility.SelectableNow
            MethodInfo RimWorld_ThingSelectionUtility_SelectableNow = typeof( ThingSelectionUtility ).GetMethod( "SelectableNow", UniversalBindingFlags );
            MethodInfo CCL_ThingSelectionUtility_SelectableNow = typeof( Detour._ThingSelectionUtility ).GetMethod( "_SelectableNow", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_ThingSelectionUtility_SelectableNow, CCL_ThingSelectionUtility_SelectableNow ) )
                return false;

            // Detour RimWorld.FoodUtility.GetFoodDef
            MethodInfo RimWorld_FoodUtility_GetFoodDef = typeof( FoodUtility ).GetMethod( "GetFinalIngestibleDef", UniversalBindingFlags );
            MethodInfo CCL_FoodUtility_GetFoodDef = typeof( Detour._FoodUtility ).GetMethod( "_GetFoodDef", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_FoodUtility_GetFoodDef, CCL_FoodUtility_GetFoodDef ) )
                return false;

            // Detour RimWorld.FoodUtility.FoodSourceOptimality
            MethodInfo RimWorld_FoodUtility_FoodSourceOptimality = typeof( FoodUtility ).GetMethod( "FoodSourceOptimality", UniversalBindingFlags );
            MethodInfo CCL_FoodUtility_FoodSourceOptimality = typeof( Detour._FoodUtility ).GetMethod( "_FoodSourceOptimality", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_FoodUtility_FoodSourceOptimality, CCL_FoodUtility_FoodSourceOptimality ) )
                return false;

            // Detour RimWorld.FoodUtility.ThoughtsFromIngesting
            MethodInfo RimWorld_FoodUtility_ThoughtsFromIngesting = typeof( FoodUtility ).GetMethod( "ThoughtsFromIngesting", UniversalBindingFlags );
            MethodInfo CCL_FoodUtility_ThoughtsFromIngesting = typeof( Detour._FoodUtility ).GetMethod( "_ThoughtsFromIngesting", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_FoodUtility_ThoughtsFromIngesting, CCL_FoodUtility_ThoughtsFromIngesting ) )
                return false;

            // Detour RimWorld.FoodUtility.BestFoodSourceOnMap
            MethodInfo RimWorld_FoodUtility_BestFoodSourceOnMap = typeof( FoodUtility ).GetMethod( "BestFoodSourceOnMap", UniversalBindingFlags );
            MethodInfo CCL_FoodUtility_BestFoodSourceOnMap = typeof( Detour._FoodUtility ).GetMethod( "_BestFoodSourceOnMap", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_FoodUtility_BestFoodSourceOnMap, CCL_FoodUtility_BestFoodSourceOnMap ) )
                return false;

            // Detour RimWorld.JobDriver_FoodDeliver.MakeNewToils
            MethodInfo RimWorld_JobDriver_FoodDeliver_MakeNewToils = typeof( JobDriver_FoodDeliver ).GetMethod( "MakeNewToils", UniversalBindingFlags );
            MethodInfo CCL_JobDriver_FoodDeliver_MakeNewToils = typeof( Detour._JobDriver_FoodDeliver ).GetMethod( "_MakeNewToils", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_JobDriver_FoodDeliver_MakeNewToils, CCL_JobDriver_FoodDeliver_MakeNewToils ) )
                return false;

            // Detour RimWorld.JobDriver_FoodFeedPatient.MakeNewToils
            MethodInfo RimWorld_JobDriver_FoodFeedPatient_MakeNewToils = typeof( JobDriver_FoodFeedPatient ).GetMethod( "MakeNewToils", UniversalBindingFlags );
            MethodInfo CCL_JobDriver_FoodFeedPatient_MakeNewToils = typeof( Detour._JobDriver_FoodFeedPatient ).GetMethod( "_MakeNewToils", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_JobDriver_FoodFeedPatient_MakeNewToils, CCL_JobDriver_FoodFeedPatient_MakeNewToils ) )
                return false;

            // Detour RimWorld.JobDriver_Ingest.UsingNutrientPasteDispenser
            PropertyInfo RimWorld_JobDriver_Ingest_UsingNutrientPasteDispenser = typeof( JobDriver_Ingest ).GetProperty( "UsingNutrientPasteDispenser", UniversalBindingFlags );
            MethodInfo RimWorld_JobDriver_Ingest_UsingNutrientPasteDispenser_get = RimWorld_JobDriver_Ingest_UsingNutrientPasteDispenser.GetGetMethod( true );
            MethodInfo CCL_JobDriver_Ingest_UsingNutrientPasteDispenser = typeof( Detour._JobDriver_Ingest ).GetMethod( "_UsingNutrientPasteDispenser", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_JobDriver_Ingest_UsingNutrientPasteDispenser_get, CCL_JobDriver_Ingest_UsingNutrientPasteDispenser ) )
                return false;

            // Detour RimWorld.JobDriver_Ingest.GetReport
            MethodInfo RimWorld_JobDriver_Ingest_GetReport = typeof( JobDriver_Ingest ).GetMethod( "GetReport", UniversalBindingFlags );
            MethodInfo CCL_JobDriver_Ingest_GetReport = typeof( Detour._JobDriver_Ingest ).GetMethod( "_GetReport", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_JobDriver_Ingest_GetReport, CCL_JobDriver_Ingest_GetReport ) )
                return false;

            // Detour RimWorld.JobDriver_Ingest.PrepareToEatToils_Dispenser
            MethodInfo RimWorld_JobDriver_Ingest_PrepareToEatToils_Dispenser = typeof( JobDriver_Ingest ).GetMethod( "PrepareToIngestToils_Dispenser", UniversalBindingFlags );
            MethodInfo CCL_JobDriver_Ingest_PrepareToEatToils_Dispenser = typeof( Detour._JobDriver_Ingest ).GetMethod( "_PrepareToEatToils_Dispenser", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_JobDriver_Ingest_PrepareToEatToils_Dispenser, CCL_JobDriver_Ingest_PrepareToEatToils_Dispenser ) )
                return false;

            // Detour RimWorld.Toils_Ingest.TakeMealFromDispenser
            MethodInfo RimWorld_Toils_Ingest_TakeMealFromDispenser = typeof( Toils_Ingest ).GetMethod( "TakeMealFromDispenser", UniversalBindingFlags );
            MethodInfo CCL_Toils_Ingest_TakeMealFromDispenser = typeof( Detour._Toils_Ingest ).GetMethod( "_TakeMealFromDispenser", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_Toils_Ingest_TakeMealFromDispenser, CCL_Toils_Ingest_TakeMealFromDispenser ) )
                return false;

            // Detour RimWorld.JobGiver_GetFood.TryGiveJob
            MethodInfo RimWorld_JobGiver_GetFood_TryGiveJob = typeof( JobGiver_GetFood ).GetMethod( "TryGiveJob", UniversalBindingFlags );
            MethodInfo CCL_JobGiver_GetFood_TryGiveJob = typeof( Detour._JobGiver_GetFood ).GetMethod( "_TryGiveJob", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_JobGiver_GetFood_TryGiveJob, CCL_JobGiver_GetFood_TryGiveJob ) )
                return false;

            // Detour RimWorld.JobDriver_SocialRelax.MakeNewToils
            MethodInfo RimWorld_JobDriver_SocialRelax_MakeNewToils = typeof( JobDriver_SocialRelax ).GetMethod( "MakeNewToils", UniversalBindingFlags );
            MethodInfo CCL_JobDriver_SocialRelax_MakeNewToils = typeof( Detour._JobDriver_SocialRelax ).GetMethod( "_MakeNewToils", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_JobDriver_SocialRelax_MakeNewToils, CCL_JobDriver_SocialRelax_MakeNewToils ) )
                return false;

            // Detour Verse.MentalStateWorker_BingingAlcohol.StateCanOccur
            MethodInfo Verse_MentalStateWorker_BingingAlcohol_StateCanOccur = typeof( MentalStateWorker_BingingDrug ).GetMethod( "StateCanOccur", UniversalBindingFlags );
            MethodInfo CCL_MentalStateWorker_BingingAlcohol_StateCanOccur = typeof( Detour._MentalStateWorker_BingingAlcohol ).GetMethod( "_StateCanOccur", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( Verse_MentalStateWorker_BingingAlcohol_StateCanOccur, CCL_MentalStateWorker_BingingAlcohol_StateCanOccur ) )
                return false;

            // Detour RimWorld.CompRottable.CompTickRare
            MethodInfo RimWorld_CompRottable_CompTickRare = typeof( CompRottable ).GetMethod( "CompTickRare", UniversalBindingFlags );
            MethodInfo CCL_CompRottable_CompTickRare = typeof( Detour._CompRottable ).GetMethod( "_CompTickRare", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_CompRottable_CompTickRare, CCL_CompRottable_CompTickRare ) )
                return false;

            // Detour RimWorld.CompRottable.CompInspectStringExtra
            MethodInfo RimWorld_CompRottable_CompInspectStringExtra = typeof( CompRottable ).GetMethod( "CompInspectStringExtra", UniversalBindingFlags );
            MethodInfo CCL_CompRottable_CompInspectStringExtra = typeof( Detour._CompRottable ).GetMethod( "_CompInspectStringExtra", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_CompRottable_CompInspectStringExtra, CCL_CompRottable_CompInspectStringExtra ) )
                return false;

            // Detour Verse.CompHeatPusherPowered.ShouldPushHeatNow
            PropertyInfo Verse_CompHeatPusherPowered_ShouldPushHeatNow = typeof( CompHeatPusherPowered ).GetProperty( "ShouldPushHeatNow", UniversalBindingFlags );
#if DEBUG
            if( Verse_CompHeatPusherPowered_ShouldPushHeatNow == null )
            {
                CCL_Log.Error( "Unable to find 'Verse.CompHeatPusherPowered.ShouldPushHeatNow'" );
                return false;
            }
#endif
            MethodInfo Verse_CompHeatPusherPowered_ShouldPushHeatNow_Getter = Verse_CompHeatPusherPowered_ShouldPushHeatNow.GetGetMethod( true );
            MethodInfo CCL_CompHeatPusherPowered_ShouldPushHeatNow = typeof( Detour._CompHeatPusherPowered ).GetMethod( "_ShouldPushHeatNow", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( Verse_CompHeatPusherPowered_ShouldPushHeatNow_Getter, CCL_CompHeatPusherPowered_ShouldPushHeatNow ) )
                return false;

            // Detour Verse.CompGlower.ShouldBeLitNow
            PropertyInfo Verse_CompGlower_ShouldBeLitNow = typeof( CompGlower ).GetProperty( "ShouldBeLitNow", UniversalBindingFlags );
#if DEBUG
            if( Verse_CompGlower_ShouldBeLitNow == null )
            {
                CCL_Log.Error( "Unable to find 'Verse.CompGlower.ShouldBeLitNow'" );
                return false;
            }
#endif
            MethodInfo Verse_CompGlower_ShouldBeLitNow_Getter = Verse_CompGlower_ShouldBeLitNow.GetGetMethod( true );
            MethodInfo CCL_CompGlower_ShouldBeLitNow = typeof( Detour._CompGlower ).GetMethod( "_ShouldBeLitNow", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( Verse_CompGlower_ShouldBeLitNow_Getter, CCL_CompGlower_ShouldBeLitNow ) )
                return false;

            // Detour RimWorld.MainTabWindow_Research.DrawLeftRect "NotFinished" predicate function
            // Use build number to get the correct predicate function
            var RimWorld_MainTabWindow_Research_DrawLeftRect_NotFinished_Name = string.Empty;
            var RimWorld_Build = RimWorld.VersionControl.CurrentBuild;
            switch( RimWorld_Build )
            {
            case 1220:
            case 1230:
                RimWorld_MainTabWindow_Research_DrawLeftRect_NotFinished_Name = "<DrawLeftRect>m__460";
                break;
            case 1232:
                RimWorld_MainTabWindow_Research_DrawLeftRect_NotFinished_Name = "<DrawLeftRect>m__45E";
                break;
            case 1234:
            case 1238:
            case 1241:
            case 1249:
                RimWorld_MainTabWindow_Research_DrawLeftRect_NotFinished_Name = "<DrawLeftRect>m__45F";
                break;
            default:
                CCL_Log.Trace(
                    Verbosity.Warnings,
                    "CCL needs updating for RimWorld build " + RimWorld_Build.ToString() );
                break;
            }
            if( RimWorld_MainTabWindow_Research_DrawLeftRect_NotFinished_Name != string.Empty )
            {
                MethodInfo RimWorld_MainTabWindow_Research_DrawLeftRect_NotFinished = typeof( RimWorld.MainTabWindow_Research ).GetMethod( RimWorld_MainTabWindow_Research_DrawLeftRect_NotFinished_Name, UniversalBindingFlags );
                MethodInfo CCL_MainTabWindow_Research_DrawLeftRect_NotFinishedNotLockedOut = typeof( Detour._MainTabWindow_Research ).GetMethod( "_NotFinishedNotLockedOut", UniversalBindingFlags );
                if( !Detours.TryDetourFromTo( RimWorld_MainTabWindow_Research_DrawLeftRect_NotFinished, CCL_MainTabWindow_Research_DrawLeftRect_NotFinishedNotLockedOut ) )
                    return false;
            }
            
            // Detour RimWorld.SocialProperness.IsSociallyProper
            MethodInfo RimWorld_SocialProperness_IsSociallyProper = typeof( SocialProperness ).GetMethods().First<MethodInfo>( ( arg ) => (
                ( arg.Name == "IsSociallyProper" ) &&
                ( arg.GetParameters().Count() == 4 )
            ) );
            MethodInfo CCL_SocialProperness_IsSociallyProper = typeof( Detour._SocialProperness ).GetMethod( "_IsSociallyProper", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_SocialProperness_IsSociallyProper, CCL_SocialProperness_IsSociallyProper ) )
                return false;

            // Detour Verse.ThingListGroupHelper.Includes
            MethodInfo Verse_ThingListGroupHelper_Includes = typeof( ThingListGroupHelper ).GetMethod( "Includes", UniversalBindingFlags );
            MethodInfo CCL_ThingListGroupHelper_Includes = typeof( Detour._ThingListGroupHelper ).GetMethod( "_Includes", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( Verse_ThingListGroupHelper_Includes, CCL_ThingListGroupHelper_Includes ) )
                return false;

            // Detour RimWorld.GenConstruct.CanBuildOnTerrain
            MethodInfo RimWorld_GenConstruct_CanBuildOnTerrain = typeof( GenConstruct ).GetMethod( "CanBuildOnTerrain", UniversalBindingFlags );
            MethodInfo CCL_GenConstruct_CanBuildOnTerrain = typeof( Detour._GenConstruct ).GetMethod( "_CanBuildOnTerrain", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_GenConstruct_CanBuildOnTerrain, CCL_GenConstruct_CanBuildOnTerrain ) )
                return false;

            // Detour RimWorld.WorkGiver_Warden_DeliverFood.FoodAvailableInRoomTo
            MethodInfo RimWorld_WorkGiver_Warden_DeliverFood_FoodAvailableInRoomTo = typeof( WorkGiver_Warden_DeliverFood ).GetMethod( "FoodAvailableInRoomTo", UniversalBindingFlags );
            MethodInfo CCL_WorkGiver_Warden_DeliverFood_FoodAvailableInRoomTo = typeof( Detour._WorkGiver_Warden_DeliverFood ).GetMethod( "_FoodAvailableInRoomTo", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_WorkGiver_Warden_DeliverFood_FoodAvailableInRoomTo, CCL_WorkGiver_Warden_DeliverFood_FoodAvailableInRoomTo ) )
                return false;

            // Detour Verse.PreLoadUtility.CheckVersionAndLoad
            MethodInfo Verse_PreLoadUtility_CheckVersionAndLoad = typeof( PreLoadUtility ).GetMethod( "CheckVersionAndLoad", UniversalBindingFlags );
            MethodInfo CCL_PreLoadUtility_CheckVersionAndLoad = typeof( Detour._PreLoadUtility ).GetMethod( "_CheckVersionAndLoad", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( Verse_PreLoadUtility_CheckVersionAndLoad, CCL_PreLoadUtility_CheckVersionAndLoad ) )
                return false;

            // Detour RimWorld.PageUtility.InitGameStart
            MethodInfo RimWorld_PageUtility_InitGameStart = typeof( PageUtility ).GetMethod( "InitGameStart", UniversalBindingFlags );
            MethodInfo CCL_PageUtility_InitGameStart = typeof( Detour._PageUtility ).GetMethod( "_InitGameStart", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_PageUtility_InitGameStart, CCL_PageUtility_InitGameStart ) )
                return false;

            // Detour Verse.ModLister.InstalledModsListHash
            MethodInfo Verse_ModLister_InstalledModsListHash = typeof( ModLister ).GetMethod( "InstalledModsListHash", UniversalBindingFlags );
            MethodInfo CCL_ModLister_InstalledModsListHash = typeof( Detour._ModLister ).GetMethod( "_InstalledModsListHash", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( Verse_ModLister_InstalledModsListHash, CCL_ModLister_InstalledModsListHash ) )
                return false;

            // Detour RimWorld.OutfitDatabase.GenerateStartingOutfits
            MethodInfo RimWorld_OutfitDatabase_GenerateStartingOutfits = typeof( OutfitDatabase ).GetMethod( "GenerateStartingOutfits", UniversalBindingFlags );
            MethodInfo CCL_OutfitDatabase_GenerateStartingOutfits = typeof( Detour._OutfitDatabase ).GetMethod( "_GenerateStartingOutfits", UniversalBindingFlags );
            if (!Detours.TryDetourFromTo( RimWorld_OutfitDatabase_GenerateStartingOutfits, CCL_OutfitDatabase_GenerateStartingOutfits ) )
                return false;

            // Detour RimWorld.Pawn_RelationsTracker.CompatibilityWith
            MethodInfo RimWorld_Pawn_RelationsTracker_CompatibilityWith = typeof( Pawn_RelationsTracker ).GetMethod( "CompatibilityWith", UniversalBindingFlags );
            MethodInfo CCL_Pawn_RelationsTracker_CompatibilityWith = typeof( Detour._Pawn_RelationsTracker ).GetMethod( "_CompatibilityWith", UniversalBindingFlags );
            if (!Detours.TryDetourFromTo( RimWorld_Pawn_RelationsTracker_CompatibilityWith, CCL_Pawn_RelationsTracker_CompatibilityWith ) ) 
                return false;

            // Detour RimWorld.Pawn_RelationsTracker.AttractionTo
            MethodInfo RimWorld_Pawn_RelationsTracker_AttractionTo = typeof( Pawn_RelationsTracker ).GetMethod( "AttractionTo", UniversalBindingFlags );
            MethodInfo CCL_Pawn_RelationsTracker_AttractionTo = typeof( Detour._Pawn_RelationsTracker ).GetMethod( "_AttractionTo", UniversalBindingFlags );
            if (!Detours.TryDetourFromTo( RimWorld_Pawn_RelationsTracker_AttractionTo, CCL_Pawn_RelationsTracker_AttractionTo ) ) 
                return false;
            
            // Detour RimWorld.PlaySettings.ExposeData
            MethodInfo RimWorld_PlaySetting_ExposeData = typeof( PlaySettings ).GetMethod( "ExposeData", UniversalBindingFlags );
            MethodInfo CCL_PlaySetting_ExposeData = typeof( Detour._PlaySettings ).GetMethod( "_ExposeData", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_PlaySetting_ExposeData, CCL_PlaySetting_ExposeData ) )
                return false;

            // Detour RimWorld.PlaySettings.DoPlaySettingsGlobalControls
            MethodInfo RimWorld_PlaySettings_DoPlaySettingsGlobalControls = typeof( PlaySettings ).GetMethod( "DoPlaySettingsGlobalControls", UniversalBindingFlags );
            MethodInfo CCL_PlaySettings_DoPlaySettingsGlobalControls = typeof( Detour._PlaySettings ).GetMethod( "_DoPlaySettingsGlobalControls", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( RimWorld_PlaySettings_DoPlaySettingsGlobalControls, CCL_PlaySettings_DoPlaySettingsGlobalControls ) )
                return false;
            
            /*
            // Detour 
            MethodInfo foo = typeof( foo_class ).GetMethod( "foo_method", UniversalBindingFlags );
            MethodInfo CCL_bar = typeof( Detour._bar ).GetMethod( "_bar_method", UniversalBindingFlags );
            if( !Detours.TryDetourFromTo( foo, CCL_bar ) )
                return false;

            */

            return true;
        }

        private void                        FixGlowers()
        {
            foreach( var def in DefDatabase<ThingDef>.AllDefs.Where( def => (
                ( def != null )&&
                ( def.comps != null )&&
                ( def.HasComp( typeof( CompGlower ) ) )
            ) ) )
            {
                var compGlower = def.GetCompProperties<CompProperties_Glower>();
                compGlower.compClass = typeof( CompGlowerToggleable );
            }
        }

        private void                        UpgradeNutrientPasteDispensers()
        {
            foreach( var def in DefDatabase<ThingDef>.AllDefs.Where( def => def.thingClass == typeof( Building_NutrientPasteDispenser ) ) )
            {
                def.thingClass = typeof( Building_AdvancedPasteDispenser );
            }
        }

    }

}
