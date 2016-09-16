﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using RimWorld;
using UnityEngine;
using Verse;

namespace CommunityCoreLibrary.Controller
{

    public class MainMonoBehaviour : MonoBehaviour
    {

        #region Instance Data

        private static bool                 initOk;
        private static bool                 gameValid;

        private static int                  ticks;

        private List<SubController>         UpdateControllers = null;

        private static MethodInfo           _DoPlayLoad;
        private static bool                 queueRecovering = false;
        private static bool                 queueLoadAllPlayData = false;

        // A14 - PDL.loaded is now private, Loaded is get only.
        private static FieldInfo PlayDataLoader_loaded = typeof( PlayDataLoader ).GetField( "loadedInt", BindingFlags.NonPublic | BindingFlags.Static );

        #endregion

        #region Destructor


#if DEVELOPER
        /* Should be a static but can't */  ~MainMonoBehaviour()
        {
            CCL_Log.CloseStream();

        }

#endif

        #endregion

        #region Preloader

        internal static void                PreLoad()
        {
            // This is a pre-start sequence to hook some deeper level functions.
            // These functions can be hooked later but it would be after the sequence
            // of operations which call them is complete.

#if DEVELOPER
            // Open a log file for CCL specific output
            // https://www.youtube.com/watch?v=jyaLZHiJJnE
            if( CCL_Log.OpenStream() == null )
            {
                Log.Error( string.Format( "Unable to open file stream for {0}!", Controller.Data.UnityObjectName ) );
            }
#endif

            // Log CCL version
            Version.Log();

            bool InjectionsOk = true;
            var stringBuilder = new StringBuilder();
            CCL_Log.CaptureBegin( stringBuilder );

            // Find all sub-controllers
            var subControllerClasses = typeof( SubController ).AllSubclasses();
            var subControllerCount = subControllerClasses.Count();
            if( subControllerCount == 0 )
            {
                InjectionsOk = false;
                CCL_Log.Error(
                    "Unable to find sub-controllers",
                    "PreLoader"
                );
            }

            // Create sub-controllers
            if( InjectionsOk )
            {
                var subControllers = new SubController[ subControllerCount ];
                for( int index = 0; index < subControllerCount; ++index )
                {
                    var subControllerType = subControllerClasses.ElementAt( index );
                    var subController = (SubController) Activator.CreateInstance( subControllerType );
                    if( subController == null )
                    {
                        CCL_Log.Error(
                            string.Format( "Unable to create sub-controller {0}", subControllerType.Name ),
                            "PreLoader"
                        );
                        InjectionsOk = false;
                        break;
                    }
                    else
                    {
                        subControllers[ index ] = subController;
                    }
                }
                if( InjectionsOk )
                {
                    Controller.Data.SubControllers = subControllers;
                }
            }

            // Detour Verse.PlayDataLoader.LoadAllPlayData
            if( InjectionsOk )
            {
                MethodInfo Verse_PlayDataLoader_LoadAllPlayData = typeof( PlayDataLoader ).GetMethod( "LoadAllPlayData", BindingFlags.Static | BindingFlags.Public );
                MethodInfo CCL_PlayDataLoader_LoadAllPlayData = typeof( Detour._PlayDataLoader ).GetMethod( "_LoadAllPlayData", BindingFlags.Static | BindingFlags.NonPublic );
                InjectionsOk &= Detours.TryDetourFromTo( Verse_PlayDataLoader_LoadAllPlayData, CCL_PlayDataLoader_LoadAllPlayData );
            }

            // Detour Verse.PlayDataLoader.ClearAllPlayData
            if( InjectionsOk )
            {
                MethodInfo Verse_PlayDataLoader_ClearAllPlayData = typeof( PlayDataLoader ).GetMethod( "ClearAllPlayData", BindingFlags.Static | BindingFlags.Public );
                MethodInfo CCL_PlayDataLoader_ClearAllPlayData = typeof( Detour._PlayDataLoader ).GetMethod( "_ClearAllPlayData", BindingFlags.Static | BindingFlags.NonPublic );
                InjectionsOk &= Detours.TryDetourFromTo( Verse_PlayDataLoader_ClearAllPlayData, CCL_PlayDataLoader_ClearAllPlayData );
            }

            // Detour Verse.UIRoot_Entry.ShouldDoMainMenu_get
            if( InjectionsOk )
            {
                PropertyInfo Verse_UIRoot_Entry_ShouldDoMainMenu = typeof( UIRoot_Entry ).GetProperty( "ShouldDoMainMenu", BindingFlags.Instance | BindingFlags.NonPublic );
                MethodInfo Verse_UIRoot_Entry_ShouldDoMainMenu_get = Verse_UIRoot_Entry_ShouldDoMainMenu.GetGetMethod( true );
                MethodInfo CCL_UIRoot_Entry_ShouldDoMainMenu_get= typeof( Detour._UIRoot_Entry ).GetMethod( "_ShouldDoMainMenu_get", BindingFlags.Static | BindingFlags.NonPublic );
                InjectionsOk &= Detours.TryDetourFromTo( Verse_UIRoot_Entry_ShouldDoMainMenu_get, CCL_UIRoot_Entry_ShouldDoMainMenu_get );
            }

            // Detour RimWorld.MainMenuDrawer.MainMenuOnGUI
            if( InjectionsOk )
            {
                MethodInfo RimWorld_MainMenuDrawer_MainMenuOnGUI = typeof( MainMenuDrawer ).GetMethod( "MainMenuOnGUI", BindingFlags.Static | BindingFlags.Public );
                MethodInfo CCL_MainMenuDrawer_MainMenuOnGUI = typeof( Detour._MainMenuDrawer ).GetMethod( "_MainMenuOnGUI", BindingFlags.Static | BindingFlags.NonPublic );
                InjectionsOk &= Detours.TryDetourFromTo( RimWorld_MainMenuDrawer_MainMenuOnGUI, CCL_MainMenuDrawer_MainMenuOnGUI );
            }

            // Detour RimWorld.MainMenuDrawer.DoMainMenuButtons
            if( InjectionsOk )
            {
                MethodInfo RimWorld_MainMenuDrawer_DoMainMenuButtons = typeof( MainMenuDrawer ).GetMethod( "DoMainMenuControls", BindingFlags.Static | BindingFlags.Public );
                MethodInfo CCL_MainMenuDrawer_DoMainMenuButtons = typeof( Detour._MainMenuDrawer ).GetMethod( "_DoMainMenuButtons", BindingFlags.Static | BindingFlags.NonPublic );
                InjectionsOk &= Detours.TryDetourFromTo( RimWorld_MainMenuDrawer_DoMainMenuButtons, CCL_MainMenuDrawer_DoMainMenuButtons );
            }

            // Detour RimWorld.VersionControl.DrawInfoInCorner
            if( InjectionsOk )
            {
                MethodInfo RimWorld_VersionControl_DrawInfoInCorner = typeof( VersionControl ).GetMethod( "DrawInfoInCorner", BindingFlags.Static | BindingFlags.Public );
                MethodInfo CCL_VersionControl_DrawInfoInCorner = typeof( Detour._VersionControl ).GetMethod( "_DrawInfoInCorner", BindingFlags.Static | BindingFlags.NonPublic );
                InjectionsOk &= Detours.TryDetourFromTo( RimWorld_VersionControl_DrawInfoInCorner, CCL_VersionControl_DrawInfoInCorner );
            }

            // Detour Verse.PostLoadInitter.DoAllPostLoadInits
            /*
            if( InjectionsOk )
            {
                MethodInfo Verse_PostLoadInitter_DoAllPostLoadInits = typeof( PostLoadInitter ).GetMethod( "DoAllPostLoadInits", BindingFlags.Static | BindingFlags.Public );
                MethodInfo CCL_PostLoadInitter_DoAllPostLoadInits = typeof( Detour._PostLoadInitter ).GetMethod( "_DoAllPostLoadInits", BindingFlags.Static | BindingFlags.NonPublic );
                InjectionsOk &= Detours.TryDetourFromTo( Verse_PostLoadInitter_DoAllPostLoadInits, CCL_PostLoadInitter_DoAllPostLoadInits );
            }
            */

            if( InjectionsOk )
            {
                LongEventHandler.ExecuteWhenFinished( CreateMonoBehaviour );
            }

            CCL_Log.CaptureEnd(
                stringBuilder,
                InjectionsOk ? "Initialized" : "Errors during injection"
            );
            CCL_Log.Trace(
                Verbosity.Injections,
                stringBuilder.ToString(),
                "PreLoader" );
            
            initOk = InjectionsOk;
        }

