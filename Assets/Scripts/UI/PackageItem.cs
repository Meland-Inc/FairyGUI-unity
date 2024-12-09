using UnityEngine;
using FairyGUI.Utils;
using System;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class PackageItem
    {
        public Action<PackageItem> OnRealUnload;
        public UIPackage owner;

        public PackageItemType type;
        public ObjectType objectType;

        public string id;
        public string name;
        public int width;
        public int height;
        public string file;
        public bool exported;
        public NTexture texture;
        public ByteBuffer rawData;
        public string[] branches;
        public string[] highResolution;

        //image
        public Rect? scale9Grid;
        public bool scaleByTile;
        public int tileGridIndice;
        public PixelHitTestData pixelHitTestData;

        //movieclip
        public float interval;
        public float repeatDelay;
        public bool swing;
        public MovieClip.Frame[] frames;

        //component
        public bool translated;
        public UIObjectFactory.GComponentCreator extensionCreator;

        //font
        public BitmapFont bitmapFont;

        //sound
        public NAudioClip audioClip;

        //spine/dragonbones
        public Vector2 skeletonAnchor;
        public object skeletonAsset;
        public int LoadCounter { get; private set; } = 0;

        public object Load()
        {
            LoadCounter++;
            return owner.GetItemAsset(this);
        }

        public void Unload()
        {
            if (LoadCounter == 0)
            {
                Debug.LogError("Redundant unload: " + file);
                return;
            }

            LoadCounter--;
            if (LoadCounter == 0)
            {
                RealUnload();
            }
        }

        public void RealUnload()
        {
            if (type == PackageItemType.Image)
            {
                if (texture != null)
                {
                    texture = null;
                    // OnRealUnload?.Invoke(this);//image类型暂时不用派发，目前业务层没有监听，将来也不大可能有，节省性能
                }

                PackageItem refAtlasPackageItem = owner.GetAtlasPackageItem(id);
                if (refAtlasPackageItem != null)
                {
                    refAtlasPackageItem.Unload();
                }
            }
            else if (type == PackageItemType.Atlas)
            {
                if (texture != null)
                {
                    texture.Unload();
                    texture = null;
                    OnRealUnload?.Invoke(this);
                }
            }
            else if (type == PackageItemType.Sound)
            {
                if (audioClip != null)
                {
                    audioClip.Unload();
                    audioClip = null;
                    OnRealUnload?.Invoke(this);
                }
            }
            else
            {
                //其他的暂时不需要处理
            }
        }

        public PackageItem getBranch()
        {
            if (branches != null && owner._branchIndex != -1)
            {
                string itemId = branches[owner._branchIndex];
                if (itemId != null)
                    return owner.GetItem(itemId);
            }

            return this;
        }

        public PackageItem getHighResolution()
        {
            if (highResolution != null && GRoot.contentScaleLevel > 0)
            {
                int i = GRoot.contentScaleLevel - 1;
                if (i >= highResolution.Length)
                    i = highResolution.Length - 1;
                string itemId = highResolution[i];
                if (itemId != null)
                    return owner.GetItem(itemId);
            }

            return this;
        }
    }
}
