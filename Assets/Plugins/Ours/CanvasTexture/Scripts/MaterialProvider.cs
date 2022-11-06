using System;

namespace ViJApps.CanvasTexture
{
    /// <summary>
    /// This attribute is used for SourceGenerator. Add it to partial static class to make it int properties provider
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ShaderPropertiesProviderAttribute: Attribute
    {
    }

    /// <summary>
    /// This attribute is used for SourceGenerator. Add it to a string constant with shader name to make a shader property for it
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ShaderAttribute : Attribute
    {
    }
    
    /// <summary>
    /// This attribute is used for SourceGenerator. Add it to a string constant with a shader property name to make a shader property for it
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ShaderPropertyAttribute : Attribute
    {
    }

    [ShaderPropertiesProvider]
    public static partial class MaterialProvider
    {
        //Shader Names
        [Shader] private const string SIMPLE_UNLIT = "ViJApps.SimpleUnlit";
        [Shader] private const string SIMPLE_UNLIT_TRANSPARENT = "ViJApps.SimpleUnlitTransparent";
        [Shader] private const string SIMPLE_LINE_UNLIT = "ViJApps.SimpleLineUnlit";
        [Shader] private const string SIMPLE_CIRCLE_UNLIT = "ViJApps.SimpleCircleUnlit";
        [Shader] private const string SIMPLE_ELLIPSE_UNLIT = "ViJApps.SimpleEllipseUnlit";

        //Property 
        [ShaderProperty] private const string COLOR = "_Color";
        [ShaderProperty] private const string FILL_COLOR = "_FillColor";
        [ShaderProperty] private const string STROKE_COLOR = "_StrokeColor";
        
        [ShaderProperty] private const string THICKNESS = "_Thickness";
        [ShaderProperty] private const string FROM_TO_COORD = "_FromToCoord";
        [ShaderProperty] private const string ASPECT = "_Aspect";
        [ShaderProperty] private const string RADIUS = "_Radius";
        [ShaderProperty] private const string CENTER = "_Center";
        [ShaderProperty] private const string AB_FILL_STROKE = "_AbFillStroke";
        
        [ShaderProperty] private const string TRANSFORM_MATRIX = "_TransformMatrix";
        
        [ShaderProperty] private const string MATRIX_COLUMN_0 = "_MatrixColumn0";
        [ShaderProperty] private const string MATRIX_COLUMN_1 = "_MatrixColumn1";
        [ShaderProperty] private const string MATRIX_COLUMN_2 = "_MatrixColumn2";
        [ShaderProperty] private const string MATRIX_COLUMN_3 = "_MatrixColumn3";
    }
}