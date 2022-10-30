using UnityEngine;
using UnityEngine.InputSystem;

namespace DuolBots
{
    public class RebindSaveLoad : MonoBehaviour
    {
        public InputActionAsset actions;

        // IDEA
        // every time we rebind, we call the custom input binding function and try and save to that using player prefs
        public void OnEnable()
        {
            var rebinds = PlayerPrefs.GetString("rebinds");
            if (!string.IsNullOrEmpty(rebinds))
                actions.LoadBindingOverridesFromJson(rebinds);
            
        }

        public void OnDisable()
        {
            var rebinds = actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", rebinds);
        }

    }
}
/*
 // Saving.
var overrides = new Dictionary<Guid, string>();
foreach (var map in asset.actionMaps)
    foreach (var binding in map.bindings)
    {
        if (!string.IsNullOrEmpty(binding.overridePath))
            overrides[binding.id] = binding.overridePath;
    }
 
// Loading.
foreach (var map in asset.actionMaps)
{
    var bindings = map.bindings;
    for (var i = 0; i < bindings.Count; ++i)
    {
        if (overrides.TryGetValue(bindings[i].id, out var overridePath)
            map.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
    }
}
 
*/
