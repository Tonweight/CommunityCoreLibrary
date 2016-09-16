﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RimWorld;
using Verse;
using UnityEngine;

namespace CommunityCoreLibrary
{

    public static class ThingDef_Extensions
    {

        internal static FieldInfo           _allRecipesCached;

        // Dummy for functions needing a ref list
        public static List<Def>             nullDefs = null;

        #region Recipe Cache

        public static void                  RecacheRecipes( this ThingDef thingDef, bool validateBills )
        {
            if( _allRecipesCached == null )
            {
                _allRecipesCached = typeof( ThingDef ).GetField( "allRecipesCached", BindingFlags.Instance | BindingFlags.NonPublic );
            }
            _allRecipesCached.SetValue( thingDef, null );

            if(
                ( !validateBills )||
                ( Current.ProgramState != ProgramState.MapPlaying )
            )
            {
                return;
            }

            // Get the recached recipes
            var recipes = thingDef.AllRecipes;

            // Remove bill on any table of this def using invalid recipes
            var buildings = Find.ListerBuildings.AllBuildingsColonistOfDef( thingDef );
            foreach( var building in buildings )
            {
                var iBillGiver = building as IBillGiver;
                if( iBillGiver != null )
                {
                    for( int i = 0; i < iBillGiver.BillStack.Count; ++ i )
                    {
                        var bill = iBillGiver.BillStack[ i ];
                        if( !recipes.Exists( r => bill.recipe == r ) )
                        {
                            iBillGiver.BillStack.Delete( bill );
                            continue;
                        }
                    }
                }
                var factory = building as Building_AutomatedFactory;
                if( factory != null )
                {
                    factory.ResetAndReprogramHoppers();
                }
            }

        }

        #endregion

        #region Availability

