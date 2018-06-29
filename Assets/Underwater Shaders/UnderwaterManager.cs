// ===============================
// Underwater Shader
// Owen Wilson
// 01/24/2017
//==================================

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnderwaterShaders
{
    [ExecuteInEditMode]
    public class UnderwaterManager : MonoBehaviour
    {
        [Header( "Material Properties" )]
        public float causticsTilingX = 0.5f;
        public float causticsTilingY = 0.5f;
        public float causticsOffsetX = 0;
        public float causticsOffsetY = 0;
        public float causticsSpeed = 1f;
        public float causticsBoost = 0;
        public float causticsIntensityA = 1f;
        public float causticsIntensityB = 1f;
        public float causticsPositionA = 10f;
        public float causticsPositionB = 0;
        public Color fogColorA = new Color( 0, 0.27f, 0.3f );
        public Color fogColorB = new Color( 0, 0.27f, 0.3f );
        public float fogIntensityA = 1f;
        public float fogIntensityB = 1f;
        public float fogPositionA = 10f;
        public float fogPositionB = 0;
        public float fogStart = 0;
        public float fogEnd = 15f;
        
        public Material[] materials;

        void Update()
        {
            Shader.EnableKeyword( "SHADER_CONTROL" );
            Shader.DisableKeyword( "SCRIPT_CONTROL" );

            foreach( Material mat in materials )
            {
                mat.DisableKeyword( "SHADER_CONTROL" );
                mat.EnableKeyword( "SCRIPT_CONTROL" );
                mat.SetVector( "_CausticsCoord", new Vector4( causticsTilingX, causticsTilingY, causticsOffsetX, causticsOffsetY ) );
                mat.SetFloat( "_CausticsSpeed", causticsSpeed );
                mat.SetFloat( "_CausticsBoost", causticsBoost );
                mat.SetFloat( "_CausticsIntensity0", causticsIntensityA );
                mat.SetFloat( "_CausticsIntensity1", causticsIntensityB );
                mat.SetFloat( "_CausticsPosition0", causticsPositionA );
                mat.SetFloat( "_CausticsPosition1", causticsPositionB );
                mat.SetColor( "_FogColor0", fogColorA );
                mat.SetColor( "_FogColor1", fogColorB );
                mat.SetFloat( "_FogIntensity0", fogIntensityA );
                mat.SetFloat( "_FogIntensity1", fogIntensityB );
                mat.SetFloat( "_FogPosition0", fogPositionA );
                mat.SetFloat( "_FogPosition1", fogPositionB );
                mat.SetFloat( "_FogStart", fogStart );
                mat.SetFloat( "_FogEnd", fogEnd );
                mat.SetFloat( "_Animation", ( Time.time * causticsSpeed * 0.05f ) % 1 );
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor( typeof( UnderwaterManager ) )]
    public class UnderwaterManagerEditor : Editor
    {
        private bool _showCaustics = true;
        private bool _showFog = true;

        public override void OnInspectorGUI()
        {
            UnderwaterManager manager = (UnderwaterManager)target;

            EditorGUILayout.Space();

            _showCaustics = EditorGUILayout.Foldout( _showCaustics, "Caustics" );
            if( _showCaustics )
            {
                EditorGUIUtility.labelWidth = 20;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "Tiling", GUILayout.Width( 75 ) );
                manager.causticsTilingX = EditorGUILayout.FloatField( "X:", manager.causticsTilingX );
                manager.causticsTilingY = EditorGUILayout.FloatField( "Y:", manager.causticsTilingY );
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "Offset", GUILayout.Width( 75 ) );
                manager.causticsOffsetX = EditorGUILayout.FloatField( "X:", manager.causticsOffsetX );
                manager.causticsOffsetY = EditorGUILayout.FloatField( "Y:", manager.causticsOffsetY );
                EditorGUILayout.EndHorizontal();

                EditorGUIUtility.labelWidth = 80;

                manager.causticsSpeed = EditorGUILayout.FloatField( "Speed:", manager.causticsSpeed );
                manager.causticsBoost = EditorGUILayout.Slider( "Boost:", manager.causticsBoost, 0, 1 );
                manager.causticsIntensityA = EditorGUILayout.Slider( new GUIContent( "Intensity A:", "Intensity of caustics at position A" ), manager.causticsIntensityA, 0, 1 );
                manager.causticsIntensityB = EditorGUILayout.Slider( new GUIContent( "Intensity B:", "Intensity of caustics at position B" ), manager.causticsIntensityB, 0, 1 );
                manager.causticsPositionA = EditorGUILayout.FloatField( new GUIContent( "Position A:", "World space position in Y (Vertical)" ), manager.causticsPositionA );
                manager.causticsPositionB = EditorGUILayout.FloatField( new GUIContent( "Position B:", "World space position in Y (Vertical)" ), manager.causticsPositionB );
            }

            EditorGUILayout.Space();

            _showFog = EditorGUILayout.Foldout( _showFog, "Fog" );
            if( _showFog )
            {
                EditorGUIUtility.labelWidth = 80;
                manager.fogColorA = EditorGUILayout.ColorField( new GUIContent( "Color A:", "Color of fog at position A" ), manager.fogColorA );
                manager.fogColorB = EditorGUILayout.ColorField( new GUIContent( "Color B:", "Color of fog at position B" ), manager.fogColorB );
                manager.fogIntensityA = EditorGUILayout.Slider( new GUIContent( "Intensity A:", "Intensity of fog at position A" ), manager.fogIntensityA, 0, 1 );
                manager.fogIntensityB = EditorGUILayout.Slider( new GUIContent( "Intensity B:", "Intensity of fog at position B" ), manager.fogIntensityB, 0, 1 );
                manager.fogPositionA = EditorGUILayout.FloatField( new GUIContent( "Position A:", "World space fog in Y (Vertical)" ), manager.fogPositionA );
                manager.fogPositionB = EditorGUILayout.FloatField( new GUIContent( "Position B:", "World space fog in Y (Vertical)" ), manager.fogPositionB );
                manager.fogStart = EditorGUILayout.FloatField( new GUIContent( "Start:", "Position starting from the camera that the fog is completly transparent." ), manager.fogStart );
                manager.fogEnd = EditorGUILayout.FloatField( new GUIContent( "End:", "Position starting from the camera that the fog is completly opaque." ), manager.fogEnd );
            }

            EditorGUILayout.Space();

            serializedObject.Update();
            SerializedProperty mats = serializedObject.FindProperty( "materials" );
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField( mats, true );
            if( EditorGUI.EndChangeCheck() )
            {
                serializedObject.ApplyModifiedProperties();
            }

            if( GUI.changed )
            {
                EditorUtility.SetDirty( target );
            }
        }
    }
#endif
}