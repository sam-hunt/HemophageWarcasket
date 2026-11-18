#!/usr/bin/env python3
# Hemophage Warcasket's config shim over the shared translation checker
# (l10n/checker/check_translations.py, the rimworld-l10n submodule). The
# engine holds all logic; this file holds only this repo's config and the
# rationale behind it. Usage is unchanged:
#   python3 Scripts/check-translations.py [--strict] [--root PATH]
# If l10n/ is empty, run: git submodule update --init

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "checker"))
import check_translations as engine  # noqa: E402  (import after sys.path edit)

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

# No [TranslationCanChangeCount]-style matching-token fields in this repo:
# the surface is three apparel defs' label/description text.
engine.PARITY_EXEMPT_FIELDS = set()

# RATIONALE: Biotech is a hard dependency (About.xml's modDependencies): the
# set is built around Biotech's hemogenic gene, and its mechanics will
# reference Biotech's defs, so the mod is never loaded without the DLC and
# the sidecar is generated with it. Odyssey is pinned as well: no def gates
# on it, only stat LEAVES carry MayRequire for it (VacuumResistance), and a
# gated leaf drops a number, not a def, but Vanilla Gravship Expanded's
# compat root requires it and if an Odyssey-only def or DefInjected ever
# lands, a sidecar generated without Odyssey would silently lose its keys.
# Ideology stays out: its only footprint is a MayRequire on a stat leaf
# (SlaveSuppressionOffset).
engine.REQUIRED_DLCS = {"Biotech", "Odyssey"}

# The three warcasket parts are VFEPirates.WarcasketDefs, a ThingDef
# subclass with no database of its own (it adds shortDescription and three
# role flags). The game rolls it into DefDatabase<ThingDef>, so the probe's
# walker dumps these defs under ThingDef and DefInjected translations
# legally target 1.6/Languages/<Language>/DefInjected/ThingDef/. The checker
# needs the XML element tag mapped to that folder to match Defs/ against the
# sidecar.
engine.DEF_TYPE_ALIASES = {
    "VFEPirates.WarcasketDef": "ThingDef",
}

# Every code-printed string lives in the one Keyed file,
# 1.6/Languages/English/Keyed/HemophageWarcasket.xml, so a language without
# a Keyed folder is missing text.
engine.ALLOW_NO_KEYED_SURFACE = False

# The Workshop title is coupled to the settings window's header key, as in
# the sibling repos: a language's Workshop page titles the mod as its
# settings window does.
engine.WORKSHOP_TITLE_KEY = "HPWC_SettingsCategory"

raise SystemExit(engine.main())