        #endregion

        #region Reload RimWorld

        public static void ReloadRimWorld()
        {
            Controller.Data.ReloadingPlayData = true;
            LongEventHandler.QueueLongEvent(
                ReloadQueue,
                "",
                true,
                null
            );
        }

        private static void ReloadQueue()
        {
            ClearAllPlayData();
            if( queueLoadAllPlayData )
            {
                LoadAllPlayData( queueRecovering );
            }
            Controller.Data.ReloadingPlayData = false;
        }

        #endregion

        #region Restart RimWorld

        internal static void RestartRimWorld()
        {
            var args = Environment.GetCommandLineArgs();
            var commandLine = "\"" + args[ 0 ] + "\"";
            var arguements = string.Empty;
            for( int index = 1; index < args.GetLength( 0 ); ++index )
            {
                if( index > 1 )
                {
                    arguements += " ";
                }
                arguements += "\"" + args[ index ] + "\"";
            }
#if DEVELOPER
            Log.Message( "Restarting RimWorld:\n" + commandLine + " " + arguements );
#endif
            Process.Start( commandLine, arguements );
            Root.Shutdown();
        }

        #endregion

        #region Play Data

        private static void ClearAllPlayData()
        {
            LanguageDatabase.Clear();
            // A14 - ModContentPackManager was removed?
            LoadedModManager.ClearDestroy();
            foreach( Type genericParam in GenTypes.AllSubclasses( typeof( Def ) ) )
            {
                GenGeneric.InvokeStaticMethodOnGenericType( typeof( DefDatabase<> ), genericParam, "Clear" );
            }
            ThingCategoryNodeDatabase.Clear();
            BackstoryDatabase.Clear();
            SolidBioDatabase.Clear();
            
            PlayDataLoader_loaded.SetValue( null, false );
        }

