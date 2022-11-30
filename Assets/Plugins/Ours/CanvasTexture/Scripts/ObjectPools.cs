using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

namespace ViJApps.CanvasTexture
{
    public class MeshPool : ObjectPool<Mesh>
    {
        public MeshPool() : base(() => new Mesh(), null, (c) => c.Clear(), Object.Destroy)
        {
        }
    }

    public class TextComponentsPool : ObjectPool<TextComponent>
    { 
        public TextComponentsPool() : base(CreateFromAddressable, Activate, Deactivate, (c) => Object.Destroy(c.gameObject))
        {
        }

        private static void Deactivate(TextComponent textComponent)
        {   
            textComponent.Clear();
            textComponent.gameObject.SetActive(false);
        }
        
        private static void Activate(TextComponent textComponent)
        {
            textComponent.gameObject.SetActive(true);
        }

        private static TextComponent CreateFromAddressable()
        {
            // var op = Addressables.InstantiateAsync("TextComponent");
            // var renderer = op.WaitForCompletion().GetComponent<TextComponent>();
            // return renderer;
            
            var op = Object.Instantiate(Resources.Load("TextComponent"));
            var renderer = ((GameObject)op).GetComponent<TextComponent>();
            return renderer;
        }
    }

    public class PropertyBlockPool : ObjectPool<MaterialPropertyBlock>
    {
        public PropertyBlockPool() : base(() => new MaterialPropertyBlock(), null, c => c.Clear(), null, true, 10, 1000)
        {
        }
    }
}