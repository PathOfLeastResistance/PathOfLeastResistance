//Matrix operations
float3x3 ColumnsToMatrices(in float3 c1, in float3 c2, in float3 c3)
{
    return float3x3(c1, c2, c3);
}

float3x3 ScaleMatrixFromAspect(in float aspect)
{
    return float3x3(float3(aspect, 0, 0), float3(0, 1, 0), float3(0, 0, 1));
}

float3x3 InverseScaleMatrixFromAspect(in float aspect)
{
    return float3x3(float3(1.0 / aspect, 0, 0), float3(0, 1, 0), float3(0, 0, 1));
}

float2 TransformPoint(in float3x3 m, in float2 point2d)
{
    return mul(m, float3(point2d.x, point2d.y, 1)).xy;
}

float2 TransformDirection(in float3x3 m, in float2 point2d)
{
    return mul(m, float3(point2d.x, point2d.y, 0)).xy;
}

float sdfUnion(float sdf1, float sdf2)
{
    return min(sdf1, sdf2);
}

float sdfSubtract(float sdf1, float sdf2)
{
    return max(sdf1, -sdf2);
}


//SDF LINE
float sdLineSegment(in half2 p, in half2 a, in half2 b)
{
    half2 ba = b - a;
    half2 pa = p - a;
    half h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - h * ba);
}

//SDF CIRCLE
float sdCircle(in half2 p, in half2 c)
{
    return length(p - c);
}

float msign(in float x) { return (x < 0.0) ? -1.0 : 1.0; }

//SDF ELLIPSE

// The MIT License
// Copyright © 2015 Inigo Quilez
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Using Newtown's root solver to compute the distance to
// an ellipse, instead of using the analytical solution in
// https://www.shadertoy.com/view/4sS3zz.
//
// In retrospect, it's the same as Antonalog's https://www.shadertoy.com/view/MtXXW7
//
// More information here:
//
// https://iquilezles.org/articles/ellipsedist
//
//
// Ellipse distances related shaders:
//
// Analytical     : https://www.shadertoy.com/view/4sS3zz
// Newton Trig    : https://www.shadertoy.com/view/4lsXDN
// Newton No-Trig : https://www.shadertoy.com/view/tttfzr 
// ?????????????? : https://www.shadertoy.com/view/tt3yz7

// List of some other 2D distances: https://www.shadertoy.com/playlist/MXdSRf
//
// and iquilezles.org/articles/distfunctions2d

/**
 * \brief 
 * \param p point position
 * \param ab radius of the ellipse
 * \return 
 */
float sdEllipse(float2 p, float2 ab)
{
    // symmetry
    p = abs(p);

    // find root with Newton solver
    float2 q = ab * (p - ab);
    float w = (q.x < q.y) ? 1.570796327 : 0.0;
    for (int i = 0; i < 5; i++)
    {
        float2 cs = float2(cos(w), sin(w));
        float2 u = ab * float2(cs.x, cs.y);
        float2 v = ab * float2(-cs.y, cs.x);
        w = w + dot(p - u, v) / (dot(p - u, u) + dot(v, v));
    }

    // compute final point and distance
    float d = length(p - ab * float2(cos(w), sin(w)));

    // return signed distance
    return (dot(p / ab, p / ab) > 1.0) ? d : -d;
}
