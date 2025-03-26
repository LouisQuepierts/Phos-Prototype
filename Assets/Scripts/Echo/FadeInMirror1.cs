using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Echo
{

    public class FadeInMirror1 : MonoBehaviour
    {
        public float fadeDuration = 2f; // ��������ʱ��
        private float currentAlpha = 0f; // ��ǰ͸����
        private Material material; // ���ӵĲ���

        void Start()
        {
            // ��ȡ���ӵĲ���
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                material = renderer.material;
            }
            else
            {
                Debug.LogWarning("No Renderer component found on mirror!");
            }

            // ��ʼ��͸����Ϊ 0
            SetAlpha(0f);
        }

        void Update()
        {
            // ������͸����
            if (currentAlpha < 1f)
            {
                currentAlpha += Time.deltaTime / fadeDuration;
                SetAlpha(currentAlpha);
            }
        }

        void SetAlpha(float alpha)
        {
            if (material != null)
            {
                // ��ȡ��ǰ��ɫ
                Color color = material.color;
                // �����µ�͸����
                color.a = alpha;
                // ���²�����ɫ
                material.color = color;
            }
        }
    }
}