        internal static void QueueLoadAllPlayData( bool recovering = false )
        {
            queueRecovering = recovering;
            queueLoadAllPlayData = true;
        }

        private static void DoPlayLoad()
        {
            if( _DoPlayLoad == null )
            {
                _DoPlayLoad = typeof( PlayDataLoader ).GetMethod( "DoPlayLoad", BindingFlags.Static | BindingFlags.NonPublic );
            }
            _DoPlayLoad.Invoke( null, null );
        }

        internal static void LoadAllPlayData( bool recovering = false )
        {
            if( PlayDataLoader.Loaded )
            {
                Log.Error( "Loading play data when already loaded. Call ClearAllPlayData first." );
            }
            else
            {
                queueRecovering = false;
                queueLoadAllPlayData = false;

                DeepProfiler.Start( "LoadAllPlayData" );
                try
                {
                    DoPlayLoad();
                }
                catch( Exception ex )
                {
                    if( !Prefs.ResetModsConfigOnCrash )
                        throw;
                    else if( recovering )
                    {
                        Log.Warning( "Could not recover from errors loading play data. Giving up." );
                        throw;
                    }
                    else
                    {
                        IEnumerable<ModMetaData> activeMods = ModsConfig.ActiveModsInLoadOrder;
                        if( Enumerable.Count<ModMetaData>( activeMods ) == 1 && Enumerable.First<ModMetaData>( activeMods ).IsCoreMod )
                        {
                            throw;
                        }
                        else
                        {
                            Log.Warning( "Caught exception while loading play data but there are active mods other than Core. Resetting mods config and trying again.\nThe exception was: " + (object)ex );
                            try
                            {
                                PlayDataLoader.ClearAllPlayData();
                            }
                            catch
                            {
                                Log.Warning( "Caught exception while recovering from errors and trying to clear all play data. Ignoring it.\nThe exception was: " + (object)ex );
                            }
                            ModsConfig.Reset();
                            CrossRefLoader.Clear();
                            PostLoadInitter.Clear();
                            PlayDataLoader.LoadAllPlayData( true );
                            return;
                        }
                    }
                }
                finally
                {
                    DeepProfiler.End();
                }
                // A14 - PlayDataLoader.loaded is now private, Loaded property is getter only
                PlayDataLoader_loaded.SetValue( null, false );
                if ( !recovering )
                    return;
                Log.Message( "Successfully recovered from errors and loaded play data." );
                DelayedErrorWindowRequest.Add( Translator.Translate( "RecoveredFromErrorsText" ), Translator.Translate( "RecoveredFromErrorsDialogTitle" ) );
            }
        }

        #endregion

        #region Mono Callbacks

        public void                         Start()
        {
            enabled = true;
        }

        public void                         FixedUpdate()
        {
            ticks++;
            if(
                ( !gameValid )||
                ( Current.ProgramState != ProgramState.MapPlaying )||
                ( Find.Map == null )||
                ( Find.Map.components == null )
            )
            {
                // Do nothing until the game has fully loaded the map and is ready to play
                return;
            }

            if( Scribe.mode == LoadSaveMode.LoadingVars )
            {
                LongEventHandler.QueueLongEvent( ReIntialize, "Initializing", true, null );
            }
            if( Scribe.mode != LoadSaveMode.Inactive )
            {
                // Do nothing while a save/load sequence is happening
                return;
            }

            LongEventHandler.ExecuteWhenFinished( UpdateSubControllers );
        }

        public void                         OnLevelWasLoaded( int level )
        {
            // Enable the frame update when the game and map are valid
            // Level 1 means we're in gameplay.
            // enabled = ( ( gameValid )&&( level == 1 ) ) ? true : false;
        }

        #endregion

        #region Long Event Handlers

