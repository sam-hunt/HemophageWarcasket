using System.Reflection;
using System.Runtime.InteropServices;

// Kept explicit (GenerateAssemblyInfo=false in the csproj) so the release flow can bump
// AssemblyVersion/AssemblyFileVersion in lockstep with About.xml's <modVersion> and
// CHANGELOG.md. Four-part X.Y.Z.0, where X.Y.Z is the mod's semantic version.
[assembly: AssemblyTitle("HemophageWarcasket")]
[assembly: AssemblyDescription("The Hemophage warcasket set for Vanilla Factions Expanded - Pirates")]
[assembly: AssemblyProduct("HemophageWarcasket")]
[assembly: AssemblyCopyright("Copyright © 2026")]
[assembly: ComVisible(false)]
[assembly: Guid("3ba538af-71e1-4f31-b4f8-ea7bdfdb6f85")]
[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1.0.0")]
// Mirrors About.xml <modVersion> verbatim, including any SemVer prerelease suffix
// (1.4.0-rc.1); the two numeric attributes above can't hold one and stay X.Y.Z.0.
[assembly: AssemblyInformationalVersion("1.1.0")]
