using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Original Author - Ben Lussman

/// <summary>
/// This class is used to control file paths in all scripts for ease of changing file paths during development
/// </summary>
public static class FilePaths
{
    //static file paths
    public readonly static string GENERATIONFILES = Application.dataPath + "/GenerationFiles";
    public readonly static string PARTANIMATIONS = GENERATIONFILES + "/PartAnimation";
    public readonly static string PARTSTILLS = GENERATIONFILES + "/PartStills";
    public readonly static string RESOURCES = Application.dataPath + "/Resources";
    public readonly static string SCRIPTABLEPARTS = RESOURCES + "/Parts";
    public readonly static string PARTMODELS = RESOURCES + "/Models";
    public readonly static string PARTWEAPONS = PARTMODELS + "/Weapons";
    public readonly static string PARTUTILITY = PARTMODELS + "/Utility";
    public readonly static string PARTMOVEMENT = PARTMODELS + "/Movement";
    public readonly static string PARTCHASIS = PARTMODELS + "/Chasis";
    public readonly static string TEMPORARYFOLDER = GENERATIONFILES+"/AAAAAA";
    public readonly static string PARTID = Application.dataPath + "/ScriptableObjects/Parts/StringIDs/PartIDs/";


    /// <summary>
    /// Get the path for a Part still image given a part name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>string</returns>
    public static string GETPARTSTILLPATH(string name)
    {
        return PARTSTILLS + "/" + name + ".png";
    }

    /// <summary>
    /// Destroy a Part Still image given a name
    /// </summary>
    /// <param name="name"></param>
    public static void DESTROYPARTSTILLPATH(string name)
    {
        System.IO.File.Delete (PARTSTILLS + "/" + name + ".png");
    }

    /// <summary>
    /// Get the path for a scriptable object image given a part name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>string</returns>
    public static string GETPARTSCRIPTABLE(string name)
    {
        return SCRIPTABLEPARTS + "/" + name + ".asset";
    }

    /// <summary>
    /// Get the path for a Part still image in temporary folder to make animation based on part name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>string</returns>
    public static string GETPNGFORTEMPORARYFOLDER(string name)
    {
        return TEMPORARYFOLDER + "/" + name + ".png";
    }

    /// <summary>
    /// Delete the temporary folder
    /// </summary>
    public static void DELETETEMPORARYFOLDER()
    {
        System.IO.Directory.Delete(TEMPORARYFOLDER);
    }

    /// <summary>
    /// Make a Folder at a given path
    /// </summary>
    public static void MAKEDIRECTORY(string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            Debug.Log("Folder");
            System.IO.Directory.CreateDirectory(path);
            REFRESHASSETDATABASE();
        }
    }

    /// <summary>
    /// Get the path for a Part_ID given a name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>string</returns>
    public static string GETPARTID(string name)
    {
        return PARTID + name + ".asset";
    }


    /// <summary>
    /// refresh the asset database in unity editor
    /// </summary>
    public static void REFRESHASSETDATABASE()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
