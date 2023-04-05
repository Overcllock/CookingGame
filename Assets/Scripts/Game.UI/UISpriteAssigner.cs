using Game.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Game.UI
{
    public class UISpriteAssigner : IDisposable
    {
        private const float MIN_PLACEHOLDER_TIME = 0.55f;

        private IPlaceholder _placeholder;

        private IEnumerator _loadRoutine;
        private AssetReference _latestRequired;

        private HashSet<AssetReference> _loading = new HashSet<AssetReference>();
        private Dictionary<AssetReference, Sprite> _sprites = new Dictionary<AssetReference, Sprite>();

        public UISpriteAssigner()
        {
        }

        public UISpriteAssigner(IPlaceholder placeholder)
        {
            _placeholder = placeholder;
        }

        public void Dispose()
        {
            _placeholder?.Dispose();
            _placeholder = null;

            _sprites = null;
            TryCancelLoad();

            if (_loadRoutine != null)
            {
                EngineEvents.CancelCoroutine(_loadRoutine);
                _loadRoutine = null;
            }
        }

        public void SetSprite(AssetReferenceSprite reference, Image[] images, Action callback = null)
        {
            var info = AssignInfo.Create(reference, images, callback);
            Process(info);
        }

        public void SetSprite(AssetReferenceSprite reference, Image image, Action callback = null)
        {
            var info = AssignInfo.Create(reference, image, callback);
            Process(info);
        }

        public void TryCancelLoad()
        {
            if (_latestRequired != null)
            {
                _latestRequired = null;
                _placeholder?.Hide();
            }
        }

        private void Process(AssignInfo info)
        {
            TryCancelLoad();

            if (_sprites.TryGetValue(info.reference, out Sprite sprite))
            {
                info.SetSprite(sprite);
                
                info.ActivateImages(true);
                
                info.callback?.Invoke();
            }
            else
            {
                _latestRequired = info.reference;
                if (_loading.Add(info.reference))
                {
                    _loadRoutine = PictureLoadRoutine(info);
                    EngineEvents.ExecuteCoroutine(_loadRoutine);
                }
            }
        }

        private IEnumerator PictureLoadRoutine(AssignInfo info)
        {
            var reference = info.reference;

            info.ActivateImages(false);

            _placeholder?.Show();

            float time = Time.time;

            var spriteContainer = AssetsLoader.Load<Sprite>(reference);
            yield return spriteContainer;

            if (spriteContainer.isCancelled) yield break;

            if (_placeholder != null)
            {
                float delta = Time.time - time;
                if (delta < MIN_PLACEHOLDER_TIME)
                {
                    yield return Wait.ForSeconds(MIN_PLACEHOLDER_TIME - delta);
                }
            }

            _sprites[spriteContainer.reference] = spriteContainer.asset;
            _loading.Remove(reference);

            if (_latestRequired == reference)
            {
                info.SetSprite(spriteContainer.asset);
                _placeholder?.Hide();

                info.ActivateImages(true);
                _latestRequired = null;
                
                info.callback?.Invoke();
            }
        }

        private struct AssignInfo
        {
            public AssetReferenceSprite reference;

            public Image image;
            public Image[] images;

            public Action callback;

            public void ActivateImages(bool active)
            {
                if (image != null)
                {
                    image.SetActive(active);
                }

                if (images != null)
                {
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].SetActive(active);
                    }
                }
            }

            public void SetSprite(Sprite sprite)
            {
                if (image != null)
                {
                    image.sprite = sprite;
                }

                if (images != null)
                {
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].sprite = sprite;
                    }
                }
            }

            public static AssignInfo Create(AssetReferenceSprite reference, Image image, Action callback = null)
            {
                return new AssignInfo
                {
                    reference = reference,
                    image = image,
                    callback = callback
                };
            }

            public static AssignInfo Create(AssetReferenceSprite reference, Image[] images, Action callback = null)
            {
                return new AssignInfo
                {
                    reference = reference,
                    images = images,
                    callback = callback
                };
            }
        }
    }
}