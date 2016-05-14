﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using UnityEngine;
using Verse;

namespace CommunityCoreLibrary.MiniMap
{

	public class MiniMapOverlay_Colonists : MiniMapOverlay_Pawns
	{

		#region Constructors

		public MiniMapOverlay_Colonists( MiniMap minimap, MiniMapOverlayDef overlayDef ) : base( minimap, overlayDef )
		{
		}

        #endregion Constructors

        #region Methods

        public override Color GetColor( Pawn pawn, float opacity = 1f )
		{
			var color = Color.green;
			color.a = opacity;
			return color;
		}

		public override IEnumerable<Pawn> GetPawns()
		{
			return Find.MapPawns.FreeColonists;
		}

		#endregion Methods

	}

}