        public static bool                  IsFoodMachine( this ThingDef thingDef )
        {
            if( typeof( Building_NutrientPasteDispenser ).IsAssignableFrom( thingDef.thingClass ) )
            {
                return true;
            }
            if( typeof( Building_AutomatedFactory ).IsAssignableFrom( thingDef.thingClass ) )
            {   // Make sure we are only return factories which are configured as food synthesizers
                var propsFactory = thingDef.GetCompProperty<CompProperties_AutomatedFactory>();
                if( propsFactory != null )
                {
                    if(
                        ( propsFactory.outputVector == FactoryOutputVector.DirectToPawn )&&
                        ( propsFactory.productionMode == FactoryProductionMode.PawnInteractionOnly )
                    )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool                  IsIngestible( this ThingDef thingDef )
        {
            return thingDef.ingestible != null;
        }

        public static bool                  IsAlcohol( this ThingDef thingDef )
        {
            if(
                ( thingDef.IsIngestible() )&& 
                ( thingDef.ingestible.foodType == FoodTypeFlags.Liquor )
            )
            {
                return true;
            }
            return false;
        }

        public static bool                  IsImplant( this ThingDef thingDef )
        {
            // Return true if a recipe exist implanting this thing def
            return
                DefDatabase< RecipeDef >.AllDefsListForReading.Exists( r => (
                    ( r.addsHediff != null )&&
                    ( r.IsIngredient( thingDef ) )
                ) );
        }

        public static RecipeDef             GetImplantRecipeDef( this ThingDef thingDef )
        {
            // Get recipe for implant
            return
                DefDatabase< RecipeDef >.AllDefsListForReading.Find( r => (
                    ( r.addsHediff != null )&&
                    ( r.IsIngredient( thingDef ) )
                ) );
        }

        public static HediffDef             GetImplantHediffDef( this ThingDef thingDef )
        {
            // Get hediff for implant
            var recipeDef = thingDef.GetImplantRecipeDef();
            return recipeDef != null
                ? recipeDef.addsHediff
                    : null;
        }

        public static bool                  EverHasRecipes( this ThingDef thingDef )
        {
            return (
                ( !thingDef.GetRecipesCurrent().NullOrEmpty() )||
                ( !thingDef.GetRecipesUnlocked( ref nullDefs ).NullOrEmpty() )||
                ( !thingDef.GetRecipesLocked( ref nullDefs ).NullOrEmpty() )
            );
        }

        public static bool                  EverHasRecipe( this ThingDef thingDef, RecipeDef recipeDef )
        {
            return (
                ( thingDef.GetRecipesCurrent().Contains( recipeDef ) )||
                ( thingDef.GetRecipesUnlocked( ref nullDefs ).Contains( recipeDef ) )||
                ( thingDef.GetRecipesLocked( ref nullDefs ).Contains( recipeDef ) )
            );
        }

        public static List<JoyGiverDef>     GetJoyGiverDefsUsing( this ThingDef thingDef )
        {
            var joyGiverDefs = DefDatabase<JoyGiverDef>.AllDefsListForReading.Where( def => (
                ( !def.thingDefs.NullOrEmpty() )&&
                ( def.thingDefs.Contains( thingDef ) )
            ) ).ToList();
            return joyGiverDefs;
        }

        public static bool                  ChangeDesignationCategory( this ThingDef thingDef, string newCategory )
        {
            if( string.IsNullOrEmpty( newCategory ) )
            {   // Invalid category
                return false;
            }
            if( thingDef.designationCategory == newCategory )
            {   // Already this category
                return true;
            }
            DesignationCategoryDef newCategoryDef =
                newCategory == "None"
                ? null
                : DefDatabase<DesignationCategoryDef>.GetNamed( newCategory, false );
            DesignationCategoryDef oldCategory = null;
            Designator_Build oldDesignator = null;
            if(
                ( !thingDef.designationCategory.NullOrEmpty() )&&
                ( thingDef.designationCategory != "None" )
            )
            {
                oldCategory = DefDatabase<DesignationCategoryDef>.GetNamed( thingDef.designationCategory );
                oldDesignator = (Designator_Build) oldCategory._resolvedDesignators().FirstOrDefault( d => (
                    ( d is Designator_Build )&&
                    ( ( d as Designator_Build ).PlacingDef == (BuildableDef) thingDef )
                ) );
            }
            if( oldCategory != null )
            {
                oldCategory._resolvedDesignators().Remove( oldDesignator );
            }
            if( newCategoryDef != null )
            {
                Designator_Build newDesignator = null;
                if( oldDesignator != null )
                {
                    newDesignator = oldDesignator;
                }
                else
                {
                    newDesignator = (Designator_Build) Activator.CreateInstance( typeof( Designator_Build ), new System.Object[] { (BuildableDef) thingDef } );
                }
                newCategoryDef._resolvedDesignators().Add( newDesignator );
            }
            thingDef.designationCategory = newCategory;
            return true;
        }

        #endregion

        #region Lists of affected data

        public static List< RecipeDef >     GetRecipesUnlocked( this ThingDef thingDef, ref List< Def > researchDefs )
        {
#if DEBUG
            CCL_Log.TraceMod(
                thingDef,
                Verbosity.Stack,
                "GetRecipesUnlocked()"
            );
#endif
            // Recipes that are unlocked on thing with research
            var recipeDefs = new List<RecipeDef>();
            if( researchDefs != null )
            {
                researchDefs.Clear();
            }

            // Look at recipes
            var recipes = DefDatabase< RecipeDef >.AllDefsListForReading.Where( r => (
                ( r.researchPrerequisite != null )&&
                (
                    (
                        ( r.recipeUsers != null )&&
                        ( r.recipeUsers.Contains( thingDef ) )
                    )||
                    (
                        ( thingDef.recipes != null )&&
                        ( thingDef.recipes.Contains( r ) )
                    )
                )&&
                ( !r.IsLockedOut() )
            ) ).ToList();

            // Look in advanced research too
            var advancedResearch = Controller.Data.AdvancedResearchDefs.Where( a => (
                ( a.IsRecipeToggle )&&
                ( !a.HideDefs )&&
                ( a.thingDefs.Contains( thingDef ) )
            ) ).ToList();

            // Aggregate advanced research
            foreach( var a in advancedResearch )
            {
                recipeDefs.AddRangeUnique( a.recipeDefs );
                if( researchDefs != null )
                {
                    if( a.researchDefs.Count == 1 )
                    {
                        // If it's a single research project, add that
                        researchDefs.AddUnique( a.researchDefs[ 0 ] );
                    }
                    else
                    {
                        // Add the advanced project instead
                        researchDefs.AddUnique( a );
                    }
                }
            }
            return recipeDefs;
        }

        public static List< RecipeDef >     GetRecipesLocked( this ThingDef thingDef, ref List< Def > researchDefs )
        {
#if DEBUG
            CCL_Log.TraceMod(
                thingDef,
                Verbosity.Stack,
                "GetRecipesLocked()"
            );
#endif
            // Things it is locked on with research
            var recipeDefs = new List<RecipeDef>();
            if( researchDefs != null )
            {
                researchDefs.Clear();
            }

            // Look in advanced research
            var advancedResearch = Controller.Data.AdvancedResearchDefs.Where( a => (
                ( a.IsRecipeToggle )&&
                ( a.HideDefs )&&
                ( a.thingDefs.Contains( thingDef ) )
            ) ).ToList();

            // Aggregate advanced research
            foreach( var a in advancedResearch )
            {
                recipeDefs.AddRangeUnique( a.recipeDefs );

                if( researchDefs != null )
                {
                    if( a.researchDefs.Count == 1 )
                    {
                        // If it's a single research project, add that
                        researchDefs.AddUnique( a.researchDefs[ 0 ] );
                    }
                    else if( a.ResearchConsolidator != null )
                    {
                        // Add the advanced project instead
                        researchDefs.AddUnique( a.ResearchConsolidator );
                    }
                }
            }

            return recipeDefs;
        }

        public static List< RecipeDef >     GetRecipesCurrent( this ThingDef thingDef )
        {
#if DEBUG
            CCL_Log.TraceMod(
                thingDef,
                Verbosity.Stack,
                "GetRecipesCurrent()"
            );
#endif
            return thingDef.AllRecipes;
        }

        public static List< RecipeDef >     GetRecipesAll( this ThingDef thingDef )
        {
#if DEBUG
            CCL_Log.TraceMod(
                thingDef,
                Verbosity.Stack,
                "GetRecipesAll()"
            );
#endif
            // Things it is locked on with research
            var recipeDefs = new List<RecipeDef>();

            recipeDefs.AddRangeUnique( thingDef.GetRecipesCurrent() );
            recipeDefs.AddRangeUnique( thingDef.GetRecipesUnlocked( ref nullDefs ) );
            recipeDefs.AddRangeUnique( thingDef.GetRecipesLocked( ref nullDefs ) );

            return recipeDefs;
        }

        #endregion

        #region Comp Properties

        // Get CompProperties by CompProperties class
        // Similar to GetCompProperties which gets CompProperties by compClass
        public static T                     GetCompProperty<T>( this ThingDef thingDef ) where T : CompProperties
        {
            foreach( var comp in thingDef.comps )
            {
                if( comp.GetType() == typeof( T ) )
                {
                    return comp as T;
                }
            }
            return null;
        }

        public static CompProperties        GetCompProperty( this ThingDef thingDef, Type propertyClass )
        {
            foreach( var comp in thingDef.comps )
            {
                if( comp.GetType() == propertyClass )
                {
                    return comp;
                }
            }
            return null;
        }

        public static CommunityCoreLibrary.CompProperties_ColoredLight CompProperties_ColoredLight ( this ThingDef thingDef )
        {
            return thingDef.GetCompProperties<CompProperties_ColoredLight>();
        }

        public static CommunityCoreLibrary.CompProperties_LowIdleDraw CompProperties_LowIdleDraw ( this ThingDef thingDef )
        {
            return thingDef.GetCompProperties<CompProperties_LowIdleDraw>();
        }

        public static CompProperties_Rottable CompProperties_Rottable ( this ThingDef thingDef )
        {
            return thingDef.GetCompProperties<CompProperties_Rottable>();
        }

        #endregion

    }

}
