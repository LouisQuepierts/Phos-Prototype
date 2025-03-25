#ifndef PHOS_GRADIENT_CG_INCLUDED
#define PHOS_GRADIENT_CG_INCLUDED

#ifdef GRADIENT
fixed4 _ScreenHigherAmbient;
fixed4 _ScreenLowerAmbient;
fixed _ScreenOffset;
fixed _ScreenFactor;
#endif
#include <HLSLSupport.cginc>

// Official HSV to RGB conversion 
inline fixed3 hsv2rgb( fixed3 c )
{
    fixed3 rgb = clamp( abs(fmod(c.x*6.0+fixed3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
    return c.z * lerp( 1.0, rgb, c.y);
}
// Smooth HSV to RGB conversion 
// https://www.shadertoy.com/view/MsS3Wc
inline fixed3 rgb2hsv( fixed3 c ){
    fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    fixed4 p = lerp(fixed4(c.bg, K.wz),fixed4(c.gb, K.xy),step(c.b, c.g));
    fixed4 q = lerp(fixed4(p.xyw, c.r),fixed4(c.r, p.yzx),step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)),d / (q.x + e),q.x);
}

inline fixed4 phos_gradient(fixed y)
{
    #ifdef GRADIENT
    return lerp(_ScreenLowerAmbient, _ScreenHigherAmbient, saturate((y + _ScreenOffset) * _ScreenFactor));
    #else
    return fixed4(1, 1, 1, 1);
    #endif
}

inline fixed4 phos_gradient(fixed y, fixed4 origin)
{
    #ifdef GRADIENT
    fixed4 ambient = phos_gradient(y);
    return fixed4(lerp(origin.rgb, ambient.rgb, ambient.a), origin.a);
    #else
    return origin;
    #endif
}

inline fixed4 phos_gradient_hsv(fixed y)
{
    #ifdef GRADIENT
    fixed t = saturate((y + _ScreenOffset) * _ScreenFactor);
    fixed3 hsv = lerp(rgb2hsv(_ScreenLowerAmbient.rgb), rgb2hsv(_ScreenHigherAmbient.rgb), t);
    return fixed4(hsv2rgb(hsv), lerp(_ScreenLowerAmbient.a, _ScreenHigherAmbient.a, t));
    #else
    return fixed4(1, 1, 1, 1);
    #endif
}

inline fixed4 phos_gradient_hsv(fixed y, fixed4 origin)
{
    #ifdef GRADIENT
    fixed4 ambient = phos_gradient_hsv(y);
    return fixed4(lerp(origin.rgb, ambient.rgb, ambient.a), origin.a);
    #else
    return origin;
    #endif
}

#endif