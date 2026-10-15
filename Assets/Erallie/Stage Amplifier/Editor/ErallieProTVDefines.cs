#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;

[InitializeOnLoad]
public static class ErallieProTVDefines
{
    private const string Symbol = "ERALLIE_PROTV";

    static ErallieProTVDefines()
    {
        EditorApplication.delayCall += Refresh;
        Events.registeredPackages += OnPackagesChanged;
    }

    private static void OnPackagesChanged(PackageRegistrationEventArgs args)
    {
        EditorApplication.delayCall += Refresh;
    }

    [MenuItem("Tools/Erallie/Refresh ProTV Integration")]
    public static void Refresh()
    {
        var packages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
        if (packages == null)
        {
            return;
        }

        bool installed = packages.Any(package => package.name == "dev.architech.protv");
        var groups = new HashSet<BuildTargetGroup>();
        foreach (BuildTargetGroup group in Enum.GetValues(typeof(BuildTargetGroup)))
        {
            if (group == BuildTargetGroup.Unknown || !groups.Add(group))
            {
                continue;
            }

            var field = typeof(BuildTargetGroup).GetField(group.ToString());
            if (field == null || Attribute.IsDefined(field, typeof(ObsoleteAttribute)))
            {
                continue;
            }

            string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var defines = current.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            bool changed = false;
            if (installed && !defines.Contains(Symbol))
            {
                defines.Add(Symbol);
                changed = true;
            }
            else if (!installed)
            {
                changed = defines.RemoveAll(value => value == Symbol) > 0;
            }

            if (changed)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines));
            }
        }
    }
}
#endif
