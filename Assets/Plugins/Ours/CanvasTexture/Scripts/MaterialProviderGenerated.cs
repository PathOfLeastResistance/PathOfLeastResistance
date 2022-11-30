
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

namespace ViJApps.CanvasTexture
{
    public static partial class MaterialProvider 
    { 
		public static int ColorPropertyId {get; private set;}
		public static int FillColorPropertyId {get; private set;}
		public static int StrokeColorPropertyId {get; private set;}
		public static int ThicknessPropertyId {get; private set;}
		public static int FromToCoordPropertyId {get; private set;}
		public static int AspectPropertyId {get; private set;}
		public static int RadiusPropertyId {get; private set;}
		public static int CenterPropertyId {get; private set;}
		public static int AbFillStrokePropertyId {get; private set;}
		public static int TransformMatrixPropertyId {get; private set;}
		public static int MatrixColumn0PropertyId {get; private set;}
		public static int MatrixColumn1PropertyId {get; private set;}
		public static int MatrixColumn2PropertyId {get; private set;}
		public static int MatrixColumn3PropertyId {get; private set;}

		public static int SimpleUnlitShaderId {get; private set;}
		public static int SimpleUnlitTransparentShaderId {get; private set;}
		public static int SimpleLineUnlitShaderId {get; private set;}
		public static int SimpleCircleUnlitShaderId {get; private set;}
		public static int SimpleEllipseUnlitShaderId {get; private set;}
 

        private static readonly Dictionary<int, Shader> Shaders = new Dictionary<int, Shader>();
        private static readonly Dictionary<int, Material> Materials = new Dictionary<int, Material>();

        static MaterialProvider()
        {
	        Init();
        }

        private static void Init() 
        { 
            var shaderTasks = new List<Task<Shader>>();
            var shaderIds = new List<int>();
			ColorPropertyId = Shader.PropertyToID(COLOR);
			FillColorPropertyId = Shader.PropertyToID(FILL_COLOR);
			StrokeColorPropertyId = Shader.PropertyToID(STROKE_COLOR);
			ThicknessPropertyId = Shader.PropertyToID(THICKNESS);
			FromToCoordPropertyId = Shader.PropertyToID(FROM_TO_COORD);
			AspectPropertyId = Shader.PropertyToID(ASPECT);
			RadiusPropertyId = Shader.PropertyToID(RADIUS);
			CenterPropertyId = Shader.PropertyToID(CENTER);
			AbFillStrokePropertyId = Shader.PropertyToID(AB_FILL_STROKE);
			TransformMatrixPropertyId = Shader.PropertyToID(TRANSFORM_MATRIX);
			MatrixColumn0PropertyId = Shader.PropertyToID(MATRIX_COLUMN_0);
			MatrixColumn1PropertyId = Shader.PropertyToID(MATRIX_COLUMN_1);
			MatrixColumn2PropertyId = Shader.PropertyToID(MATRIX_COLUMN_2);
			MatrixColumn3PropertyId = Shader.PropertyToID(MATRIX_COLUMN_3);

            SimpleUnlitShaderId = Shader.PropertyToID(SIMPLE_UNLIT);
	        Shaders[SimpleUnlitShaderId] = (Shader)Resources.Load(SIMPLE_UNLIT);
	        Materials[SimpleUnlitShaderId] = new Material(Shaders[SimpleUnlitShaderId]);
                        
            SimpleUnlitTransparentShaderId = Shader.PropertyToID(SIMPLE_UNLIT_TRANSPARENT);
            Shaders[SimpleUnlitTransparentShaderId] = (Shader)Resources.Load(SIMPLE_UNLIT_TRANSPARENT);
            Materials[SimpleUnlitTransparentShaderId] = new Material(Shaders[SimpleUnlitTransparentShaderId]);
                        
            SimpleLineUnlitShaderId = Shader.PropertyToID(SIMPLE_LINE_UNLIT);
            Shaders[SimpleLineUnlitShaderId] = (Shader)Resources.Load(SIMPLE_LINE_UNLIT);
            Materials[SimpleLineUnlitShaderId] = new Material(Shaders[SimpleLineUnlitShaderId]);
            
            SimpleCircleUnlitShaderId = Shader.PropertyToID(SIMPLE_CIRCLE_UNLIT);
            Shaders[SimpleCircleUnlitShaderId] = (Shader)Resources.Load(SIMPLE_CIRCLE_UNLIT);
            Materials[SimpleCircleUnlitShaderId] = new Material(Shaders[SimpleCircleUnlitShaderId]);
                        
            SimpleEllipseUnlitShaderId = Shader.PropertyToID(SIMPLE_ELLIPSE_UNLIT);
            Shaders[SimpleEllipseUnlitShaderId] = (Shader)Resources.Load(SIMPLE_ELLIPSE_UNLIT);
            Materials[SimpleEllipseUnlitShaderId] = new Material(Shaders[SimpleEllipseUnlitShaderId]);
        }

        public static Shader GetShader(int shader) => Shaders[shader];

        public static Material GetMaterial(int shader) => Materials[shader];
    }
}