        public static void                         CreateMonoBehaviour()
        {
            var gameObject = new GameObject( Controller.Data.UnityObjectName );
            if( gameObject == null )
            {
                CCL_Log.Error( "Unable to create GameObject" );
                initOk = false;
                return;
            }
            else
            {
                if( gameObject.AddComponent< Controller.MainMonoBehaviour >() == null )
                {
                    CCL_Log.Error( "Unable to create MonoBehaviour" );
                    initOk = false;
                    return;
                }
                else
                {
                    UnityEngine.Object.DontDestroyOnLoad( gameObject );
                    Controller.Data.UnityObject = gameObject;
                }
            }

            CCL_Log.Message( "Queueing Library Initialization" );
            LongEventHandler.QueueLongEvent( Initialize, "LibraryStartup", true, null );
        }

        public static void                         Initialize()
        {
            //enabled = false;
            gameValid = false;

            if( !initOk )
            {
                return;
            }

            var subControllers = Controller.Data.SubControllers.ToList();
            if( subControllers.NullOrEmpty() )
            {
                CCL_Log.Error( "SubControllers array is empty!" );
                return;
            }

            LongEventHandler.SetCurrentEventText( "LibraryValidation".Translate() );

            // Validate all subs-systems
            subControllers.Sort( (x,y) => ( x.ValidationPriority > y.ValidationPriority ) ? -1 : 1 );
            foreach( var subsys in subControllers )
            {
                if( subsys.ValidationPriority != SubController.DontProcessThisPhase )
                {
                    if( !subsys.Validate() )
                    {
                        CCL_Log.Error( subsys.strReturn, subsys.Name + " :: Validation"  );
                        return;
                    }
                    if( subsys.strReturn != string.Empty )
                    {
                        CCL_Log.Message( subsys.strReturn, subsys.Name + " :: Validations" );
                    }
                }
                else
                {
                    subsys.State = SubControllerState.Validated;
                }
            }

            LongEventHandler.SetCurrentEventText( "Initializing".Translate() );

            // Initialize all sub-systems
            subControllers.Sort( (x,y) => ( x.InitializationPriority > y.InitializationPriority ) ? -1 : 1 );
            foreach( var subsys in subControllers )
            {
                if( subsys.InitializationPriority != SubController.DontProcessThisPhase )
                {
                    if( !subsys.Initialize() )
                    {
                        CCL_Log.Error( subsys.strReturn, subsys.Name + " :: Initialization" );
                        return;
                    }
                    if( subsys.strReturn != string.Empty )
                    {
                        CCL_Log.Message( subsys.strReturn, subsys.Name + " :: Initialization" );
                    }
                }
                else
                {
                    subsys.State = SubControllerState.Ok;
                }
            }

            CCL_Log.Message( "Initialized" );

            // Yay!
            gameValid = true;
            //enabled = true;
            ticks = 0;
        }

        public void                         ReIntialize()
        {
            // Call controller Initialize() on game load
            var subControllers = Controller.Data.SubControllers.ToList();
            subControllers.Sort( (x,y) => ( x.InitializationPriority > y.InitializationPriority ) ? -1 : 1 );

            foreach( var subsys in subControllers )
            {
                if( subsys.InitializationPriority != SubController.DontProcessThisPhase )
                {
                    if(
                        ( subsys.State >= SubControllerState._BaseOk )&&
                        ( subsys.ReinitializeOnGameLoad )
                    )
                    {
                        if( !subsys.Initialize() )
                        {
                            CCL_Log.Error( subsys.strReturn, subsys.Name + " :: Reinitialization" );
                            gameValid = false;
                            enabled = false;
                            return;
                        }
                        if( subsys.strReturn != string.Empty )
                        {
                            CCL_Log.Message( subsys.strReturn, subsys.Name + " :: Reinitialization" );
                        }
                    }
                }
            }
            ticks = 0;
        }

        public void                         UpdateSubControllers()
        {
            if( UpdateControllers == null )
            {
                // Create a list of sub controllers in update order
                UpdateControllers = Controller.Data.SubControllers.ToList();
                UpdateControllers.Sort( (x,y) => ( x.UpdatePriority > y.UpdatePriority ) ? -1 : 1 );
            }

            foreach( var subsys in UpdateControllers )
            {
                if( subsys.UpdatePriority != SubController.DontProcessThisPhase )
                {
                    if(
                        ( subsys.State == SubControllerState.Ok )&&
                        ( subsys.IsHashIntervalTick( ticks ) )
                    )
                    {
                        if( !subsys.Update() )
                        {
                            CCL_Log.Error( subsys.strReturn, subsys.Name + " :: Update" );
                            return;
                        }
                        if( subsys.strReturn != string.Empty )
                        {
                            CCL_Log.Message( subsys.strReturn, subsys.Name + " :: Update" );
                        }
                    }
                }
            }

        }

        #endregion

    }

}
