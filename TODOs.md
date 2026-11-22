# TODOs

## Pre-release polish

- **Workshop gallery** Add images, Medic/Overlord as precedent
- **Smoke test on 1.6**
- **Quest pilots in game** Generate both sanguophage quests at 100% chance (dev quest generation) and check:
  - the join offer and a capture
  - under Combat Extended, that a dressed pawn's autorifle fires and reloads from the three spare magazines
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README

## Later

- **AI use of the rush and Dismember** A pilot the player does not control casts neither (VEF fires no apparel ability's autocast), so a quest's sanguophage in the set fights with its plating alone.
- **AI sanguophages and their own abilities** Check whether a quest's sanguophage uses its gene abilities in a fight (piercing spine, longjump, coagulate), in the set and out of it.
- **Flaky test host: turn off xunit's shadow copy.** About one `dotnet test` run in thirty crashes the host or fails every test with "Method has zero rva". xunit's net472 runner shadow-copies the test folder by default, and mono's shadow copy takes no lock, so the test classes xunit runs in parallel copy and map the same DLL at once (`xunit.assert.dll`, `UnityEngine.SharedInternalsModule.dll`). The 1.7 install plays no part: the host loads the frozen 1.6 DLLs. `dotnet test -- xUnit.ShadowCopy=false` ran 120 of 120 clean. The fix is an `xunit.runner.json` holding `"shadowCopy": false`, copied to the test output, made in `../RimworldModTemplate/` first and mirrored here and in the siblings.
