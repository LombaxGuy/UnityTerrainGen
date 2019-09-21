//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(MapDisplay))]
//public class MapDisplayEditor : Editor {
//    public override void OnInspectorGUI()
//    {
//        MapDisplay mapDisplay = (MapDisplay)target;

//        if (DrawDefaultInspector())
//        {
//            if (mapDisplay.autoUpdate)
//            {
//                mapDisplay.DrawMapInEditor();
//            }
//        }

//        if (GUILayout.Button("Display"))
//        {
//            mapDisplay.DrawMapInEditor();
//        }

//        if (mapDisplay.drawMode != MapDisplay.DrawMode.Mesh)
//        {
//            GUI.enabled = false;
//        }

//        if (GUILayout.Button("Bake Mesh Collider"))
//        {
//            mapDisplay.BakeCollisions();
//        }

//        if (GUILayout.Button("Place Clutter"))
//        {
//            mapDisplay.PlaceClutter();
//        }

//        GUI.enabled = true;
//    }
//}
